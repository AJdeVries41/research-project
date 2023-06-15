using System;
using System.Drawing;

namespace research_project
{
    public class Line
    {
        public double slope;
        public double intercept;
        
        public Line(double slope, double intercept)
        {
            this.slope = slope;
            this.intercept = intercept;
        }

        public (double, double) Intersect(Line other)
        {
            var leftSide = this.slope - other.slope;
            var rightSide = other.intercept - this.intercept;
            var x = rightSide / leftSide;
            var y = this.slope * x + this.intercept;
            return (x, y);
        }

        public void Draw(Graphics g)
        {
            double x1 = -2000;
            double x2 = 2000;
            var y1 = this.slope * x1 + this.intercept;
            var y2 = this.slope * x2 + this.intercept;
            Point p1 = new Point((int) Math.Round(x1), (int) Math.Round(y1));
            Point p2 = new Point((int) Math.Round(x2), (int) Math.Round(y2));
            g.DrawLine(Pens.Black, p1, p2);
        }
    }
}