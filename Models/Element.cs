using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IterativeArrangement.Models
{
    public class Element
    {
        public string Name { get; set; }
        public List<(Net Net, int Pin)> Nets { get; } = new();
    }
}
