using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms.VisualStyles;

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
        public List<Tile> knownTiles;
        

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

            this.knownTiles = new List<Tile>();

        }

        public String TileSideToString(TileSide s)
        {
            return s.ToString().Substring(0, 1);
        }

        public String StepToString(Step s)
        {
            return s.ToString().Substring(0, 1);
        }

        //Generates a tiling by continuously adding new Tile objects to the tile list
        public void GenerateTiling()
        {
            TileSide[] sides = { TileSide.North, TileSide.West, TileSide.South, TileSide.East };
            Step[] steps = { Step.Forward, Step.Left, Step.Right };
            int NUM_ITERATIONS = 200;
            int iterationCount = 0;
            
            //First generate the initial tile
            Tile initial = new Tile(this.initialPoints, this.initialCircles);
            this.knownTiles.Add(initial);
            Queue<Tile> q = new Queue<Tile>();
            q.Enqueue(initial);
            
            //Then generate the first layer of new tiles, which is unique because we generate in all 4 directions
            if (this.p == 4 && this.q == 5)
            {
                initial = q.Dequeue();
                foreach (var side in sides)
                {
                    String newPath = TileSideToString(side);
                    Tile reflectedTile = initial.ReflectIntoSide(side, newPath);
                    reflectedTile.path = TileSideToString(side);
                    this.knownTiles.Add(reflectedTile);
                    q.Enqueue(reflectedTile);
                }
                //Now we somehow have to generate only Forward, Left and Right tiles iff that is allowed according to the underlying graph

                while (q.Count != 0 && iterationCount < NUM_ITERATIONS)
                {
                    Tile current = q.Dequeue();
                    TileSide currentForwardDir = current.currentForwardDirection;
                    //Reflect the tile into each of the "Steps" which are currently allowed
                    foreach (var step in steps)
                    {
                        if (!current.isStepLegal(step))
                        {
                            continue;
                        }
                        TileSide reflectIn = StepToTileSide(step, currentForwardDir);
                        String newPath = current.path + StepToString(step);
                        Tile reflectedTile = current.ReflectIntoSide(reflectIn, newPath);
                        this.knownTiles.Add(reflectedTile);
                        q.Enqueue(reflectedTile);
                    }
                    iterationCount++;
                }
            }
            else
            {
                while (q.Count != 0 && iterationCount < NUM_ITERATIONS)
                {
                    Tile current = q.Dequeue();
                    for (int i = 0; i < current.edges.Length; i++)
                    {
                        Tile reflectedTile = current.ReflectIntoEdge(current.edges[i]);
                        if (!this.knownTiles.Contains(reflectedTile))
                        {
                            this.knownTiles.Add(reflectedTile);
                            q.Enqueue(reflectedTile);
                        }
                    }
                    iterationCount++;
                }
            }
        }
        
        public static TileSide StepToTileSide(Step s, TileSide currentForwardDirection)
        {
            switch (currentForwardDirection)
            {
                case TileSide.North:
                    switch (s)
                    {
                        case Step.Forward:
                            return TileSide.North;
                        case Step.Left:
                            return TileSide.West;
                        case Step.Right:
                            return TileSide.East;
                    }
                    break;
                case TileSide.West:
                    switch (s)
                    {
                        case Step.Forward:
                            return TileSide.West;
                        case Step.Left:
                            return TileSide.South;
                        case Step.Right:
                            return TileSide.North;
                    }

                    break;
                case TileSide.South:
                    switch (s)
                    {
                        case Step.Forward:
                            return TileSide.South;
                        case Step.Left:
                            return TileSide.East;
                        case Step.Right:
                            return TileSide.West;
                    }
                    break;
                case TileSide.East:
                    switch (s)
                    {
                        case Step.Forward:
                            return TileSide.East;
                        case Step.Left:
                            return TileSide.North;
                        case Step.Right:
                            return TileSide.South;
                    }
                    break;
            }
            //this never happens
            return TileSide.East;
        }
        
        public void DrawTiling(Graphics g)
        {
            //Draw unit circle
            g.DrawEllipse(Pens.Red, this.unitCircle.GetRectangle());
            //Draw each known tile
            foreach (var tile in this.knownTiles)
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