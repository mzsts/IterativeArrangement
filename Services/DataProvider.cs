using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IterativeArrangement.Services
{
    public static class DataProvider
    {
        public static string ReadFromFile(string path)
        {
            return File.ReadAllTextAsync(path).Result;
        }
    }
}
