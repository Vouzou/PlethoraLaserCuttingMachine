using System.Collections.Generic;

namespace PlethoraModels
{
    public abstract class Edge
    {
        public int id { get; set; }
        public List<int> Vertices { get; set; }
        public string Type { get; set; }
    }

    public class LineSegment : Edge
    {
        public double Magnitude { get; set; }
        public Point Vector2D { get; set; }
    }

    public class CircularArc : Edge
    {
        public Point Center { get; set; }
        public int ClockwiseFrom { get; set; }
        public int ClockwiseTo { get; set; }
        public double Radius { get; set; }
        public double Length { get; set; }
    }
}
