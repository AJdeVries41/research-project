using System;
using System.Collections.Generic;
using System.Security.Permissions;

namespace research_project
{
    //Whether we are allowed to generate a tile in a specific Step depends on three conditions
    //https://math.stackexchange.com/questions/2222314/description-of-the-order-5-square-tiling-of-the-hyperbolic-plane-as-a-graph/2231612#2231612
    //1) No two right consecutive steps are allowed
    //2) No two left steps are allowed to happen without at least 1 right step in between
    public class HolonomyTile : GenericTile
    {
        public Direction CurrentForwardDirection;
        public String path;
        //You are always allowed to take the first left step that appears
        public bool hasFirstLeftOccurred;
        //Has a right step occurred before we encountered the next left step?
        public bool rightBeforeLeft;
        //Was the last step a right step?
        public bool wasLastStepRight;
        
        //Constructor for the initial tile
        public HolonomyTile(List<(double, double)> points, List<Circle> circles) : base(points, circles)
        {
            this.CurrentForwardDirection = Direction.O;
            this.path = "";
            this.hasFirstLeftOccurred = false;
            this.rightBeforeLeft = false;
            this.wasLastStepRight = false;
        }
        
        //Constructor for a general tile which was created via reflection
        public HolonomyTile(Geodesic[] edges, Direction newForwardDirection, String newPath, 
            bool hasFirstLeftOccurred, bool rightBeforeLeft, bool wasLastStepRight) : base(edges)
        {
            this.CurrentForwardDirection = newForwardDirection;
            this.path = newPath;
            this.hasFirstLeftOccurred = hasFirstLeftOccurred;
            this.rightBeforeLeft = rightBeforeLeft;
            this.wasLastStepRight = wasLastStepRight;
        }
        
        public bool IsInitialTile()
        {
            return this.CurrentForwardDirection == Direction.O;
        }
        
        public bool IsLeftAllowed()
        {
            return (!this.hasFirstLeftOccurred) || rightBeforeLeft;
        }

        public bool IsStepLegal(Step step)
        {
            switch (step)
            {
                case Step.F:
                    return true;
                case Step.R:
                    return !this.wasLastStepRight;
                //step == Step.Left
                default:
                    return this.IsLeftAllowed();
            }
        }
        
        /// <summary>
        /// Assumes a {4, 5} tiling
        /// Reflects into any of North, West, South or East directions
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public HolonomyTile ReflectIntoDirection(Direction dir, Step step)
        {
            var newEdges = new Geodesic[this.edges.Length];
            var reflectInto = this.edges[Convert.ToInt32(dir)];
            Geodesic reflNorth = this.edges[Convert.ToInt32(Direction.N)].ReflectIntoEdge(reflectInto);
            Geodesic reflWest = this.edges[Convert.ToInt32(Direction.W)].ReflectIntoEdge(reflectInto);
            Geodesic reflSouth = this.edges[Convert.ToInt32(Direction.S)].ReflectIntoEdge(reflectInto);
            Geodesic reflEast = this.edges[Convert.ToInt32(Direction.E)].ReflectIntoEdge(reflectInto);
            switch (dir)
            {
                case Direction.N:
                    newEdges[Convert.ToInt32(Direction.N)] = reflSouth;
                    newEdges[Convert.ToInt32(Direction.W)] = reflWest;
                    newEdges[Convert.ToInt32(Direction.S)] = reflectInto;
                    newEdges[Convert.ToInt32(Direction.E)] = reflEast;
                    break;
                case Direction.W:
                    newEdges[Convert.ToInt32(Direction.N)] = reflNorth;
                    newEdges[Convert.ToInt32(Direction.W)] = reflEast;
                    newEdges[Convert.ToInt32(Direction.S)] = reflSouth;
                    newEdges[Convert.ToInt32(Direction.E)] = reflectInto;
                    break;
                case Direction.S:
                    newEdges[Convert.ToInt32(Direction.N)] = reflectInto;
                    newEdges[Convert.ToInt32(Direction.W)] = reflWest;
                    newEdges[Convert.ToInt32(Direction.S)] = reflNorth;
                    newEdges[Convert.ToInt32(Direction.E)] = reflEast;
                    break;
                case Direction.E:
                    newEdges[Convert.ToInt32(Direction.N)] = reflNorth;
                    newEdges[Convert.ToInt32(Direction.W)] = reflectInto;
                    newEdges[Convert.ToInt32(Direction.S)] = reflSouth;
                    newEdges[Convert.ToInt32(Direction.E)] = reflWest;
                    break;
            }
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
                hasFirstLeftOccurred = this.hasFirstLeftOccurred;
                rightBeforeLeft = this.rightBeforeLeft;
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
                hasFirstLeftOccurred = this.hasFirstLeftOccurred;
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
    }
}