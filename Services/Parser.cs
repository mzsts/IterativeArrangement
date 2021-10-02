using System.Collections.Generic;
using System.Linq;
using IterativeArrangement.Models;

namespace IterativeArrangement.Services
{
    public static class Parser
    {
        public static (List<Element> Elements, List<Net> Nets) ParseToObjects(string fileData)
        {
            List<Net> nets = new();
            List<Element> elements = new();

            List<string> lines = ParseToStrings(fileData);

            foreach (string item in lines)
            {
                List<string> els = item.Trim().Split(' ').ToList();
                Net net = new() { Name = els[0] };
                Element el;
                for (int i = 1; i < els.Count; i++)
                {
                    string[] tempS = els[i].Split('\'');
                    (string name, int pin) = (tempS[0], int.Parse(tempS[1]));

                    if (elements.Find(item => item.Name == name) is Element element && element is not null)
                    {
                        if (element.Nets.Find(n => n.Net.Name == net.Name) is var currentNet && currentNet is not (null, _))
                        {
                            currentNet.Pins.Add(pin);
                        }
                        else
                        {
                            el = element;
                            el.Nets.Add((net, new() { pin }));
                            net.Elements.Add(el);
                        }
                    }
                    else
                    {
                        el = new() { Name = name };
                        el.Nets.Add((net, new() { pin }));
                        elements.Add(el);
                        net.Elements.Add(el);
                    }
                    nets.Add(net);
                }
            }

            return (elements, nets);
        }

        public static (List<Element> Elements, List<Net> Nets) ParseToObjects(List<string> fileData)
        {
            List<Net> nets = new();
            List<Element> elements = new();

            foreach (string item in fileData)
            {
                List<string> els = item.Trim().Split(' ').ToList();
                Net net = new() { Name = els[0] };
                Element el;
                for (int i = 1; i < els.Count; i++)
                {
                    string[] tempS = els[i].Split('\'');
                    (string name, int pin) = (tempS[0], int.Parse(tempS[1]));

                    if (elements.Find(item => item.Name == name) is Element element && element is not null)
                    {
                        if (element.Nets.Find(n => n.Net.Name == net.Name) is var currentNet && currentNet is not (null, _))
                        {
                            currentNet.Pins.Add(pin);
                        }
                        else
                        {
                            el = element;
                            el.Nets.Add((net, new() { pin }));
                            net.Elements.Add(el);
                        }
                    }
                    else
                    {
                        el = new() { Name = name };
                        el.Nets.Add((net, new() { pin }));
                        elements.Add(el);
                        net.Elements.Add(el);
                    }
                    nets.Add(net);
                }
            }

            return (elements, nets);
        }

        public static List<string> ParseToStrings(string data)
        {
            List<string> temp = data.Replace("\r\n", "").
                                  Replace(",", "").
                                  Replace("(", "").
                                  Replace(")", "").
                                  Split(";").
                                  ToList();

            List<string> lines = new();


            foreach (var item in temp)
            {
                string line = string.Empty;
                foreach (string el in item.Split(" "))
                {
                    if (string.IsNullOrEmpty(el.Trim()) is false)
                    {
                        line += el + " ";
                    }
                }
                lines.Add(line);
            }

            return lines;
        }
    }
}
