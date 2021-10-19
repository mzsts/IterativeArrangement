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
            List<Net> nets = new();
            int maxPinNumber = default;
            foreach (Element element in elements)
            {
                foreach ((Net net, List<int> pins) in element.Nets)
                {
                    if (pins.Max() > maxPinNumber)
                    {
                        maxPinNumber = pins.Max();
                    }
                    if (nets.Contains(net) is false)
                    {
                        nets.Add(net);
                    }
                }
            }

            DataTable matrixB = new("Matrix B");

            DataColumn column = new("B", typeof(string));
            DataRow row;

            matrixB.Columns.Add(column);

            foreach (Net item in nets)
            {
                column = new(item.Name, typeof(int));
                matrixB.Columns.Add(column);
            }

            for (int i = 0; i < maxPinNumber; i++)
            {
                row = matrixB.NewRow();
                row[0] = (i + 1).ToString();
                for (int j = 1; j < matrixB.Columns.Count; j++)
                {
                    row[j] = 0;
                    foreach (Element element in nets[i].Elements)
                    {
                        foreach (var item in element.Nets)
                        {
                            if (item.Pins.Contains(i + 1))
                            {
                                row[j] = 1;
                            }
                        }
                    }
                }
                matrixB.Rows.Add(row);
            }

            return matrixB;
        }

        public static DataTable GetMatrixQ(DataTable matrixA, DataTable matixB)
        {
            DataTable matrixQ = new("Matrix Q");

            DataColumn column = new("Q", typeof(string));
            DataRow row;



            return matrixQ;
        }
    }
}
