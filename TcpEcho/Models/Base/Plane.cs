using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Plane
    {
        public string Name { get; set; }
        public string ModelType { get; set; }
        public string Driver { get; set; }
        public int Seat { get; set; }
        public decimal Value { get; set; }
        public double TopSpeed { get; set; }
        public double Weight { get; set; }
        public double Capacity { get; set; }
    }
}
