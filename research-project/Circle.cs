using System;
using System.Collections.Generic;
using System.Drawing;

namespace research_project
{
    public class Circle
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
            int intBottomLeftX = (int)Math.Round(bottomLeftX);
            int intBottomLeftY = (int)Math.Round(bottomLeftY);
            int rectangleEdgeLength = (int)Math.Round(2 * this.r);
            Rectangle boundingRectangle = new Rectangle(intBottomLeftX, intBottomLeftY, rectangleEdgeLength, rectangleEdgeLength);
            return boundingRectangle;
        }

        /// <summary>
        /// Checks the 2 intersection points of this circle with another circle
        /// Returns an empty list if the two circles do not intersect at two points
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public (double, double)[] Intersect(Circle other)
        {
            var result = new (double, double)[2];
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
                result[0] = (x1, y1);
                result[1] = (x2, y2);
            }
            return result;
        }
    }
}