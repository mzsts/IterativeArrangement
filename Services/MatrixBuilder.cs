using System.Collections.Generic;
using System.Data;
using IterativeArrangement.Models;

namespace IterativeArrangement.Services
{
    public static class MatrixBuilder
    {
        public static DataTable GetMatrixQ(List<Element> elements, List<Net> nets)
        {
            DataTable matrixQ = new DataTable("Matrix Q");

            DataColumn column = new DataColumn("Q=", typeof(string));
            DataRow row;

            matrixQ.Columns.Add(column);

            foreach (Element item in elements)
            {
                column = new DataColumn(item.Name, typeof(int));
                matrixQ.Columns.Add(column);
            }

            for (int i = 0; i < nets.Count; i++)
            {
                row = matrixQ.NewRow();
                row[0] = nets[i].Name;

                for (int j = 1; j < matrixQ.Columns.Count; j++)
                {
                    row[j] = 0;
                    if (elements[j - 1].Nets.Find(net => net.Net.Name == nets[i].Name)
                        is (Net, List<int>) net && net != (null, null))
                    {
                        row[j] = net.Pins.Count;
                    }
                }

                matrixQ.Rows.Add(row);
            }

            return matrixQ;
        }

        public static DataTable GetMatrixR(List<Element> elements)
        {
            DataTable matrixR = new DataTable("Matrix Table");
            DataColumn column;
            DataRow row;

            column = new DataColumn("R=", typeof(string));

            matrixR.Columns.Add(column);

            foreach (Element item in elements)
            {
                column = new DataColumn(item.Name, typeof(int));
                matrixR.Columns.Add(column);
            }

            for (int i = 0; i < elements.Count; i++)
            {
                row = matrixR.NewRow();
                row[0] = elements[i].Name;
                for (int j = 1; j <= elements.Count; j++)
                {
                    if (matrixR.Columns[j].ColumnName == elements[i].Name)
                    {
                        row[j] = 0;
                    }
                    else
                    {
                        int linksCount = 0;
                        foreach ((Net net, List<int> _) in elements[i].Nets)
                        {
                            if (net.Elements.Find(el => el.Name == matrixR.Columns[j].ColumnName) != null)
                            {
                                linksCount++;
                            }
                        }
                        row[j] = linksCount;
                    }
                }
                matrixR.Rows.Add(row);
            }
            return matrixR;
        }
    }
}
