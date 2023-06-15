using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;

namespace research_project
{
    public class HolonomyTile : Tile
    {
        public Direction CurrentForwardDirection;
        private String path;
        //Whether we are allowed to generate a tile in a specific Step depends on three conditions
        //https://math.stackexchange.com/questions/2222314/description-of-the-order-5-square-tiling-of-the-hyperbolic-plane-as-a-graph/2231612#2231612
        //1) No two right consecutive steps are allowed
        //2) No two left steps are allowed to happen without at least 1 right step in between
        //You are always allowed to take the first left step that appears
        public bool HasFirstLeftOccurred;
        //Has a right step occurred before we encountered the next left step?
        public bool RightBeforeLeft;
        //Was the last step a right step?
        public bool WasLastStepRight;
        
        /// <summary>
        /// Constructor for the initial tile
        /// For a HolonomyTile, it holds that Edges[0] = top edge,
        /// Edges[1] = left edge
        /// Edges[2] = bottom edge
        /// Edges[3] = right edge
        /// </summary>
        /// <param name="points"></param>
        /// <param name="circles"></param>
        public HolonomyTile(List<(double, double)> points, List<Circle> circles) : base(points, circles)
        {
            this.CurrentForwardDirection = Direction.O;
            this.path = "";
            this.HasFirstLeftOccurred = false;
            this.RightBeforeLeft = false;
            this.WasLastStepRight = false;
        }
        
        /// <summary>
        /// Constructor for a tile that was created via reflection
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="newForwardDirection">Specifies the direction that is the forward direction for this tile</param>
        /// <param name="newPath"></param>
        /// <param name="hasFirstLeftOccurred">Has the first left step occurred when reaching this tile</param>
        /// <param name="rightBeforeLeft">Did we see another right step before the next left step</param>
        /// <param name="wasLastStepRight">Was the previous step to arrive here a right step</param>
        public HolonomyTile(Geodesic[] edges, Direction newForwardDirection, String newPath, 
            bool hasFirstLeftOccurred, bool rightBeforeLeft, bool wasLastStepRight) : base(edges)
        {
            this.CurrentForwardDirection = newForwardDirection;
            this.path = newPath;
            this.HasFirstLeftOccurred = hasFirstLeftOccurred;
            this.RightBeforeLeft = rightBeforeLeft;
            this.WasLastStepRight = wasLastStepRight;
        }
        
        private bool IsInitialTile()
        {
            return this.CurrentForwardDirection == Direction.O;
        }
        
        private bool IsLeftAllowed()
        {
            return (!this.HasFirstLeftOccurred) || RightBeforeLeft;
        }

        public bool IsStepLegal(Step step)
        {
            switch (step)
            {
                case Step.F:
                    return true;
                case Step.R:
                    return !this.WasLastStepRight;
                //step == Step.Left
                default:
                    return this.IsLeftAllowed();
            }
        }
        
        /// <summary>
        /// Reflects this tile into any of North, West, South or East directions
        /// </summary>
        /// <param name="dir">direction into which we should reflect (N, W, S, E)</param>
        /// <param name="step">which step was taken from this tile to get here (F, L, R)</param>
        /// <returns></returns>
        public HolonomyTile ReflectIntoDirection(Direction dir, Step step)
        {
            var newEdges = new Geodesic[this.Edges.Length];
            var reflectionCircle = this.Edges[Convert.ToInt32(dir)].c;
            //swap the start and endpoint of reflectinto, that is the new edge
            //this.Edges[Convert.ToInt32(dir)] will be an edge in the reflected tile,
            //but with swapped start and end points. As a consqequence, the start and sweep angle will also
            //have to be recomputed
            var reflectInEdge = this.Edges[Convert.ToInt32(dir)];
            var newEdge = new Geodesic(reflectionCircle, reflectInEdge.endPoint, reflectInEdge.startPoint);

            Geodesic reflNorth = this.Edges[Convert.ToInt32(Direction.N)].ReflectIntoEdge(reflectionCircle);
            Geodesic reflWest = this.Edges[Convert.ToInt32(Direction.W)].ReflectIntoEdge(reflectionCircle);
            Geodesic reflSouth = this.Edges[Convert.ToInt32(Direction.S)].ReflectIntoEdge(reflectionCircle);
            Geodesic reflEast = this.Edges[Convert.ToInt32(Direction.E)].ReflectIntoEdge(reflectionCircle);
            switch (dir)
            {
                case Direction.N:
                    newEdges[Convert.ToInt32(Direction.N)] = reflSouth;
                    newEdges[Convert.ToInt32(Direction.W)] = reflWest;
                    newEdges[Convert.ToInt32(Direction.S)] = newEdge;
                    newEdges[Convert.ToInt32(Direction.E)] = reflEast;
                    break;
                case Direction.W:
                    newEdges[Convert.ToInt32(Direction.N)] = reflNorth;
                    newEdges[Convert.ToInt32(Direction.W)] = reflEast;
                    newEdges[Convert.ToInt32(Direction.S)] = reflSouth;
                    newEdges[Convert.ToInt32(Direction.E)] = newEdge;
                    break;
                case Direction.S:
                    newEdges[Convert.ToInt32(Direction.N)] = newEdge;
                    newEdges[Convert.ToInt32(Direction.W)] = reflWest;
                    newEdges[Convert.ToInt32(Direction.S)] = reflNorth;
                    newEdges[Convert.ToInt32(Direction.E)] = reflEast;
                    break;
                case Direction.E:
                    newEdges[Convert.ToInt32(Direction.N)] = reflNorth;
                    newEdges[Convert.ToInt32(Direction.W)] = newEdge;
                    newEdges[Convert.ToInt32(Direction.S)] = reflSouth;
                    newEdges[Convert.ToInt32(Direction.E)] = reflWest;
                    break;
            }
            return ConstructNextTile(newEdges, dir, step);
        }

        private HolonomyTile ConstructNextTile(Geodesic[] newEdges, Direction dir, Step step)
        {
            bool hasFirstLeftOccurred;
            bool rightBeforeLeft;
            bool wasLastStepRight;
            String newPath;
            //construct the booleans that determine legal steps based on the current step
            //if we step away from the initial tile
            if (this.IsInitialTile())
            {
                hasFirstLeftOccurred = false;
                rightBeforeLeft = false;
                wasLastStepRight = false;
                newPath = dir.ToString();
            }
            else if (step == Step.F)
            {
                hasFirstLeftOccurred = this.HasFirstLeftOccurred;
                rightBeforeLeft = this.RightBeforeLeft;
                wasLastStepRight = false;
                newPath = this.path + step.ToString();
            }
            else if (step == Step.L)
            {
                hasFirstLeftOccurred = true;
                rightBeforeLeft = false;
                wasLastStepRight = false;
                newPath = this.path + step.ToString();
            }
            else if (step == Step.R)
            {
                hasFirstLeftOccurred = this.HasFirstLeftOccurred;
                rightBeforeLeft = true;
                wasLastStepRight = true;
                newPath = this.path + step.ToString();
            }
            //step = Step.Dc
            else
            {
                throw new SystemException("this should not happen");
            }
            HolonomyTile reflectedTile = new HolonomyTile(newEdges, dir, newPath, hasFirstLeftOccurred, rightBeforeLeft, wasLastStepRight);
            return reflectedTile;
        }

        /// <summary>
        /// Set color of the tile based on the path
        /// of this tile
        /// </summary>
        /// <param name="g"></param>
        public override void FillTile(Graphics g)
        {
            GraphicsPath gp = new GraphicsPath();
            for (int i = 0; i < this.Edges.Length; i++)
            {
                gp.AddArc(this.Edges[i].c.GetRectangle(), this.Edges[i].startAngleDegree, this.Edges[i].sweepAngleDegree);
            }
            MD5 md5 = MD5.Create();
            byte[] input = Encoding.ASCII.GetBytes(this.path);
            byte[] hash = md5.ComputeHash(input);
            Color c = Color.FromArgb(hash[0], hash[1], hash[2]);
            Brush b = new SolidBrush(c);
            g.FillRegion(b, new Region(gp));
        }
        
        

        public override string ToString()
        {
            return this.path;
        }
    }
}