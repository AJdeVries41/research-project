using System;
using System.Collections.Generic;
using System.Drawing;

namespace research_project
{
    public class Tiling
    {
        public int p { get; set; }
        public int q { get; set; }
        private int d { get; set; }
        public double initialRotation { get; set; }
        public Circle unitCircle { get; set; }

        public List<(double, double)> initialPoints;
        public List<Circle> initialCircles;
        public List<GenericTile> knownTiles;
        

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
            this.CalculateInitialDistance(smallestResolution);

            this.initialPoints = this.InitialVertices();
            this.initialCircles = this.InitialCircles();

            this.knownTiles = new List<GenericTile>();

        }

        //see https://www.malinc.se/noneuclidean/en/poincaretiling.php why this.d is calculated like this
        private void CalculateInitialDistance(int smallestResolution)
        {
            var numerator = Math.Tan((Math.PI / 2) - (Math.PI / q)) - Math.Tan(Math.PI/p);
            var denominator = Math.Tan((Math.PI / 2) - (Math.PI / q)) + Math.Tan(Math.PI / p);
            double d = Math.Sqrt(numerator / denominator);
            //The formula calculates d as a fraction of a unit circle of length 1, but we have a unit circle of length
            //"smallestResolution / 2" so we need to multiply by that
            this.d = (int) Math.Round(d * (smallestResolution / 2));
        }

        public void GenerateTiling()
        {
            if (this.p == 4 && this.q == 5)
            {
                GenerateHolonomyTiling();
            }
            else
            {
                GenerateGenericTiling();
            }
        }

        private void GenerateHolonomyTiling()
        {
            Direction[] dirs = { Direction.N, Direction.W, Direction.S, Direction.E };
            Step[] steps = { Step.F, Step.L, Step.R };
            int NUM_DESIRED_TILES = 400;
            int generatedTiles = 0;
            HolonomyTile initial = new HolonomyTile(this.initialPoints, this.initialCircles);
            this.knownTiles.Add(initial);
            Queue<HolonomyTile> q = new Queue<HolonomyTile>();

            foreach (var dir in dirs)
            {
                HolonomyTile reflectedTile = initial.ReflectIntoDirection(dir, Step.DC);
                q.Enqueue(reflectedTile);
                this.knownTiles.Add(reflectedTile);
            }
            // //Now we somehow have to generate only Forward, Left and Right tiles iff that is allowed according to the underlying graph
            
            while (q.Count != 0 && generatedTiles < NUM_DESIRED_TILES)
            {
                HolonomyTile current = q.Dequeue();
                Direction currentForwardDir = current.CurrentForwardDirection;
                //Reflect the tile into each of the "steps" which are currently allowed
                foreach (var step in steps)
                {
                    if (!current.IsStepLegal(step))
                    {
                        continue;
                    }
                    else
                    {
                        Direction reflectIn = ConvertStepToDirection(step, currentForwardDir);
                        HolonomyTile reflectedTile = current.ReflectIntoDirection(reflectIn, step);
                        generatedTiles++;
                        this.knownTiles.Add(reflectedTile);
                        q.Enqueue(reflectedTile);
                    }
                }
            }
        }

        private void GenerateGenericTiling()
        {
            Queue<GenericTile> q = new Queue<GenericTile>();
            int NUM_ITERATIONS = 400;
            int iterationCount = 0;
            GenericTile initial = new GenericTile(this.initialPoints, this.initialCircles);
            this.knownTiles.Add(initial);
            q.Enqueue(initial);
            while (q.Count != 0 && iterationCount < NUM_ITERATIONS)
            {
                GenericTile current = q.Dequeue();
                for (int i = 0; i < current.edges.Length; i++)
                {
                    GenericTile reflectedTile = current.ReflectIntoEdge(current.edges[i]);
                    if (!this.knownTiles.Contains(reflectedTile))
                    {
                        this.knownTiles.Add(reflectedTile);
                        q.Enqueue(reflectedTile);
                    }
                }
                iterationCount++;
            }
        }

        private static Direction ConvertStepToDirection(Step s, Direction currentForwardDirection)
        {
            switch (currentForwardDirection)
            {
                case Direction.N:
                    switch (s)
                    {
                        case Step.F:
                            return Direction.N;
                        case Step.L:
                            return Direction.W;
                        case Step.R:
                            return Direction.E;
                    }
                    break;
                case Direction.W:
                    switch (s)
                    {
                        case Step.F:
                            return Direction.W;
                        case Step.L:
                            return Direction.S;
                        case Step.R:
                            return Direction.N;
                    }

                    break;
                case Direction.S:
                    switch (s)
                    {
                        case Step.F:
                            return Direction.S;
                        case Step.L:
                            return Direction.E;
                        case Step.R:
                            return Direction.W;
                    }
                    break;
                case Direction.E:
                    switch (s)
                    {
                        case Step.F:
                            return Direction.E;
                        case Step.L:
                            return Direction.N;
                        case Step.R:
                            return Direction.S;
                    }
                    break;
            }
            //this never happens
            return Direction.E;
        }
        
        public void DrawTiling(Graphics g)
        {
            //Draw unit circle
            g.DrawEllipse(Pens.Red, this.unitCircle.GetRectangle());
            //Draw each known tile
            foreach (var tile in this.knownTiles)
            {
                tile.DrawBounds(g);
                //tile.FillTile(g);
            }
        }

        public void FillTiling(Graphics g)
        {
            //Draw unit circle
            g.DrawEllipse(Pens.Red, this.unitCircle.GetRectangle());
            foreach (var tile in this.knownTiles)
            {
                tile.FillTile(g);
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