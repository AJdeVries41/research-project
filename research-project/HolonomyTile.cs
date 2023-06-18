﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;

namespace research_project
{
    public class HolonomyTile
    {
        public Geodesic[] Edges;
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
        public HolonomyTile ReflectIntoDirection(Direction dir, Step step, Circle unitCircle)
        {
            var newEdges = new Geodesic[this.Edges.Length];
            var reflectionCircle = this.Edges[Convert.ToInt32(dir)].c;
            //swap the start and endpoint of reflectinto, that is the new edge
            //this.Edges[Convert.ToInt32(dir)] will be an edge in the reflected tile,
            //but with swapped start and end points. As a consequence, the start and sweep angle will also
            //have to be recomputed
            var reflectInEdge = this.Edges[Convert.ToInt32(dir)];
            //originalEdge = reflectInEdge with flipped start and endpoints
            var originalEdge = new Geodesic(reflectionCircle, reflectInEdge.endPoint, reflectInEdge.startPoint);

            Geodesic reflNorth = this.Edges[Convert.ToInt32(Direction.N)].ReflectAlongEdge(reflectionCircle, unitCircle);
            Geodesic reflWest = this.Edges[Convert.ToInt32(Direction.W)].ReflectAlongEdge(reflectionCircle, unitCircle);
            Geodesic reflSouth = this.Edges[Convert.ToInt32(Direction.S)].ReflectAlongEdge(reflectionCircle, unitCircle);
            Geodesic reflEast = this.Edges[Convert.ToInt32(Direction.E)].ReflectAlongEdge(reflectionCircle, unitCircle);
            switch (dir)
            {
                case Direction.N:
                    newEdges[Convert.ToInt32(Direction.N)] = reflSouth;
                    newEdges[Convert.ToInt32(Direction.W)] = reflWest;
                    newEdges[Convert.ToInt32(Direction.S)] = originalEdge;
                    newEdges[Convert.ToInt32(Direction.E)] = reflEast;
                    break;
                case Direction.W:
                    newEdges[Convert.ToInt32(Direction.N)] = reflNorth;
                    newEdges[Convert.ToInt32(Direction.W)] = reflEast;
                    newEdges[Convert.ToInt32(Direction.S)] = reflSouth;
                    newEdges[Convert.ToInt32(Direction.E)] = originalEdge;
                    break;
                case Direction.S:
                    newEdges[Convert.ToInt32(Direction.N)] = originalEdge;
                    newEdges[Convert.ToInt32(Direction.W)] = reflWest;
                    newEdges[Convert.ToInt32(Direction.S)] = reflNorth;
                    newEdges[Convert.ToInt32(Direction.E)] = reflEast;
                    break;
                case Direction.E:
                    newEdges[Convert.ToInt32(Direction.N)] = reflNorth;
                    newEdges[Convert.ToInt32(Direction.W)] = originalEdge;
                    newEdges[Convert.ToInt32(Direction.S)] = reflSouth;
                    newEdges[Convert.ToInt32(Direction.E)] = reflWest;
                    break;
            }
            return UpdateGenerationConstraints(newEdges, dir, step);
        }

        
        private HolonomyTile UpdateGenerationConstraints(Geodesic[] newEdges, Direction dir, Step step)
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