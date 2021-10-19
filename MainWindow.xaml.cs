using Microsoft.Win32;
using System.Windows;
using IterativeArrangement.Services;
using IterativeArrangement.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Controls;

namespace IterativeArrangement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Element> elements;
        private List<Net> nets;
        private DataTable table;
        private DataTable matrixA;
        private DataTable matrixB;
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

        private void FillDataGrid()
        {
            table = new("elemetnsTable");
            DataColumn column;
            DataRow row;

            column = new("Els", typeof(string));

            table.Columns.Add(column);

            foreach (Element item in elements)
            {
                column = new(item.Name, typeof(int));
                table.Columns.Add(column);
            }

            for (int i = 0; i < elements.Count; i++)
            {
                row = table.NewRow();
                row[0] = elements[i].Name;
                for (int j = 1; j <= elements.Count; j++)
                {
                    if (table.Columns[j].ColumnName == elements[i].Name)
                    {
                        row[j] = 0;
                    }
                    else
                    {
                        int linksCount = 0;
                        foreach ((Net net, List<int> _) in elements[i].Nets)
                        {
                            if (net.Elements.Find(el => el.Name == table.Columns[j].ColumnName) is not null)
                            {
                                linksCount++;
                            }
                        }
                        row[j] = linksCount;
                    }
                }
                table.Rows.Add(row);
            }
            elementsDataGrid.DataContext = table;
            elementsDataGrid.ItemsSource = table.DefaultView;
        }

        private void InitTables()
        {
            FillDataGrid();

            matrixA = MatrixBuilder.GetMatrixA(elements, nets);
            matrixB = MatrixBuilder.GetMatrixB(elements);
            matrixQ = MatrixBuilder.GetMatrixQ(matrixA, matrixB);

            matrixADataGrid.DataContext = matrixA;
            matrixADataGrid.ItemsSource = matrixA.DefaultView;

            matrixBDataGrid.DataContext = matrixB;
            matrixBDataGrid.ItemsSource = matrixB.DefaultView;

            matrixQDataGrid.DataContext = matrixQ;
            matrixQDataGrid.ItemsSource = matrixQ.DefaultView;
        }
    }
}
