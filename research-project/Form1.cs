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
            g.DrawLine(Pens.Cyan, new Point(0, 0), new Point(300, 0));
            
            //Y-axis
            g.DrawLine(Pens.Cyan, new Point(0, 0), new Point(0, 300));
            
            var lesserScreenSize = Math.Min(this.ClientSize.Width, this.ClientSize.Height);

            Circle unitCircle = new Circle(lesserScreenSize / 2, (0, 0));
            //Draw the unit circle
            DrawCircle(g, Pens.Purple, new Circle(lesserScreenSize / 2, (0, 0)));

            List<(double, double)> inits = InitialVertices(100, 4);
            List<Circle> circles = new List<Circle>();
            //for each pair of adjacent points
            int j = 1;
            for (int i = 0; i < inits.Count; i++)
            {
                (double, double) inversion = InvertPoint(inits[i], unitCircle.centerPoint, unitCircle.r);
                Circle connectingCircle = CircleFromThreePoints(inversion, inits[i], inits[j]);
                circles.Add(connectingCircle);
                j++;
                if (j == inits.Count)
                {
                    j = 0;
                }
            }

            //Draw the initial tile, based on the initial points
            //for each pair of adjacent points
            j = 1;
            for (int i = 0; i < inits.Count; i++)
            {
                var circleCentreX = circles[i].centerPoint.Item1;
                var circleCentreY = circles[i].centerPoint.Item2;
                var startX = inits[i].Item1;
                var startY = inits[i].Item2;
                var destX = inits[j].Item1;
                var destY = inits[j].Item2;
                //I'm going to just use radians counterclockwise for angles and convert in the end
                var startAngle = Math.Atan2(startY - circleCentreY, startX - circleCentreX);
                var destAngle = Math.Atan2(destY - circleCentreY, destX - circleCentreX);
                var diffAngle = destAngle - startAngle;
                var degreeStartAngle = (180/Math.PI) * startAngle;
                var degreeDiffAngle = (180 / Math.PI) * diffAngle;
                
                g.DrawArc(Pens.Red, circles[i].GetRectangle(), (float)degreeStartAngle, (float)degreeDiffAngle);
                
                j++;
                if (j == inits.Count)
                {
                    j = 0;
                }
            }

            // foreach (var circle in circles)
            // {
            //     DrawCircle(g, Pens.Red, circle);
            // }

        }

        private void DrawPoint(Graphics g, Point p)
        {
            g.FillRectangle(Brushes.Red, p.X, p.Y, 1, 1);
        }

        private void DrawCircle(Graphics g, Pen p, Circle c)
        {
            Rectangle boundingRectangle = c.GetRectangle();
            g.DrawEllipse(p, boundingRectangle);
        }

        /// <summary>
        /// Calculates the first p points a distance d from the origin
        /// The first point is always generated a distance at x=d, y=0
        /// </summary>
        /// <param name="d"></param>
        /// <returns>A list of initial points from the origin point (0, 0)</returns>
        public List<(double, double)> InitialVertices(int d, int p)
        {

            double angle = 2 * Math.PI / p;
            List<(double, double)> result = new List<(double, double)>();

            double curAngle = 0;
            for (int i = 0; i < p; i++)
            {
                double x = d * Math.Cos(curAngle);
                double y = d * Math.Sin(curAngle);
                curAngle += angle;
                result.Add((x, y));
            }
            return result;
        }

        public double Distance((double, double) p1, (double, double) p2)
        {
            var xDiff = p1.Item1 - p2.Item1;
            var yDiff = p1.Item2 - p2.Item2;
            return Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
        }
        
        //see https://www.malinc.se/noneuclidean/en/circleinversion.php
        //Takes a point inside a circle and returns a point outside of the circle
        public (double, double) InvertPoint((double, double) point, (double, double) center, double r)
        {
            
            double x = point.Item1;
            double y = point.Item2;
            double centerX = center.Item1;
            double centerY = center.Item2;
            
            double EPSILON = 0.000001;

            double centerToX = x - centerX;
            double centerToY = y - centerY;
            
            var hypot = Math.Sqrt(Math.Pow(centerToX, 2) + Math.Pow(centerToY, 2));

            if (Math.Abs(hypot) < EPSILON)
            {
                throw new ArithmeticException(
                    "Cannot invert a centerPoint of a circle (this would result in division by zero)");
            }
            
            var phi = Math.Atan2(centerToY, centerToX);

            var distCenterToInversion = Math.Pow(r, 2) / hypot;

            var invX = centerX + (distCenterToInversion * Math.Cos(phi));
            var invY = centerY + (distCenterToInversion * Math.Sin(phi));

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