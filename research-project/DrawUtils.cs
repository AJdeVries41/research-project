using System;
using System.Drawing;

namespace research_project
{
    public class DrawUtils
    {
        /// <summary>
        /// Draw a point by constructing a filling a small rectangle
        /// Where the bottomleft of that rectangle is the given point
        /// </summary>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="p"></param>
        public static void DrawPoint(Graphics g, Brush b, (double, double) p)
        {
            Point point = new Point((int) Math.Round(p.Item1), (int) Math.Round(p.Item2));
            Rectangle r = new Rectangle(point, new Size(3, 3));
            g.FillRectangle(b, r);
        }
    }
}