using System;
using System.Collections.Generic;
using System.Drawing;

namespace research_project
{
    public class Tiling
    {
        private int P { get; set; }
        private int Q { get; set; }
        private Circle UnitCircle { get; set; }
        protected List<(double, double)> InitialPoints;
        protected List<Circle> InitialCircles;

        protected List<Tile> KnownTiles;
        

        // Saves all relevant info for the tiling
        public Tiling(int p, int q, int smallestResolution, double initialRotation)
        {
            if (!IsValidTiling(p, q))
            {
                throw new InvalidOperationException($"Invalid tiling of {{{p}, {q}}} given");
            }
            this.P = p;
            this.Q = q;
            this.UnitCircle = new Circle(smallestResolution / 2, (0, 0));
            int d = CalculateInitialDistance(smallestResolution);
            this.InitialPoints = this.CalculateInitialPoints(d, initialRotation);
            this.InitialCircles = this.CalculateInitialCircles();

            this.KnownTiles = new List<Tile>();

        }
        
        private static bool IsValidTiling(int p, int q)
        {
            return (p-2)*(q-2) > 4;
        }
        
        /// <summary>
        /// Calculates the first p points a distance d from the origin
        /// </summary>
        /// <param name="d"></param>
        /// <returns>A list of initial points from the origin point (0, 0)</returns>
        private List<(double, double)> CalculateInitialPoints(int d, double initialRotation)
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

        private List<Circle> CalculateInitialCircles()
        {
            List<Circle> circles = new List<Circle>();
            //for each pair of adjacent points
            int j = 1;
            for (int i = 0; i < this.InitialPoints.Count; i++)
            {
                (double, double) inversion = GeomUtils.InvertPoint(this.InitialPoints[i], UnitCircle);
                Circle connectingCircle = GeomUtils.CircleFromThreePoints(inversion, this.InitialPoints[i], this.InitialPoints[j]);
                circles.Add(connectingCircle);
                j++;
                if (j == this.InitialPoints.Count)
                {
                    j = 0;
                }
            }
            return circles;
        }

        //see https://www.malinc.se/noneuclidean/en/poincaretiling.php why this.d is calculated like this
        private int CalculateInitialDistance(int smallestResolution)
        {
            var numerator = Math.Tan((Math.PI / 2) - (Math.PI / Q)) - Math.Tan(Math.PI/P);
            var denominator = Math.Tan((Math.PI / 2) - (Math.PI / Q)) + Math.Tan(Math.PI / P);
            double d = Math.Sqrt(numerator / denominator);
            //The formula calculates d as a fraction of a unit circle of length 1, but we have a unit circle of length
            //"smallestResolution / 2" so we need to multiply by that
            int res = (int) Math.Round(d * (smallestResolution / 2));
            return res;
        }

        public virtual void GenerateTiling()
        {
            Queue<Tile> q = new Queue<Tile>();
            int NUM_ITERATIONS = 400;
            int iterationCount = 0;
            Tile initial = new Tile(this.InitialPoints, this.InitialCircles);
            this.KnownTiles.Add(initial);
            q.Enqueue(initial);
            while (q.Count != 0 && iterationCount < NUM_ITERATIONS)
            {
                Tile current = q.Dequeue();
                for (int i = 0; i < current.Edges.Length; i++)
                {
                    Tile reflectedTile = current.ReflectIntoEdge(current.Edges[i].c);
                    if (!this.KnownTiles.Contains(reflectedTile))
                    {
                        this.KnownTiles.Add(reflectedTile);
                        q.Enqueue(reflectedTile);
                    }
                }
                iterationCount++;
            }
        }

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