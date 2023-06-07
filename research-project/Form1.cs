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

namespace research_project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
            //Also, now g.DrawArc draws arcs counterclockwise
            
            //X-axis
            g.DrawLine(Pens.Cyan, new Point(0, 0), new Point(300, 0));
            
            //Y-axis
            g.DrawLine(Pens.Cyan, new Point(0, 0), new Point(0, 300));
            
            var lesserScreenSize = Math.Min(this.ClientSize.Width, this.ClientSize.Height);
            
            Tiling t = new Tiling(4, 5, lesserScreenSize);
            
            //Draw the unit circle
            g.DrawEllipse(Pens.Purple, t.unitCircle.GetRectangle());

            //To demonstrate, this draws the unitCircle from 0 degrees to 270 degrees counterclockwise
           // g.DrawArc(Pens.Yellow, unitCircle.GetRectangle(), 0F, 270F);

           List<(double, double)> inits = t.InitialVertices(Math.PI/4);

           List<Circle> initialCircles = t.InitialCircles(inits);

           Tile initialTile = new Tile(inits, initialCircles);

           DrawTile(g, initialTile);
           //DrawTileCircles(g, initialTile);
           
           
           
           //example time
           //reflect the bottom edge into the top edge
           //  Circle topEdge = initialTile.circles[0];
           //  Circle bottomEdge = initialTile.circles[2];
           //
           //
           //  (double, double) reflPoint1 = GeomUtils.InvertPoint(initialTile.points[2], topEdge);
           //  (double, double) reflPoint2 = GeomUtils.InvertPoint(initialTile.points[3], topEdge);
           //  (double, double) newCenter = GeomUtils.InvertPoint(bottomEdge.centerPoint, topEdge);
           //  double newR = GeomUtils.Distance(newCenter, reflPoint1);
           //  var newCircle = new Circle(newR, newCenter);
           //
           //  DrawPoint(g, Color.Aquamarine, reflPoint1);
           //  DrawPoint(g, Color.Aquamarine, reflPoint2);
           //  
           //  g.DrawEllipse(Pens.Aquamarine, newCircle.GetRectangle());
           //
           //
           //
           // //reflect the left edge into the top edge (this doesn't work)
           //
           // int leftEdge = 1;
           //
           // double r = initialTile.circles[leftEdge].r;
           // (double, double) center = initialTile.circles[leftEdge].centerPoint;
           //
           // var startAngle = initialTile.angles[leftEdge].Item1;
           // var diffAngle = initialTile.angles[leftEdge].Item3;
           //
           // var midAngleDegrees = startAngle + (diffAngle / 2);
           //
           // var midAngleRad = (Math.PI / 180) * midAngleDegrees;
           //
           // var midPoint = GeomUtils.AddPoints(center, (r * Math.Cos(midAngleRad), r * Math.Sin(midAngleRad)));
           //
           // var invertMidPoint = GeomUtils.InvertPoint(midPoint, topEdge);
           //
           // DrawPoint(g, Color.Aquamarine, invertMidPoint);
           //
           // //check if midpoints is correct
           //
           // //the 3 relevant points
           //
           // DrawPoint(g, Color.Aquamarine, reflPoint1);
           // DrawPoint(g, Color.Aquamarine, invertMidPoint);
           // DrawPoint(g, Color.Aquamarine, initialTile.points[1]);
           //
           // Circle resultingCircle = GeomUtils.CircleFromThreePoints(reflPoint1, invertMidPoint, initialTile.points[1]);
           // g.DrawEllipse(Pens.Aquamarine, resultingCircle.GetRectangle());
           
           //reflect left edge into top edge

           Circle result1 = initialTile.ReflectIntoEdge(1, 0);
           
           g.DrawEllipse(Pens.Aquamarine, result1.GetRectangle());

           Circle result2 = initialTile.ReflectIntoEdge(2, 0);
           Circle result3 = initialTile.ReflectIntoEdge(3, 0);
           
           g.DrawEllipse(Pens.Aquamarine, result2.GetRectangle());
           g.DrawEllipse(Pens.Aquamarine, result3.GetRectangle());
           
           //for each edge, reflect into this edge
           for (int i = 0; i < t.p; i++)
           {
               var cs = new List<Circle>();
               for (int j = 0; j < t.p; j++)
               {
                   if (i == j)
                   {
                       continue;
                   }

                   Circle result = initialTile.ReflectIntoEdge(j, i);
                   cs.Add(result);
               }

               foreach (var c in cs)
               {
                   g.DrawEllipse(Pens.Aquamarine, c.GetRectangle());
               }
           }



        }

        private void DrawTile(Graphics g, Tile t)
        {
            //any edge of the tile is represented by its circles and its angles how it should be drawn
            for (int i = 0; i < t.circles.Count; i++)
            {
                float startAngle = t.angles[i].Item1;
                float diffAngle = t.angles[i].Item3;
                g.DrawArc(Pens.Red, t.circles[i].GetRectangle(), startAngle, diffAngle);
            }
        }

        private void DrawTileCircles(Graphics g, Tile t)
        {
            for (int i = 0; i < t.circles.Count; i++)
            {
                g.DrawEllipse(Pens.Red, t.circles[i].GetRectangle());
            }
        }

        private void DrawPoint(Graphics g, Color c, (double, double) point)
        {
            Point p = new Point((int)Math.Round(point.Item1), (int)Math.Round(point.Item2));
            g.FillRectangle(new SolidBrush(c), p.X, p.Y, 5, 5);
        }
        
        
    }
}