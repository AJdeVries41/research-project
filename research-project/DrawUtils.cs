using System;
using System.Drawing;

namespace research_project
{
    public class DrawUtils
    {
        public static void DrawPoint(Graphics g, Brush b, (double, double) p)
        {
            Point point = new Point((int) Math.Round(p.Item1), (int) Math.Round(p.Item2));
            Rectangle r = new Rectangle(point, new Size(3, 3));
            g.FillRectangle(b, r);
        }
    }
}