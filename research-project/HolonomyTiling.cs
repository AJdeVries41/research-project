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
        public HolonomyTile InitialTile;
        public List<HolonomyTile> KnownTiles;
        

        public HolonomyTiling(int smallestResolution, double initialRotation)
        {
            this.P = 4;
            this.Q = 5;
            this.UnitCircle = new Circle((0, 0), smallestResolution / 2);
            int d = CalculateInitialDistance(smallestResolution);
            var initialPoints = this.CalculateInitialPoints(d, initialRotation);
            var initialCircles = this.CalculateInitialCircles(initialPoints);
            this.InitialTile = new HolonomyTile(initialPoints, initialCircles);
            this.KnownTiles = new List<HolonomyTile>();
        }
        
        
        
        public void GenerateTiling()
        {
            Direction[] dirs = { Direction.N, Direction.W, Direction.S, Direction.E };
            Step[] steps = { Step.F, Step.L, Step.R };
            
            this.KnownTiles.Add(this.InitialTile);
            int NUM_DESIRED_TILES = 100;
            int generatedTiles = 0;
            Queue<HolonomyTile> q = new Queue<HolonomyTile>();
            HolonomyTile initial = this.InitialTile;

            foreach (var dir in dirs)
            {
                HolonomyTile reflectedTile = initial.ReflectIntoDirection(dir, Step.DC);
                q.Enqueue(reflectedTile);
                this.KnownTiles.Add(reflectedTile);
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
                        //Direction reflectIn = this.stepToDirection[currentForwardDir, step];
                        Direction reflectIn = ConvertStepToDirection(step, currentForwardDir);
                        HolonomyTile reflectedTile = current.ReflectIntoDirection(reflectIn, step);
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

        public void MoveInitialTile((double, double) B, Graphics g)
        {
            HolonomyTile initial = this.InitialTile;
            var invCircle = GeomUtils.HyperbolicBisectorFromCenter2(B, this.UnitCircle, g);
            var newEdges = new Geodesic[initial.Edges.Length];
            //iterate thru the initial points of the tiling
            for (int i = 0; i < initial.Edges.Length; i++)
            {
                (double, double) startPoint = initial.Edges[i].startPoint;
                (double, double) endPoint = initial.Edges[i].endPoint;
                (double, double) reflStartPoint = GeomUtils.InvertPoint(startPoint, invCircle);
                (double, double) reflEndPoint = GeomUtils.InvertPoint(endPoint, invCircle);
                Circle connectingCircle =
                    GeomUtils.CircleBetweenPointsInDisk(reflStartPoint, reflEndPoint, this.UnitCircle);
                Geodesic newEdge = new Geodesic(connectingCircle, reflStartPoint, reflEndPoint);
                newEdges[i] = newEdge;
            }
            this.InitialTile = new HolonomyTile(newEdges, Direction.O, "", false, false, false);
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