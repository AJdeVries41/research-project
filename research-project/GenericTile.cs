﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace research_project
{
    public class GenericTile : IEquatable<GenericTile>
    {
        public Geodesic[] edges;

        public GenericTile(Geodesic[] edges)
        {
            this.edges = edges;
        }
        
        //Constructor for the initial tile
        public GenericTile(List<(double, double)> points, List<Circle> circles)
        {
            //if there are p points that also means a polygon has p sides in this tiling
            this.edges = new Geodesic[points.Count];
            ConstructInitialEdges(points, circles);
        }
        
        public void DrawBounds(Graphics g)
        {
            foreach (var geo in this.edges)
            {
                if (geo != null)
                {
                    geo.Draw(g);
                }
            }
        }

        public void FillTile(Graphics g)
        {
            GraphicsPath gp = new GraphicsPath();
            for (int i = this.edges.Length - 1; i >= 0; i--)
            {
                gp.AddArc(this.edges[i].c.GetRectangle(), this.edges[i].startAngleDegree, this.edges[i].diffAngleDegree);
            }
            MD5 md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(this.ToString()));
            var color = Color.FromArgb(hash[0], hash[1], hash[2]);
            Brush b = new SolidBrush(color);
            g.FillRegion(b, new Region(gp));
        }
        
        
        
        public void ConstructInitialEdges(List<(double, double)> points, List<Circle> circles)
        {
            int j = 1;
            for (int i = 0; i < points.Count; i++)
            {
                Circle c = circles[i];
                
                //we are drawing the circle counterclockwise from the "end" point to the "start" point
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
        
        /// <summary>
        /// Reflects this tile into the given Geodesic
        /// </summary>
        /// <param name="reflectInto"></param>
        /// <returns></returns>
        public GenericTile ReflectIntoEdge(Geodesic reflectInto)
        {
            var newEdges = new Geodesic[this.edges.Length];
            //reflect all edges[j] into edges[i]
            for (int j = 0; j < this.edges.Length; j++)
            {
                Geodesic reflection = this.edges[j].ReflectIntoEdge(reflectInto);
                newEdges[j] = reflection;
            }
            GenericTile newTile = new GenericTile(newEdges);
            return newTile;
        }
        
        public override string ToString()
        {
            String res = "";
            foreach (var edge in this.edges)
            {
                res += edge.ToString();
            }

            if (res.Length != 0)
            {
                res.Remove(res.Length - 1);
            }

            return res;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GenericTile)obj);
        }

        public override int GetHashCode()
        {
            return (edges != null ? edges.GetHashCode() : 0);
        }

        public bool Equals(GenericTile other)
        {
            bool equalSize = this.edges.Length == other.edges.Length;
            //I have no clue why this doesn't work the expected way
            //Why are the lists equal, but the set difference is not empty?
            //bool equalDifferentOrder = !this.edges.Except(other.edges).Any();
            
            bool equalElements = this.edges.SequenceEqual(other.edges);
            return equalElements;
            
        }
    }
}