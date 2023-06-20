// using System;
// using System.Collections.Generic;
// using System.Drawing;
// using System.Drawing.Drawing2D;
// using System.Linq;
// using System.Security.Cryptography;
// using System.Text;
//
// namespace research_project
// {
//     public class Tile : IEquatable<Tile>
//     {
//         public Geodesic[] Edges;
//
//         public Tile(Geodesic[] edges)
//         {
//             this.Edges = edges;
//         }
//         
//         /// <summary>
//         /// Constructor for the initial tile
//         /// </summary>
//         /// <param name="points"></param>
//         /// <param name="circles"></param>
//         public Tile(List<(double, double)> points, List<Circle> circles)
//         {
//             //if there are p points that also means a polygon has p sides in this tiling
//             this.Edges = new Geodesic[points.Count];
//             ConstructInitialEdges(points, circles);
//         }
//
//         public void DrawBounds(Graphics g)
//         {
//             Pen drawingPen = new Pen(Brushes.Black, 1);
//             foreach (var geo in this.Edges)
//             {
//                 if (geo != null)
//                 {
//                     g.DrawArc(drawingPen,geo.c.GetRectangle(), geo.startAngleDegree, geo.sweepAngleDegree);
//                 }
//             }
//         }
//
//         public virtual void FillTile(Graphics g)
//         {
//             GraphicsPath gp = new GraphicsPath();
//             for (int i = 0; i < this.Edges.Length; i++)
//             {
//                 gp.AddArc(this.Edges[i].c.GetRectangle(), this.Edges[i].startAngleDegree, this.Edges[i].sweepAngleDegree);
//             }
//             MD5 md5 = MD5.Create();
//             var hashCode = this.GetHashCode();
//             var bytes = BitConverter.GetBytes(hashCode);
//             var hash = md5.ComputeHash(bytes);
//             var color = Color.FromArgb(hash[0], hash[1], hash[2]);
//             Brush b = new SolidBrush(color);
//             g.FillRegion(b, new Region(gp));
//         }
//         
//         
//         
//         private void ConstructInitialEdges(List<(double, double)> points, List<Circle> circles)
//         {
//             int j = 1;
//             for (int i = 0; i < points.Count; i++)
//             {
//                 Circle c = circles[i];
//                 
//                 var start = points[i];
//                 var dest = points[j];
//
//                 Geodesic edge = new Geodesic(c, start, dest);
//                 this.Edges[i] = edge;
//                 
//                 j++;
//                 if (j == points.Count)
//                 {
//                     j = 0;
//                 }
//                 
//             }
//         }
//         
//         /// <summary>
//         /// Reflects this tile into the given Geodesic
//         /// </summary>
//         /// <param name="reflectionCircle">Circle of the edge in which the reflection takes place</param>
//         /// <returns></returns>
//         public Tile ReflectIntoEdge(Circle reflectionCircle)
//         {
//             var newEdges = new Geodesic[this.Edges.Length];
//             //reflect all Edges[j] into Edges[i]
//             for (int j = 0; j < this.Edges.Length; j++)
//             {
//                 Geodesic reflection = this.Edges[j].ReflectIntoEdge(reflectionCircle);
//                 newEdges[j] = reflection;
//             }
//             Tile newTile = new Tile(newEdges);
//             return newTile;
//         }
//         
//         public override string ToString()
//         {
//             String res = "";
//             foreach (var edge in this.Edges)
//             {
//                 res += edge.ToString();
//             }
//
//             if (res.Length != 0)
//             {
//                 res.Remove(res.Length - 1);
//             }
//
//             return res;
//         }
//
//         public override bool Equals(object obj)
//         {
//             if (ReferenceEquals(null, obj)) return false;
//             if (ReferenceEquals(this, obj)) return true;
//             if (obj.GetType() != this.GetType()) return false;
//             return Equals((Tile)obj);
//         }
//
//         public override int GetHashCode()
//         {
//             return (Edges != null ? Edges.GetHashCode() : 0);
//         }
//
//         public bool Equals(Tile other)
//         {
//             bool equalSize = this.Edges.Length == other.Edges.Length;
//             //I have no clue why this doesn't work the expected way
//             //Why are the lists equal, but the set difference is not empty?
//             //bool equalDifferentOrder = !this.Edges.Except(other.Edges).Any();
//             
//             bool equalElements = this.Edges.SequenceEqual(other.Edges);
//             return equalElements;
//             
//         }
//     }
// }