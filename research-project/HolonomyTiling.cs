using System;
using System.Collections.Generic;
using System.Drawing;

namespace research_project
{
    public class HolonomyTiling
    {
        //Kind of an arbitrary table, but you should read it as
        //StepToDirection[currentForwardDirection][step]
        //which, when cast to a Direction, gives the direction that "step" represents
        public int P;
        public int Q;
        private static readonly int[,] StepToDirection = new int[4, 3] { { 0, 1, 3 }, { 1, 2, 0 }, { 2, 3, 1 }, { 3, 0, 2 } };
        public Circle UnitCircle { get; set; }
        public List<(double, double)> InitialPoints;
        public List<HolonomyTile> KnownTiles;

        public HolonomyTiling(int smallestResolution, double initialRotation)
        {
            this.P = 4;
            this.Q = 5;
            this.UnitCircle = new Circle((0, 0), smallestResolution / 2);
            int d = CalculateInitialDistance(smallestResolution);
            this.InitialPoints = this.CalculateInitialPoints(d, initialRotation);
            this.KnownTiles = new List<HolonomyTile>();
        }

        /// <summary>
        /// Procedure to generate the initial tile from the initial points
        /// </summary>
        /// <returns></returns>
        private HolonomyTile GenerateInitialTile()
        {
            Geodesic[] edges = new Geodesic[this.InitialPoints.Count];
            int j = 1;
            for (int i = 0; i < this.InitialPoints.Count; i++)
            {
                (double, double) fromPoint = this.InitialPoints[i];
                (double, double) toPoint = this.InitialPoints[j];
                Circle connectingCircle = GeomUtils.CircleBetweenPointsInDisk(fromPoint, toPoint, this.UnitCircle);
                Geodesic edge = new Geodesic(connectingCircle, fromPoint, toPoint);
                edges[i] = edge;
                j++;
                if (j == this.InitialPoints.Count)
                {
                    j = 0;
                }
            }
            return new HolonomyTile(edges, Direction.O, "", false, false, false);
        }
        
        /// <summary>
        /// The main algorithm to generate a Holonomy tiling with a certain amount of tiles
        /// </summary>
        /// <param name="numDesiredTiles"></param>
        /// <exception cref="ArgumentException"></exception>
        public void GenerateTiling(int numDesiredTiles = 100)
        {
            if (numDesiredTiles <= 0)
            {
                throw new ArgumentException("Need to generate at least 1 tile");
            }
            Direction[] dirs = { Direction.N, Direction.W, Direction.S, Direction.E };
            Step[] steps = { Step.F, Step.L, Step.R };
            Queue<HolonomyTile> queue = new Queue<HolonomyTile>();
            HolonomyTile initial = this.GenerateInitialTile();
            this.KnownTiles.Add(initial);
            if (this.KnownTiles.Count == numDesiredTiles)
            {
                return;
            }
            foreach (var dir in dirs)
            {
                HolonomyTile reflectedTile = initial.ReflectIntoDirection(dir, this.UnitCircle, Step.Dc);
                queue.Enqueue(reflectedTile);
                this.KnownTiles.Add(reflectedTile);
                if (this.KnownTiles.Count == numDesiredTiles)
                {
                    return;
                }
            }
            while (queue.Count != 0)
            {
                HolonomyTile current = queue.Dequeue();
                int i = 0;
                while (i < steps.Length && this.KnownTiles.Count < numDesiredTiles)
                {
                    if (current.IsStepLegal(steps[i]))
                    {
                        Direction reflectIn = ConvertStepToDirection(steps[i], current.CurrentForwardDirection);
                        HolonomyTile reflectedTile = current.ReflectIntoDirection(reflectIn, this.UnitCircle, steps[i]);
                        this.KnownTiles.Add(reflectedTile);
                        queue.Enqueue(reflectedTile);
                    }
                    i++;
                }
            }
        }
        
        /// <summary>
        /// Converts a step to a direction given the currentForwardDirection of a tile
        /// </summary>
        /// <param name="s"></param>
        /// <param name="currentForwardDirection"></param>
        /// <returns></returns>
        private static Direction ConvertStepToDirection(Step s, Direction currentForwardDirection)
        {
            int dirInt = Convert.ToInt32(currentForwardDirection);
            int stepInt = Convert.ToInt32(s);
            return (Direction) StepToDirection[dirInt, stepInt];
        }

        /// <summary>
        /// Calculate the distance d how far the initial points should be from the origin
        /// </summary>
        /// <param name="smallestResolution"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Move the intial points centerd at a new point B instead of (0, 0)
        /// </summary>
        /// <param name="B"></param>
        public void MoveInitialTile((double, double) B)
        {
            var invCircle = GeomUtils.HyperbolicBisectorFromCenter(B, this.UnitCircle);
            var newInitialPoints = new List<(double, double)>();
            //iterate thru the initial points of the tiling
            for (int i = 0; i < this.InitialPoints.Count; i++)
            {
                (double, double) invPoint = GeomUtils.InvertPoint(this.InitialPoints[i], invCircle);
                newInitialPoints.Add(invPoint);
            }
            this.InitialPoints = newInitialPoints;
        }

        /// <summary>
        /// Draws the lines of each tile
        /// </summary>
        /// <param name="g"></param>
        /// <param name="c"></param>
        /// <param name="width"></param>
        public void DrawTiling(Graphics g, Color c, int width)
        {
            //Draw unit circle
            g.DrawEllipse(Pens.Red, this.UnitCircle.GetRectangle());
            Pen drawingPen = new Pen(c, width);
            //Draw each known tile
            foreach (var tile in this.KnownTiles)
            {
                tile.DrawBounds(g, drawingPen);
            }
        }

        /// <summary>
        /// Draws each tile with a filled color
        /// </summary>
        /// <param name="g"></param>
        public void DrawColoredTiling(Graphics g)
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