using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IterativeArrangement.Models
{
    public class Net
    {
        public string Name { get; set; }
        public List<Element> Elements { get; } = new();
    }
}
