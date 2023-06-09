using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace research_project
{
    public class Tile : IEquatable<Tile>
    {
        //public List<Geodesic> edges;
        public Geodesic[] edges;
        
        //Constructor for a general tile
        public Tile(Geodesic[] edges)
        {
            this.edges = edges;
        }
        
        //Constructor for the initial tile
        public Tile(List<(double, double)> points, List<Circle> circles)
        {
            //if there are p points that also means a polygon has p sides
            this.edges = new Geodesic[points.Count];
            ConstructInitialEdges(points, circles);
        }

        public void Draw(Graphics g)
        {
            foreach (var geo in this.edges)
            {
                geo.Draw(g);
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
    }
}