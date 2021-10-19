using System.Collections.Generic;
using System.Data;
using IterativeArrangement.Models;
using System.Linq;

namespace IterativeArrangement.Services
{
    public static class MatrixBuilder
    {
        public static DataTable GetMatrixA(List<Element> elements, List<Net> nets)
        {
            List<int> pins = new();
            foreach (Element element in elements)
            {
                foreach (var item in element.Nets)
                {
                    foreach (int pin in item.Pins)
                    {
                        if (pins.Contains(pin) is false)
                        {
                            pins.Add(pin);
                        }
                    }
                }
            }
            pins.Sort();

            DataTable matrixA = new("Matrix A");

            DataColumn column;
            DataRow row;

            column = new("A", typeof(string));
            matrixA.Columns.Add(column);

            foreach (Net item in nets)
            {
                column = new($"{item.Name}", typeof(int));
                matrixA.Columns.Add(column);
            }

            for (int i = 0; i < pins.Count; i++)
            {
                row = matrixA.NewRow();
                row[0] = (i + 1).ToString();
                for (int j = 1; j <= nets.Count; j++)
                {
                    row[j] = 0;
                    foreach (Element element in nets[j - 1].Elements)
                    {
                        if (element.Nets.Find(n => n.Net.Name == nets[j - 1].Name).Pins.Contains(i + 1))
                        {
                            row[j] = 1;
                        }
                    }
                }
                matrixA.Rows.Add(row);
            }

            return matrixA;
        }

        public static DataTable GetMatrixB(List<Element> elements)
        {
            int maxPinNumber = default;
            foreach (Element element in elements)
            {
                foreach ((Net net, List<int> pins) in element.Nets)
                {
                    if (pins.Max() > maxPinNumber)
                    {
                        maxPinNumber = pins.Max();
                    }
                }
            }

            DataTable matrixB = new("Matrix B");

            DataColumn column = new("B", typeof(string));
            DataRow row;

            matrixB.Columns.Add(column);

            foreach (Element element in elements)
            {
                column = new(element.Name, typeof(int));
                matrixB.Columns.Add(column);
            }

            for (int i = 0; i < maxPinNumber; i++)
            {
                row = matrixB.NewRow();
                row[0] = (i + 1).ToString();
                for (int j = 1; j <= elements.Count; j++)
                {
                    row[j] = 0;

                    foreach (var item in elements[j - 1].Nets)
                    {
                        if (item.Pins.Contains(i +1))
                        {
                            row[j] = 1;
                        }
                    }
                }
                matrixB.Rows.Add(row);
            }

            return matrixB;
        }

        public static DataTable GetMatrixQ(DataTable matrixA, DataTable matrixB)
        {
            DataTable matrixQ = new("Matrix Q");

            DataColumn column = new("Q", typeof(string));
            DataRow row;

            matrixQ.Columns.Add(column);

            foreach (DataColumn item in matrixB.Columns)
            {
                column = new(item.ColumnName, typeof(int));
                matrixQ.Columns.Add(column);
            }

            for (int i = 1; i < matrixA.Columns.Count; i++)
            {
                row = matrixQ.NewRow();
                row[0] = matrixA.Columns[i].ColumnName;

                for (int j = 1; j < matrixB.Columns.Count; j++)
                {
                    row[j] = GetSum(matrixA, matrixB, i, j);
                }
                matrixQ.Rows.Add(row);
            }

            return matrixQ;
        }

        private static int GetSum(DataTable matrixA, DataTable matrixB, int indexA, int indexB)
        {
            List<int> mult = new();
            
            for (int i = 0; i < matrixA.Rows.Count; i++)
            {
                mult.Add((int)matrixA.Rows[i][indexA] * (int)matrixB.Rows[i][indexB]);
            }

            return mult.Sum();
        }
    }
}
