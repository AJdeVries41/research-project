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
            //AllocConsole();
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
            
            Tiling t = new Tiling(4, 5, lesserScreenSize, Math.PI/4);
            
            t.GenerateTiling();;

            Tile originTile = t.knownTiles[0];

            // for (int i = 0; i < 1000; i++)
            // {
            //     originTile.Draw(g);
            // }

            t.DrawTiling(g);

        }

        private void DrawPoint(Graphics g, Color c, (double, double) point)
        {
            Point p = new Point((int)Math.Round(point.Item1), (int)Math.Round(point.Item2));
            g.FillRectangle(new SolidBrush(c), p.X, p.Y, 5, 5);
        }
        
        
    }
}