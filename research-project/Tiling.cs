using System;
using System.Collections.Generic;
using System.Drawing;

namespace research_project
{
    public abstract class Tiling
    {
        protected int P { get; set; }
        protected int Q { get; set; }
        protected Circle UnitCircle { get; set; }
        
        protected Tile InitialTile;
        protected List<Tile> KnownTiles;

        public Tiling(int p, int q, int smallestResolution)
        {
            if (!IsValidTiling(p, q))
            {
                throw new InvalidOperationException($"Invalid tiling of {{{p}, {q}}} given");
            }
            this.P = p;
            this.Q = q;
            this.UnitCircle = new Circle(smallestResolution / 2, (0, 0));
            this.KnownTiles = new List<Tile>();
        }

        public static bool IsValidTiling(int p, int q)
        {
            return (p-2)*(q-2) > 4;
        }
        
        //see https://www.malinc.se/noneuclidean/en/poincaretiling.php why this.d is calculated like this
        protected int CalculateInitialDistance(int smallestResolution)
        {
            var numerator = Math.Tan((Math.PI / 2) - (Math.PI / Q)) - Math.Tan(Math.PI/P);
            var denominator = Math.Tan((Math.PI / 2) - (Math.PI / Q)) + Math.Tan(Math.PI / P);
            double d = Math.Sqrt(numerator / denominator);
            //The formula calculates d as a fraction of a unit circle of length 1, but we have a unit circle of length
            //"smallestResolution / 2" so we need to multiply by that
            int res = (int) Math.Round(d * (smallestResolution / 2));
            return res;
        }
        
        /// <summary>
        /// Calculates the first p points a distance d from the origin
        /// </summary>
        /// <param name="d"></param>
        /// <returns>A list of initial points from the origin point (0, 0)</returns>
        protected List<(double, double)> CalculateInitialPoints(int d, double initialRotation)
        {

            double angle = 2 * Math.PI / P;
            List<(double, double)> result = new List<(double, double)>();

            double curAngle = initialRotation;
            for (int i = 0; i < P; i++)
            {
                double x = d * Math.Cos(curAngle);
                double y = d * Math.Sin(curAngle);
                curAngle += angle;
                result.Add((x, y));
            }
            return result;
        }

        protected List<Circle> CalculateInitialCircles(List<(double, double)> initialPoints)
        {
            List<Circle> circles = new List<Circle>();
            //for each pair of adjacent points
            int j = 1;
            for (int i = 0; i < initialPoints.Count; i++)
            {
                (double, double) inversion = GeomUtils.InvertPoint(initialPoints[i], UnitCircle);
                Circle connectingCircle = GeomUtils.CircleFromThreePoints(inversion, initialPoints[i], initialPoints[j]);
                circles.Add(connectingCircle);
                j++;
                if (j == initialPoints.Count)
                {
                    j = 0;
                }
            }
            return circles;
        }
        
        protected void MoveInitialTile((double, double) B)
        {
            //To move the initial tile to the given point, we construct a hyperbolic bisector between
            //the origin and the given point. Then, we invert the entire initial tile within that bisector
            
        }


        public abstract void GenerateTiling();
        
        public void DrawTiling(Graphics g)
        {
            //Draw unit circle
            g.DrawEllipse(Pens.Red, this.UnitCircle.GetRectangle());
            //Draw each known tile
            foreach (var tile in this.KnownTiles)
            {
                tile.DrawBounds(g);
            }
        }

        public void FillTiling(Graphics g)
        {
            //Draw unit circle
            g.DrawEllipse(Pens.Red, this.UnitCircle.GetRectangle());
            foreach (var tile in this.KnownTiles)
            {
                tile.FillTile(g);
            }
        }
    }
    
    
}