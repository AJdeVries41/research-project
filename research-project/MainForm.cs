using System;
using System.Drawing;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace research_project
{
    public partial class MainForm : Form
    {
        private (double, double) newOriginPoint = (0, 0);
        public MainForm()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
            //Maximizes the window on startup.
            this.WindowState = FormWindowState.Maximized;
            //To make sure that the entire image is redrawn whenever the window is resized
            this.ResizeRedraw = true;
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(MainForm_KeyDown);
        }
        
        //This is to be able to use the console while running the application
        //(in order to print values to debug)
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        protected override void OnPaint(PaintEventArgs eventArgs)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
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
            HolonomyTiling t = new HolonomyTiling(lesserScreenSize-10, Math.PI/4);

            if (!GeomUtils.NearlyEqual(GeomUtils.Distance(newOriginPoint, (0, 0)), 0))
            {
                //then move the origin to the newOriginPoint
                t.MoveInitialTile(this.newOriginPoint);
            }
            
            t.GenerateTiling(100);
            t.DrawColoredTiling(g);
            //t.DrawTiling(g, Color.Black, 3);
            
            stopwatch.Stop();
            Console.WriteLine($"Took {stopwatch.ElapsedMilliseconds} ms to generate and draw tiling");
            
        }


        protected void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                this.newOriginPoint.Item2 += 10.0;
            }
            else if (e.KeyCode == Keys.Down)
            {
                this.newOriginPoint.Item2 += -10.0;
            }
            else if (e.KeyCode == Keys.Right)
            {
                this.newOriginPoint.Item1 += 10.0;
            }
            else if (e.KeyCode == Keys.Left)
            {
                this.newOriginPoint.Item1 += -10.0;
            }
            this.Refresh();
        }


    }
}