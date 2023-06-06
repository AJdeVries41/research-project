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

           List<(double, double)> inits = t.InitialVertices();

           List<Circle> initialCircles = t.InitialCircles(inits);
           
           DrawInitialTile(g, inits, initialCircles);

        }

        private void DrawInitialTile(Graphics g, List<(double, double)> inits, List<Circle> circles)
        {
            //Draw the initial tile, based on the initial points
            //for each pair of adjacent points, draw an arc counterclockwise from inits[j] to inits[i]
            int j = 1;
            for (int i = 0; i < inits.Count; i++)
            {
                var circleCentreX = circles[i].centerPoint.Item1;
                var circleCentreY = circles[i].centerPoint.Item2;
                //we are drawing the circle counterclockwise from the 2nd point to the 1st point
                var startX = inits[j].Item1;
                var startY = inits[j].Item2;
                var destX = inits[i].Item1;
                var destY = inits[i].Item2;

                var startAngle = GeomUtils.AngleConverter(Math.Atan2(startY - circleCentreY, startX - circleCentreX));
                var destAngle = GeomUtils.AngleConverter(Math.Atan2(destY - circleCentreY, destX - circleCentreX));
                if (destAngle < startAngle)
                {
                    destAngle += 2 * Math.PI;
                }
                //we can now assume that destAngle is necessarily larger than startAngle
                var diffAngle = destAngle - startAngle;
                float degreeStartAngle = (float) ((180/Math.PI) * startAngle);
                float degreeDiffAngle = (float) ((180 / Math.PI) * diffAngle);

                g.DrawArc(Pens.Red, circles[i].GetRectangle(), degreeStartAngle, degreeDiffAngle);
                
                j++;
                if (j == inits.Count)
                {
                    j = 0;
                }
            }
        }
        
        private void DrawPoint(Graphics g, Point p)
        {
            g.FillRectangle(Brushes.Red, p.X, p.Y, 1, 1);
        }
        
        
    }
}