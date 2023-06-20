using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

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

        public HolonomyTile GenerateInitialTile()
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
        
        
        
        public void GenerateTiling(int numDesiredTiles = 100)
        {
            Direction[] dirs = { Direction.N, Direction.W, Direction.S, Direction.E };
            Step[] steps = { Step.F, Step.L, Step.R };
            
            int generatedTiles = 0;
            Queue<HolonomyTile> q = new Queue<HolonomyTile>();

            HolonomyTile initial = this.GenerateInitialTile();
            this.KnownTiles.Add(initial);
            generatedTiles++;
            if (generatedTiles == numDesiredTiles)
            {
                return;
            }
            foreach (var dir in dirs)
            {
                HolonomyTile reflectedTile = initial.ReflectIntoDirection(dir, Step.Dc, this.UnitCircle);
                q.Enqueue(reflectedTile);
                this.KnownTiles.Add(reflectedTile);
                generatedTiles++;
                if (generatedTiles == numDesiredTiles)
                {
                    return;
                }
            }

            // //Now we somehow have to generate only Forward, Left and Right tiles iff that is allowed according to the underlying graph
            while (q.Count != 0 && generatedTiles < numDesiredTiles)
            {
                HolonomyTile current = q.Dequeue();
                //Reflect the tile into each of the "steps" which are currently allowed
                foreach (var step in steps)
                {
                    if (!current.IsStepLegal(step))
                    {
                        continue;
                    }
                    else
                    {
                        //Direction reflectIn = this.stepToDirection[currentForwardDir, step];
                        Direction reflectIn = ConvertStepToDirection(step, current.CurrentForwardDirection);
                        HolonomyTile reflectedTile = current.ReflectIntoDirection(reflectIn, step, this.UnitCircle);
                        generatedTiles++;
                        this.KnownTiles.Add(reflectedTile);
                        q.Enqueue(reflectedTile);
                    }
                }
            }
        }
        
        private static Direction ConvertStepToDirection(Step s, Direction currentForwardDirection)
        {
            int dirInt = Convert.ToInt32(currentForwardDirection);
            int stepInt = Convert.ToInt32(s);
            return (Direction) StepToDirection[dirInt, stepInt];
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

        public void MoveInitialTile((double, double) B, Graphics g)
        {
            var invCircle = GeomUtils.HyperbolicBisectorFromCenter(B, this.UnitCircle, g);
            var newInitialPoints = new List<(double, double)>();
            
            //iterate thru the initial points of the tiling
            for (int i = 0; i < this.InitialPoints.Count; i++)
            {
                (double, double) invPoint = GeomUtils.InvertPoint(this.InitialPoints[i], invCircle);
                newInitialPoints.Add(invPoint);
            }
            this.InitialPoints = newInitialPoints;
        }

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