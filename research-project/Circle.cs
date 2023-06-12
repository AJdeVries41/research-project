using System;
using System.Configuration;
using System.Drawing;

namespace research_project
{
    public class Circle : IEquatable<Circle>
    {
        public double r { get; }
        public (double, double) CenterPoint { get; }

        public Circle(double r, (double, double) p)
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
            Point bottomLeft = new Point()
            {
                X=cx-r,
                Y=cy-r
            };
            Size rectangleSize = new Size(2 * r, 2 * r);
            Rectangle boundingRectangle = new Rectangle(bottomLeft, rectangleSize);
            return boundingRectangle;
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