using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace research_project
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
            //To make sure that the entire image is redrawn whenever the window is resized
            this.ResizeRedraw = true;
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

            var lesserScreenSize = Math.Min(this.ClientSize.Width, this.ClientSize.Height);

            Tiling t = new HolonomyTiling(lesserScreenSize, Math.PI / 4);
            
            
            t.GenerateTiling();
            t.FillTiling(g);
            //t.DrawTiling(g);
            //DrawingLab(g, t);
        }

        public void DrawingLab(Graphics g, Tiling t)
        {
            var B = (0, 100);
            DrawUtils.DrawPoint(g, Brushes.Orange, B);
            g.DrawEllipse(Pens.Red, t.UnitCircle.GetRectangle());
            //DrawUtils.DrawPoint(g, Brushes.Orange, A);
            //DrawUtils.DrawPoint(g, Brushes.Orange, B);
            var res = GeomUtils.HyperbolicBisectorFromCenter2(B, t.UnitCircle, g);
            g.DrawEllipse(Pens.Aquamarine, res.GetRectangle());
            // g.DrawEllipse(Pens.Red, t.UnitCircle.GetRectangle());
            // (double, double) origin = (30, 30);
            // (double, double) B = (100, 100);
            // t.MoveInitialTile(B, g);

            // var c1 = new Circle(330, (-275, 110));
            // var c2 = new Circle(100, (-20, 50));
            //
            // g.DrawEllipse(Pens.Red, c1.GetRectangle());
            // g.DrawEllipse(Pens.Red, c2.GetRectangle());
            //
            // var intersections = c1.Intersect(c2);
            // foreach (var point in intersections)
            // {
            //     DrawPoint(g, Brushes.Black, point);
            // }
        }

        
    }
}