using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;

namespace research_project
{
    public class Tile : IEquatable<Tile>
    {
        //public List<Geodesic> edges;
        public Geodesic[] edges;
        public bool hasFirstLeftOccurred;
        public bool rightBeforeLeft;
        public bool wasLastStepRight;
        //the TileSide which currently maps to the "Forward" direction
        //E.G. if we initially turn "west" from the origin, Forward=TileSide.West
        //From that you can infer that "Left=TileSide.South" and "Right=TileSide.North"
        public TileSide currentForwardDirection;
        public String path;

        //When we don't care about current forward direction, set it to 0 since we have to set it to something
        public Tile(Geodesic[] edges)
        {
            this.edges = edges;
            this.currentForwardDirection = 0;
        }
        
        //Constructor for a general tile which was created via reflection
        public Tile(Geodesic[] edges, TileSide newForwardDirection, String newPath, 
            bool hasFirstLeftOccurred, bool rightBeforeLeft, bool wasLastStepRight)
        {
            this.edges = edges;
            this.currentForwardDirection = newForwardDirection;
            this.path = newPath;
            this.hasFirstLeftOccurred = hasFirstLeftOccurred;
            this.rightBeforeLeft = rightBeforeLeft;
            this.wasLastStepRight = wasLastStepRight;
        }
        
        //Constructor for the initial tile
        public Tile(List<(double, double)> points, List<Circle> circles)
        {
            //if there are p points that also means a polygon has p sides
            this.edges = new Geodesic[points.Count];
            ConstructInitialEdges(points, circles);
            //We don't know yet the current direction when we generate the initial tile because we could 
            //go in all 4 direction
            this.currentForwardDirection = 0;
            
        }

        public bool isLeftAllowed()
        {
            return (!this.hasFirstLeftOccurred) || rightBeforeLeft;
        }

        public void Draw(Graphics g)
        {
            //For debugging purposes, print the path of this tile. I can't figure out why I keep generating duplicates
            Console.WriteLine(this.path);
            foreach (var geo in this.edges)
            {
                if (geo != null)
                {
                    geo.Draw(g);
                }
            }
        }

        public void ConstructInitialEdges(List<(double, double)> points, List<Circle> circles)
        {
            int j = 1;
            for (int i = 0; i < points.Count; i++)
            {
                Circle c = circles[i];
                
                //we are drawing the circle counterclockwise from the 2nd point to the 1st point
                var start = points[j];
                var dest = points[i];

                Geodesic edge = new Geodesic(c, start, dest);
                this.edges[i] = edge;
                
                j++;
                if (j == points.Count)
                {
                    j = 0;
                }
                
            }
        }

        public Tile ReflectIntoEdge(Geodesic reflectInto)
        {
            var newEdges = new Geodesic[this.edges.Length];
            //reflect all edges[j] into edges[i]
            for (int j = 0; j < this.edges.Length; j++)
            {
                Geodesic reflection = this.edges[j].ReflectIntoEdge(reflectInto);
                newEdges[j] = reflection;
            }
            Tile newTile = new Tile(newEdges);
            return newTile;
        }

        //Assumes a {4, 5} tiling
        //returns a new tile with correct north, west, south, east mapping for the generated tile
        public Tile ReflectIntoSide(TileSide side, String newPath)
        {
            var newEdges = new Geodesic[this.edges.Length];
            var reflectInto = this.edges[Convert.ToInt32(side)];
            Geodesic reflNorth = this.edges[Convert.ToInt32(TileSide.North)].ReflectIntoEdge(reflectInto);
            Geodesic reflWest = this.edges[Convert.ToInt32(TileSide.West)].ReflectIntoEdge(reflectInto);
            Geodesic reflSouth = this.edges[Convert.ToInt32(TileSide.South)].ReflectIntoEdge(reflectInto);
            Geodesic reflEast = this.edges[Convert.ToInt32(TileSide.East)].ReflectIntoEdge(reflectInto);
            switch (side)
            {
                case TileSide.North:
                    newEdges[Convert.ToInt32(TileSide.North)] = reflSouth;
                    newEdges[Convert.ToInt32(TileSide.West)] = reflWest;
                    newEdges[Convert.ToInt32(TileSide.South)] = reflectInto;
                    newEdges[Convert.ToInt32(TileSide.East)] = reflEast;
                    break;
                case TileSide.West:
                    newEdges[Convert.ToInt32(TileSide.North)] = reflNorth;
                    newEdges[Convert.ToInt32(TileSide.West)] = reflEast;
                    newEdges[Convert.ToInt32(TileSide.South)] = reflSouth;
                    newEdges[Convert.ToInt32(TileSide.East)] = reflectInto;
                    break;
                case TileSide.South:
                    newEdges[Convert.ToInt32(TileSide.North)] = reflectInto;
                    newEdges[Convert.ToInt32(TileSide.West)] = reflWest;
                    newEdges[Convert.ToInt32(TileSide.South)] = reflNorth;
                    newEdges[Convert.ToInt32(TileSide.East)] = reflEast;
                    break;
                case TileSide.East:
                    newEdges[Convert.ToInt32(TileSide.North)] = reflNorth;
                    newEdges[Convert.ToInt32(TileSide.West)] = reflectInto;
                    newEdges[Convert.ToInt32(TileSide.South)] = reflSouth;
                    newEdges[Convert.ToInt32(TileSide.East)] = reflWest;
                    break;
            }
            //"side" is going to be the new forward direction no matter what
            char lastDirection = newPath[newPath.Length - 1];
            bool hasFirstLeftOccurred;
            bool rightBeforeLeft;
            bool wasLastStepRight;
            if (lastDirection == 'N' || lastDirection == 'W' || lastDirection == 'S' || lastDirection == 'E')
            {
                // public bool hasFirstLeftOccurred;
                // public bool rightBeforeLeft;
                // public bool wasLastStepRight;
                hasFirstLeftOccurred = false;
                rightBeforeLeft = false;
                wasLastStepRight = false;
            }
            else if (lastDirection == 'R')
            {
                hasFirstLeftOccurred = this.hasFirstLeftOccurred;
                wasLastStepRight = true;
                rightBeforeLeft = true;
            }
            else if (lastDirection == 'L')
            {
                hasFirstLeftOccurred = true;
                wasLastStepRight = false;
                rightBeforeLeft = false;
            }
            //lastDirection == Forward
            else
            {
                hasFirstLeftOccurred = this.hasFirstLeftOccurred;
                wasLastStepRight = false;
                rightBeforeLeft = this.rightBeforeLeft;
            }
            Tile reflectedTile = new Tile(newEdges, side, newPath, hasFirstLeftOccurred, rightBeforeLeft, wasLastStepRight);
            return reflectedTile;
        }

        public override string ToString()
        {
            String res = "";
            foreach (var edge in this.edges)
            {
                res += $"Geodesic<{edge.ToString()}>,";
            }

            if (res.Length != 0)
            {
                res.Remove(res.Length - 1);
            }

            return res;
        }

        public bool Equals(Tile other)
        {
            bool equalSize = this.edges.Length == other.edges.Length;
            //I have no clue why this doesn't work the expected way
            //Why are the lists equal, but the set difference is not empty?
            //bool equalDifferentOrder = !this.edges.Except(other.edges).Any();
            
            //bool equalElements = this.edges.SequenceEqual(other.edges);
            //return equalElements;

            return this.EqualsDifferentOrder(other);
        }

        public bool EqualsDifferentOrder(Tile other)
        {
            foreach (var geo in other.edges)
            {
                if (!this.edges.Contains(geo))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tile)obj);
        }

        public override int GetHashCode()
        {
            return (edges != null ? edges.GetHashCode() : 0);
        }

        public bool isStepLegal(Step step)
        {
            switch (step)
            {
                case Step.Forward:
                    return true;
                case Step.Right:
                    return !this.wasLastStepRight;
                //step == Step.Left
                default:
                    return this.isLeftAllowed();
            }
        }
    }
}