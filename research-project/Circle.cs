using System;
using System.Drawing;

namespace research_project
{
    public class Circle
    {
        public double r { get; }
        public (double, double) centerPoint { get; }

        public Circle(double r, (double, double) p)
        {
            this.r = r;
            this.centerPoint = p;
        }

        public Rectangle GetRectangle()
        {
            int r = (int) Math.Round(this.r);
            int cx = (int) Math.Round(this.centerPoint.Item1);
            int cy = (int) Math.Round(this.centerPoint.Item2);
            Point bottomLeft = new Point()
            {
                X=cx-r,
                Y=cy-r
            };
            Console.WriteLine("bottomLeft x: " + bottomLeft.X);
            Console.WriteLine("bottomLeft y: " + bottomLeft.Y);
            Size rectangleSize = new Size(2 * r, 2 * r);
            Rectangle boundingRectangle = new Rectangle(bottomLeft, rectangleSize);
            return boundingRectangle;
        }


    }
}