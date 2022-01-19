using System;

namespace IterativeArrangement.Models
{
    public class Permutation : IComparable
    {
        public Permutation((Element, Element) elements, int total) =>
            (Elements, Total) = (elements, total);

        public (Element First, Element Second) Elements { get; set; }
        public int Total { get; set; }

        public int CompareTo(object obj)
        {
            return Total.CompareTo((obj as Permutation).Total);
        }

        public void Perform()
        {
            int temp = Elements.First.Group;
            Elements.First.Group = Elements.Second.Group;
            Elements.Second.Group = temp;
        }
    }
}
