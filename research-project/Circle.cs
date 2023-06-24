using System;
using System.Collections.Generic;
using System.Drawing;

namespace research_project
{
    public class Circle : IEquatable<Circle>
    {
        public double r { get; }
        public (double, double) CenterPoint { get; }

        public Circle((double, double) p, double r)
        {
            this.r = r;
            this.CenterPoint = p;
        }
        
        // /// <summary>
        // /// Computes the bounding rectangle of this circle, i.e. the rectangle such that the circle touches
        // /// (and does not cross) this rectangle
        // /// </summary>
        // /// <returns></returns>
        public Rectangle GetRectangle()
        {
            double bottomLeftX = this.CenterPoint.Item1 - this.r;
            double bottomLeftY = this.CenterPoint.Item2 - this.r;
            int intBottomLeftX = Convert.ToInt32(bottomLeftX);
            int intBottomLeftY = Convert.ToInt32(bottomLeftY);
            int rectangleEdgeLength = Convert.ToInt32(2 * this.r);
            Point bottomLeft = new Point(intBottomLeftX, intBottomLeftY);
            Size rectangleSize = new Size(rectangleEdgeLength, rectangleEdgeLength);
            Rectangle boundingRectangle = new Rectangle(bottomLeft, rectangleSize);
            return boundingRectangle;
        }

        /// <summary>
        /// Checks the 2 intersection points of this circle with another circle
        /// Returns an empty list if the two circles do not intersect at two points
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public List<(double, double)> Intersect(Circle other)
        {
            var result = new List<(double, double)>();
            //We only care about circles that have 2 solutions
            double dist = GeomUtils.Distance(this.CenterPoint, other.CenterPoint);
            if (Math.Abs(this.r - other.r) < dist && dist < this.r + other.r)
            {
                double a = this.CenterPoint.Item1;
                double b = this.CenterPoint.Item2;
                double c = other.CenterPoint.Item1;
                double d = other.CenterPoint.Item2;
                double r1 = this.r;
                double r2 = other.r;
                
                //x = alpha*y+beta
                double alpha = (2 * b - 2 * d) / (-2 * a + 2 * c);
                double beta = ((r1*r1) - (r2*r2) - (a*a) + (c*c) - (b*b) + (d*d)) / (-2 * a + 2 * c);
                //Solve for y using the 2nd equation
                double v1 = (alpha * alpha + 1);
                double v2 = 2 * alpha * beta - 2 * c * alpha - 2 * d;
                double v3 = (beta * beta) - 2 * c * beta + (c * c) + (d * d) - (r2 * r2);
                var (y1, y2) = GeomUtils.SolveQuadraticEquation(v1, v2, v3);
                var x1 = alpha * y1 + beta;
                var x2 = alpha * y2 + beta;
                result.Add((x1, y1));
                result.Add((x2, y2));
                return result;
            }
            return new List<(double, double)>();
        }

        public bool Equals(Circle other)
        {
            return GeomUtils.NearlyEqual(this.r, other.r)
                   && GeomUtils.NearlyEqual(this.CenterPoint.Item1, other.CenterPoint.Item1)
                   && GeomUtils.NearlyEqual(this.CenterPoint.Item2, other.CenterPoint.Item2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Circle)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (r.GetHashCode() * 397) ^ CenterPoint.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{nameof(r)}: {r}, {nameof(CenterPoint)}: {CenterPoint}";
        }
    }
}