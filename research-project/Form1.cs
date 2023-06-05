using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace research_project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs eventArgs)
        {
            Graphics g = eventArgs.Graphics;
            
            //sets the origin to the middle of the screen which makes it a lot easier to draw stuff
            g.TranslateTransform(this.ClientSize.Width / 2, this.ClientSize.Height / 2);
            //Flips the Y-axis such that we can use normal Euclidean coordinates
            g.ScaleTransform(1.0F, -1.0F);
            
            //IMPORTANT
            //previously, any shapes with `drawXYZ` were drawn with topLeft point.
            //however, now we have to use the bottomLeft point because we flipped every point across the 
            //y-axis.
            //It still kinda makes sense tho, since we still use the point that is "closest" to the origin in all cases.
            
            //X-axis
            g.DrawLine(Pens.Cyan, new Point(0, 0), new Point(50, 0));
            
            //Y-axis
            g.DrawLine(Pens.Cyan, new Point(0, 0), new Point(0, 50));

            Circle unitCircle = new Circle(200, (0, 0));
            
            
            //Draw the unit circle
            DrawCircle(g, Pens.Purple, 200, new Point(0, 0));

            List<(double, double)> inits = InitialVertices(50, 4);
            
            Console.WriteLine(inits.Count);

            foreach (var pair in inits)
            {
                //Only convert to "Point" when we actually need to draw it, because you lose
                //precision when you do that
                Point p = new Point((int)Math.Round(pair.Item1), (int)Math.Round(pair.Item2));
                DrawPoint(g, p);
            }

            List<Circle> circles = new List<Circle>();
            
            //for each pair of adjacent points
            for (int i = 0; i < inits.Count - 1; i++)
            {
                Console.WriteLine("i: " + i);
                (double, double) inversion = InvertPoint(inits[i], unitCircle.centerPoint, unitCircle.r);
                double a = inversion.Item1;
                double b = inversion.Item2;
                double c = inits[i].Item1;
                double d = inits[i].Item2;
                double e = inits[i + 1].Item1;
                double f = inits[i + 1].Item2;
                Circle circle = CircleFromThreePoints(inversion, inits[i], inits[i + 1]);
                circles.Add(circle);
            }

            (double, double) lastInversion =
                InvertPoint(inits[inits.Count - 1], unitCircle.centerPoint, unitCircle.r);
            Circle lastCircle = CircleFromThreePoints(lastInversion, inits[inits.Count - 1], inits[0]);
            circles.Add(lastCircle);

            Console.WriteLine("Circles.size: " + circles.Count);

            foreach (var circle in circles)
            {
                DrawCircle(g, Pens.Red, circle);
            }
            
            
            



        }

        // <summary>
        // Takes a point that is in normal Euclidean space w/ origin (0, 0)
        // and converts it to a point in the screen space (w/ origin (res.x/2, res.y/2))
        //</summary>
        public Point ConvertToScreenSpace((double, double) point)
        {
            int resX = this.ClientSize.Width;
            int resY = this.ClientSize.Height;

            int p1 = (int)Math.Round(point.Item1);
            int p2 = (int)Math.Round(point.Item2);

            return new Point(resX + p1, resY + p2);
        }

        private void DrawPoint(Graphics g, Point p)
        {
            g.FillRectangle(Brushes.Red, p.X, p.Y, 1, 1);
        }

        private void DrawCircle(Graphics g, Pen p, Circle c)
        {
            int r = (int) Math.Round(c.r);
            int x = (int) Math.Round(c.centerPoint.Item1);
            int y = (int) Math.Round(c.centerPoint.Item2);
            DrawCircle(g, p, r, new Point(x, y));
        }

        private void DrawCircle(Graphics g, Pen p, int r, Point centerPoint)
        {
            Point bottomLeft = new Point()
            {
                X=centerPoint.X-r,
                Y=centerPoint.Y-r
            };
            Console.WriteLine("bottomLeft x: " + bottomLeft.X);
            Console.WriteLine("bottomLeft y: " + bottomLeft.Y);
            Size rectangleSize = new Size(2 * r, 2 * r);
            Rectangle boundingRectangle = new Rectangle(bottomLeft, rectangleSize);
            g.DrawEllipse(p, boundingRectangle);
        }
        
        /// <summary>
        /// Draw the first p points a distance d from the origin
        /// </summary>
        /// <param name="d"></param>
        /// <returns>A list of initial points from the origin point (0, 0)</returns>
        public List<(double, double)> InitialVertices(int d, int p)
        {

            double angle = 2 * Math.PI / p;
            List<(double, double)> result = new List<(double, double)>();

            double curAngle = angle;
            for (int i = 0; i < p; i++)
            {
                double x = d * Math.Cos(curAngle);
                double y = d * Math.Sin(curAngle);
                curAngle += angle;
                result.Add((x, y));
            }
            return result;
        }
        
        //see https://www.malinc.se/noneuclidean/en/circleinversion.php
        //Takes a point inside a circle and returns a point outside of the circle
        public (double, double) InvertPoint((double, double) point, (double, double) center, double r)
        {
            double x = point.Item1;
            double y = point.Item2;
            double centerX = center.Item1;
            double centerY = center.Item2;
            
            double originToX = x - centerX;
            double originToY = y - centerY;

            var phi = Math.Atan2(originToY, originToX);
            var hypot = Math.Sqrt(Math.Pow(originToX, 2) + Math.Pow(originToX, 2));
            
            var originToInversion = Math.Pow(r, 2) / hypot;

            var invX = centerX + (originToInversion * Math.Cos(phi));
            var invY = centerY + (originToInversion * Math.Sin(phi));

            return (invX, invY);
        }


        public Circle CircleFromThreePoints((double, double) p1, (double, double) p2, (double, double) p3)
        {
            double a = p1.Item1;
            double b = p1.Item2;
            double c = p2.Item1;
            double d = p2.Item2;
            double e = p3.Item1;
            double f = p3.Item2;
            
            
            double a1 = -2 * c + 2 * a;
            double b1 = -2 * d + 2 * b;
            double c1 = Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2) - Math.Pow(d, 2);

            double a2 = -2 * e + 2 * a;
            double b2 = -2 * f + 2 * b;
            double c2 = Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(e, 2) - Math.Pow(f, 2);

            //Solve system of equations using Cramer's rule

            //assert a1b2 - b1a2 != 0

            if (a1 * b2 - b1 * a2 == 0)
            {
                throw new SystemException("a1*b2 - b2*a2 = 0. This would return in division by zero");
            }

            double centerX = (c1 * b2 - b1 * c2) / (a1 * b2 - b1 * a2);
            double centerY = (a1 * c2 - c1 * a2) / (a1 * b2 - b1 * a2);

            //calculate r using any equation, e.g. the first
            double r = Math.Sqrt(Math.Pow(a - centerX, 2) + Math.Pow(b - centerY, 2));

            return new Circle(r, (centerX, centerY));
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
        }
        
        //This is to be able to use the console while running the application
        //(in order to print values to debug)
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}