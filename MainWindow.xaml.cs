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
