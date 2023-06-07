using System;
using System.Collections.Generic;
using System.Drawing;

namespace research_project
{
    public class Tiling
    {
        public int p;
        public int q;
        private int d;
        public double initialRotation;
        public Circle unitCircle;

        public List<(double, double)> initialPoints;
        public List<Circle> initialCircles;

        public Tile InitialTile;
        
        public List<Tile> tiles;
        

        // Saves all relevant info for the tiling and also builds the initial tile
        public Tiling(int p, int q, int smallestResolution, double initialRotation)
        {
            if (!IsValidTiling(p, q))
            {
                throw new InvalidOperationException($"Invalid tiling of {{{p}, {q}}} given");
            }
            this.p = p;
            this.q = q;
            this.initialRotation = initialRotation;
            this.unitCircle = new Circle(smallestResolution / 2, (0, 0));
            
            //see https://www.malinc.se/noneuclidean/en/poincaretiling.php why this.d is calculated like this
            var numerator = Math.Tan((Math.PI / 2) - (Math.PI / q)) - Math.Tan(Math.PI/p);
            var denominator = Math.Tan((Math.PI / 2) - (Math.PI / q)) + Math.Tan(Math.PI / p);
            double d = Math.Sqrt(numerator / denominator);
            this.d = (int) Math.Round(d * (smallestResolution / 2));

            this.initialPoints = this.InitialVertices();
            this.initialCircles = this.InitialCircles();
            this.InitialTile = new Tile(this.initialPoints, this.initialCircles);
            
            
            this.tiles = new List<Tile>();

        }

        //Generates a tiling by continuously adding new Tile objects to the tile list
        public void GenerateTiling(Graphics g)
        {
            g.DrawEllipse(Pens.Purple, this.unitCircle.GetRectangle());
            Tile current = InitialTile;
            current.Draw(g);
            for (int i = 0; i < current.edges.Count; i++)
            {
                //reflect all edges[j] into edges[i]
                for (int j = 0; j < current.edges.Count; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }

                    Geodesic reflection = Geodesic.ReflectIntoEdge(current.edges[j], current.edges[i]);
                    reflection.Draw(g);
                }
            }
        }
        
        public void DrawTiling(Graphics g)
        {
            //Draw the unit circle
            g.DrawEllipse(Pens.Purple, this.unitCircle.GetRectangle());
            InitialTile.Draw(g);
        }

        public static bool IsValidTiling(int p, int q)
        {
            return (p-2)*(q-2) > 4;
        }
        
        /// <summary>
        /// Calculates the first p points a distance d from the origin
        /// </summary>
        /// <param name="d"></param>
        /// <returns>A list of initial points from the origin point (0, 0)</returns>
        public List<(double, double)> InitialVertices()
        {

            double angle = 2 * Math.PI / p;
            List<(double, double)> result = new List<(double, double)>();

            double curAngle = 0 + this.initialRotation;
            for (int i = 0; i < p; i++)
            {
                double x = d * Math.Cos(curAngle);
                double y = d * Math.Sin(curAngle);
                curAngle += angle;
                result.Add((x, y));
            }
            return result;
        }

        public List<Circle> InitialCircles()
        {
            List<Circle> circles = new List<Circle>();
            //for each pair of adjacent points
            int j = 1;
            for (int i = 0; i < this.initialPoints.Count; i++)
            {
                (double, double) inversion = GeomUtils.InvertPoint(this.initialPoints[i], unitCircle);
                Circle connectingCircle = GeomUtils.CircleFromThreePoints(inversion, this.initialPoints[i], this.initialPoints[j]);
                circles.Add(connectingCircle);
                j++;
                if (j == this.initialPoints.Count)
                {
                    j = 0;
                }
            }

            return circles;
        }
    }
}