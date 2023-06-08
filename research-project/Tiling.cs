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
        public List<Tile> tiles;
        

        // Saves all relevant info for the tiling
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

            this.tiles = new List<Tile>();

        }

        //Generates a tiling by continuously adding new Tile objects to the tile list
        public void GenerateTiling()
        {
            //First generate the initial tile
            Tile initial = new Tile(this.initialPoints, this.initialCircles);
            this.tiles.Add(initial);

            Queue<Tile> q = new Queue<Tile>();
            
            q.Enqueue(initial);

            int NUM_ITERATIONS = 40;
            int iterationCount = 0;

            while (q.Count != 0 && iterationCount < NUM_ITERATIONS)
            {
                Tile current = q.Dequeue();
                //Reflect the current tile into each of its edges
                for (int i = 0; i < current.edges.Count; i++)
                {
                    Tile reflectedTile = current.ReflectIntoEdge(i);
                    q.Enqueue(reflectedTile);
                    this.tiles.Add(reflectedTile);
                }
                iterationCount++;
            }
        }
        
        public void DrawTiling(Graphics g)
        {
            //Draw unit circle
            g.DrawEllipse(Pens.Red, this.unitCircle.GetRectangle());
            foreach (var tile in this.tiles)
            {
                tile.Draw(g);
            }
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