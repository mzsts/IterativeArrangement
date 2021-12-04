using Microsoft.Win32;
using System.Windows;
using IterativeArrangement.Services;
using IterativeArrangement.Models;
using System.Collections.Generic;
using System.Data;

using Microsoft.Msagl.Prototype.Ranking;
using Microsoft.Msagl.Drawing;

namespace IterativeArrangement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

                        edge.LabelText = item.Elements[i].Nets.Find(el => el.Net.Name == item.Name).Pins.Count.ToString();
                        
                        edge.Attr.ArrowheadAtSource = ArrowStyle.None;
                        edge.Attr.ArrowheadAtTarget = ArrowStyle.None;

                        graph.AddPrecalculatedEdge(edge);
                    }
                }
            }

            graph.LayoutAlgorithmSettings = new RankingLayoutSettings();

            gViewer.Graph = graph;
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
