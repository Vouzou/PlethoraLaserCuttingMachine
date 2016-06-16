using System;
using PlethoraModels;

namespace ImportJsonApplication.Utilities
{
    public class GeometryUtilities
    {
        /// <summary>
        /// Constructs a Vector from 2 Vertices
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <returns></returns>
        public static Point ConstructVector(Vertex vertex1, Vertex vertex2)
        {
            var point = new Point
            {
                X = vertex2.Position.X - vertex1.Position.X,
                Y = vertex2.Position.Y - vertex1.Position.Y
            };
            return point;
        }

        /// <summary>
        /// Returns the magnitude of a vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static double GetVectorMagnitude(Point vector)
        {
            return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }

        /// <summary>
        /// Return the angle between 2 line segments
        /// </summary>
        /// <param name="lineSegment1"></param>
        /// <param name="lineSegment2"></param>
        /// <returns></returns>
        public static double GetAngleBetweenLineSegments(LineSegment lineSegment1, LineSegment lineSegment2)
        {
            var dotProduct = lineSegment1.Vector2D.X*lineSegment2.Vector2D.X +
                             lineSegment1.Vector2D.Y*lineSegment2.Vector2D.Y;
            return Math.Acos(dotProduct/(lineSegment1.Magnitude*lineSegment2.Magnitude));
        }

        /// <summary>
        /// Returns the radius of a circular arc
        /// It only works for semi-circle
        /// </summary>
        /// <param name="center"></param>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public static double GetRadiusFromCircularArc(Point center, Vertex vertex)
        {
            var vector = new Point
            {
                X = vertex.Position.X - center.X,
                Y = vertex.Position.Y - center.Y
            };
            return GetVectorMagnitude(vector);
        }

        /// <summary>
        /// Returns the perimeter of a circle given the radius and two points that lie on the circle
        /// It's based on the law of cosines
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="fromVertex"></param>
        /// <param name="toVertex"></param>
        /// <returns></returns>
        public static double GetPerimeterFromCircularArc(double radius, Vertex fromVertex, Vertex toVertex)
        {
            var vector = new Point
            {
                X = fromVertex.Position.X - toVertex.Position.X,
                Y = fromVertex.Position.Y - toVertex.Position.Y
            };
            var distance = GetVectorMagnitude(vector);
            //var cosTheta = 1 - Math.Pow(distance, 2)/2*Math.Pow(radius, 2);
            //var theta = Math.Acos(cosTheta);
            var theta = 2*Math.Asin(distance/(2*radius));
            var arcLength = theta*radius;
            return arcLength;
        }

        /// <summary>
        /// Returns the area of a semi-circle
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static double GetAreaFromCircularArc(double radius)
        {
            return Math.PI*Math.Pow(radius, 2)/2;
        }
        
        /// <summary>
        /// Returns the midpoint between two points
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Point FindMidpoint(Point point1, Point point2)
        {
            var midpoint = new Point
            {
                X = (point1.X + point2.X)/2,
                Y = (point1.Y + point2.Y)/2
            };
            return midpoint;
        }

        /// <summary>
        /// Find the intersection between the circle and the line that connects the center of the circle to the midpoint of the chord
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="fromVertex"></param>
        /// <param name="toVertex"></param>
        /// <returns></returns>
        public static Point FindCircleIntersection(Point center, double radius, Vertex fromVertex, Vertex toVertex)
        {
            var midpoint = FindMidpoint(fromVertex.Position, toVertex.Position);
            var intersection = new Point();
            var phi = Math.Atan2(midpoint.Y - center.Y, midpoint.X - center.X);
            if (fromVertex.Position.X - toVertex.Position.X < 0 || fromVertex.Position.Y - toVertex.Position.Y < 0)
            {
                phi = Math.PI - phi;
            }
            intersection.X = center.X + radius*Math.Cos(phi);
            intersection.Y = center.Y + radius*Math.Sin(phi);
            return intersection;
        }

        /// <summary>
        /// Returns the area of a regular rectangular shape with edges a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double GetAreaRectangularArea(double a, double b)
        {
            return a*b;
        }

    }
}
