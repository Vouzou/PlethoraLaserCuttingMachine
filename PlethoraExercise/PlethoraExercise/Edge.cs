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
        
    }

    public class CircularArc : Edge
    {
        public Point Center { get; set; }
        public int ClockwiseFrom { get; set; }
    }
}
