using Microsoft.Win32;
using System.Windows;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Msagl.Prototype.Ranking;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Core.Routing;

using IterativeArrangement.Models;
using IterativeArrangement.Services;
using System.Diagnostics;
using System;

namespace IterativeArrangement
{
    public partial class MainWindow : Window
    {   
        private List<Element> elements;
        private List<Net> nets;

        private DataTable matrixR;
        private DataTable matrixQ;

        public MainWindow()
        {
            InitializeComponent(); 
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                Title = "Выберите файл данных",
                Filter = "Файл данных (*.net)|*.net"
            };

            if (openFileDialog.ShowDialog() is true)
            {
                GetDataFromFile(openFileDialog.FileName);
            }
        }

        private void CreateGraph(object sender, RoutedEventArgs e)
        {
            Graph graph = new();

            Node node;
            foreach (Element item in elements)
            {
                node = new(item.Name);
                node.Attr.FillColor = Color.BlueViolet;
                node.Attr.Color = Color.Black;

                graph.AddNode(node);
            }

            Edge edge;
            foreach (Net item in nets)
            {
                for (int i = 0; i < item.Elements.Count - 1; i++)
                {
                    for (int j = i + 1; j < item.Elements.Count; j++)
                    {
                        edge = new(graph.FindNode(item.Elements[i].Name), 
                            graph.FindNode(item.Elements[j].Name), ConnectionToGraph.Connected);
                        
                        edge.Attr.ArrowheadAtSource = ArrowStyle.None;
                        edge.Attr.ArrowheadAtTarget = ArrowStyle.None;

                        graph.AddPrecalculatedEdge(edge);
                    }
                }
            }

            graph.LayoutAlgorithmSettings.EdgeRoutingSettings.EdgeRoutingMode = EdgeRoutingMode.StraightLine;

            graph.LayoutAlgorithmSettings = new RankingLayoutSettings();

            gViewer.Graph = graph;
        }

        private void RebuildGraph()
        {
            Random rnd = new();
            int max = 255 / elements.Max(el => el.Group);

            List<Color> colors = new();
            for (int i = 1; i <= elements.Max(el => el.Group); i++)
            {
                colors.Add(new((byte)rnd.Next(max * i), (byte)rnd.Next(max * i), (byte)rnd.Next(max * i)));
            }

            Graph graph = new();

            Node node;
            foreach (Element item in elements)
            {
                node = new(item.Name);
                node.Attr.FillColor = colors[item.Group - 1];
                node.Attr.Color = Color.Black;

                graph.AddNode(node);
            }

            Edge edge;
            foreach (Net item in nets)
            {
                for (int i = 0; i < item.Elements.Count - 1; i++)
                {
                    for (int j = i + 1; j < item.Elements.Count; j++)
                    {
                        edge = new(graph.FindNode(item.Elements[i].Name),
                            graph.FindNode(item.Elements[j].Name), ConnectionToGraph.Connected);

                        edge.Attr.ArrowheadAtSource = ArrowStyle.None;
                        edge.Attr.ArrowheadAtTarget = ArrowStyle.None;

                        graph.AddPrecalculatedEdge(edge);
                    }
                }
            }

            graph.LayoutAlgorithmSettings.EdgeRoutingSettings.EdgeRoutingMode = EdgeRoutingMode.StraightLine;

            graph.LayoutAlgorithmSettings = new RankingLayoutSettings();

            gViewer.Graph = graph;
        }

        private void Compose(object sender, RoutedEventArgs e)
        {
            int groupCount = 2;

            for (int i = 0, j = 1; i < elements.Count; i++)
            {
                if (i == elements.Count / groupCount)
                {
                    j++;
                }

                elements[i].Group = j;
            }

            CalculateConnectivityCoefficient();

            var permutations = CreatePermutationList();

            while (permutations.Max() is var per && per.Total > 0)
            {
                per.Perform();
                CalculateConnectivityCoefficient();
                permutations = CreatePermutationList();
            }

            RebuildGraph();
        }

        private void CalculateConnectivityCoefficient()
        {
            foreach (DataRow row in matrixR.Rows)
            {
                var current = elements.Find(el => el.Name == ((string)row[0]));

                current.ConnectivityCoefficient = 0;

                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (matrixR.Columns[i].ColumnName != (string)row[0])
                    {
                        if (elements.Find(el => el.Name == matrixR.Columns[i].ColumnName).Group == current.Group)
                        {
                            current.ConnectivityCoefficient -= (int)row[i];
                        }
                        else
                        {
                            current.ConnectivityCoefficient += (int)row[i];
                        }
                    }
                }
            }
        }

        private List<Permutation> CreatePermutationList()
        {
            List<Permutation> permutations = new();

            foreach (var element in elements)
            {
                var others = elements.FindAll(el => el.Group != element.Group);

                foreach (var other in others)
                {
                    string rowS = (string)matrixR.Rows[matrixR.Columns.IndexOf(other.Name) - 1][0];
                    string colS = matrixR.Columns[matrixR.Columns.IndexOf(other.Name)].ColumnName;

                    permutations.Add(
                        new Permutation((element, other),
                        (element.ConnectivityCoefficient + other.ConnectivityCoefficient) - 
                        (2 * (int)matrixR.Rows[matrixR.Columns.IndexOf(element.Name) - 1][matrixR.Columns.IndexOf(other.Name)])));
                }
            }

            return permutations;
        }

        private void GetDataFromFile(string path)
        {
            fileData.Text = string.Empty;

            List<string> temp = Parser.ParseToStrings(
                    DataProvider.ReadFromFile(path));

            foreach (string item in temp)
            {
                if (string.IsNullOrEmpty(item.Trim()) is false)
                {
                    fileData.Text += $"{item}\n";
                }
            }

            (elements, nets) = Parser.ParseToObjects(temp);

            elementsCount.Text = $"Количество элементов: {elements.Count}";

            InitTables();
        }

        private void InitTables()
        {
            matrixQ = MatrixBuilder.GetMatrixQ(elements, nets);
            matrixR = MatrixBuilder.GetMatrixR(elements);

            matrixQDataGrid.DataContext = matrixQ;
            matrixQDataGrid.ItemsSource = matrixQ.DefaultView;

            matrixRDataGrid.DataContext = matrixR;
            matrixRDataGrid.ItemsSource = matrixR.DefaultView;
        }
    }
}
