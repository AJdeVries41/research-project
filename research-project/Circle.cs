using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Xml.XPath;

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

        /// <summary>
        /// Computes the bounding rectangle of this circle, i.e. the rectangle such that the courcles touches
        /// (and does not cross) this rectangle
        /// </summary>
        /// <returns></returns>
        public Rectangle GetRectangle()
        {
            int r = (int) Math.Round(this.r);
            int cx = (int) Math.Round(this.CenterPoint.Item1);
            int cy = (int) Math.Round(this.CenterPoint.Item2);
            Point bottomLeft = new Point(cx - r, cy - r);
            Size rectangleSize = new Size(2 * r, 2 * r);
            Rectangle boundingRectangle = new Rectangle(bottomLeft, rectangleSize);
            return boundingRectangle;
        }

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

        // public List<(double, double)> Intersect(Circle other)
        // {
        //     //We only care about the intersections between circles iff there's exactly 2
        //     double dist = GeomUtils.Distance(this.CenterPoint, other.CenterPoint);
        //     if (this.r - other.r < dist && dist < this.r + other.r)
        //     {
        //         double cosAlpha = (Math.Pow(this.r, 2) + Math.Pow(dist, 2) - Math.Pow(other.r, 2)) / (2 * this.r * dist);
        //         double alpha = Math.Acos(cosAlpha);
        //         double d1 = this.r * cosAlpha;
        //         double h = Math.Sqrt(Math.Pow(this.r, 2) - Math.Pow(d1, 2));
        //         double yDiff = other.CenterPoint.Item2 - this.CenterPoint.Item2;
        //         double xDiff = other.CenterPoint.Item1 - this.CenterPoint.Item1;
        //         double theta = Math.Atan2(yDiff, xDiff);
        //         (double, double) i1 = GeomUtils.ConvertFromPolar(this, theta + alpha);
        //         (double, double) i2 = GeomUtils.ConvertFromPolar(this, -(theta + alpha));
        //         return new List<(double, double)> { i1, i2 };
        //     }
        //
        //     return new List<(double, double)>();
        // }

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