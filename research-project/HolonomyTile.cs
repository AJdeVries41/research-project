using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text;

namespace research_project
{
    public class HolonomyTile
    {
        public Geodesic[] Edges;
        public Direction CurrentForwardDirection;
        private String path;
        //Whether we are allowed to generate a tile in a specific Step depends on three conditions
        //1) No two right consecutive steps are allowed
        //2) No two left steps are allowed to happen without at least 1 right step in between
        //3) You are always allowed to take the first left step that appears
        public bool HasFirstLeftOccurred;
        //Has a right step occurred before we encountered the next left step?
        public bool RightBeforeLeft;
        //Was the last step a right step?
        public bool WasLastStepRight;

        /// <summary>
        ///
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="newForwardDirection">Specifies the direction that is the forward direction for this tile</param>
        /// <param name="newPath"></param>
        /// <param name="hasFirstLeftOccurred">Has the first left step occurred when reaching this tile</param>
        /// <param name="rightBeforeLeft">Did we see another right step before the next left step</param>
        /// <param name="wasLastStepRight">Was the previous step to arrive here a right step</param>
        public HolonomyTile(Geodesic[] edges, Direction newForwardDirection, String newPath, 
            bool hasFirstLeftOccurred, bool rightBeforeLeft, bool wasLastStepRight)
        {
            this.Edges = edges;
            this.CurrentForwardDirection = newForwardDirection;
            this.path = newPath;
            this.HasFirstLeftOccurred = hasFirstLeftOccurred;
            this.RightBeforeLeft = rightBeforeLeft;
            this.WasLastStepRight = wasLastStepRight;
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
        /// Reflecs this tile into either north, west, south, or east
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="unitCircle"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public HolonomyTile ReflectIntoDirection(Direction dir, Circle unitCircle, Step step)
        {
            var newEdges = new Geodesic[this.Edges.Length];
            var reflectionCircle = this.Edges[Convert.ToInt32(dir)].c;
            var reflectInEdge = this.Edges[Convert.ToInt32(dir)];
            var originalEdgeReversed = new Geodesic(reflectionCircle, reflectInEdge.endPoint, reflectInEdge.startPoint);
            //So for any of the four directions holds: if we reflect in "dir", then the new "dir" edge will be the opposite of "dir"
            newEdges[Convert.ToInt32(dir)] = this.Edges[Convert.ToInt32(dir.Opposite())].ReflectIntoEdge(reflectionCircle, unitCircle);
            newEdges[Convert.ToInt32(dir.Opposite())] = originalEdgeReversed;
            var orthogonals = dir.Orthogonals();
            newEdges[Convert.ToInt32(orthogonals.Item1)] = this.Edges[Convert.ToInt32(orthogonals.Item1)]
                .ReflectIntoEdge(reflectionCircle, unitCircle);
            newEdges[Convert.ToInt32(orthogonals.Item2)] = this.Edges[Convert.ToInt32(orthogonals.Item2)]
                .ReflectIntoEdge(reflectionCircle, unitCircle);
            return UpdateGenerationConstraints(newEdges, dir, step);
        }

        /// <summary>
        /// Updates the info necessary for determining what subsequent tiles we can generate from this tile
        /// </summary>
        /// <param name="newEdges"></param>
        /// <param name="dir"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        /// <exception cref="SystemException"></exception>
        private HolonomyTile UpdateGenerationConstraints(Geodesic[] newEdges, Direction dir, Step step)
        {
            bool hasFirstLeftOccurred;
            bool rightBeforeLeft;
            bool wasLastStepRight;
            String newPath;
            //construct the booleans that determine legal steps based on the current step
            //if we step away from the initial tile
            switch (step)
            {
                case Step.Dc:
                    hasFirstLeftOccurred = false;
                    rightBeforeLeft = false;
                    wasLastStepRight = false;
                    newPath = dir.ToString();
                    break;
                case Step.F:
                    hasFirstLeftOccurred = this.HasFirstLeftOccurred;
                    rightBeforeLeft = this.RightBeforeLeft;
                    wasLastStepRight = false;
                    newPath = this.path + step.ToString();
                    break;
                case Step.L:
                    hasFirstLeftOccurred = true;
                    rightBeforeLeft = false;
                    wasLastStepRight = false;
                    newPath = this.path + step.ToString();
                    break;
                //case Step.R
                default:
                    hasFirstLeftOccurred = this.HasFirstLeftOccurred;
                    rightBeforeLeft = true;
                    wasLastStepRight = true;
                    newPath = this.path + step.ToString();
                    break;
            }
            HolonomyTile reflectedTile = new HolonomyTile(newEdges, dir, newPath, hasFirstLeftOccurred, rightBeforeLeft, wasLastStepRight);
            return reflectedTile;
        }

        /// <summary>
        /// Draws the tile by floodfilling the tile with a color based on its path
        /// </summary>
        /// <param name="g"></param>
        public void FillTile(Graphics g)
        {
            GraphicsPath gp = new GraphicsPath();
            for (int i = 0; i < this.Edges.Length; i++)
            {
                try
                {
                    gp.AddArc(this.Edges[i].c.GetRectangle(), this.Edges[i].startAngleDegree,
                        this.Edges[i].sweepAngleDegree);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine($"Got argument exception for tile {this.ToString()}");
                    //Don't try to give this tile a color then I suppose...
                    return;
                }
            }
            MD5 md5 = MD5.Create();
            byte[] input = Encoding.ASCII.GetBytes(this.path);
            byte[] hash = md5.ComputeHash(input);
            Color c = Color.FromArgb(hash[0], hash[1], hash[2]);
            Brush b = new SolidBrush(c);
            g.FillRegion(b, new Region(gp));
        }
        
        /// <summary>
        /// Draws the bounds of this tile
        /// </summary>
        /// <param name="g"></param>
        /// <param name="drawingPen"></param>
        public void DrawBounds(Graphics g, Pen drawingPen)
        {
            foreach (var geo in this.Edges)
            {
                try
                {
                    g.DrawArc(drawingPen,geo.c.GetRectangle(), geo.startAngleDegree, geo.sweepAngleDegree);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine($"Got argument exception for tile {this.ToString()}");
                    //Don't try to draw this tile then I suppose...
                    return;
                } 
            }
        }
        
        public override string ToString()
        {
            return this.path;
        }
    }
}