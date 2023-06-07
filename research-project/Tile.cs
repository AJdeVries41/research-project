using System;
using System.Collections.Generic;
using System.Drawing;

namespace research_project
{
    public class Tile
    {
        public List<Geodesic> edges;
        
        //Constructor for a general tile
        public Tile(List<Geodesic> edges)
        {
            this.edges = edges;
        }
        
        //Constructor for the initial tile
        public Tile(List<(double, double)> points, List<Circle> circles)
        {
            this.edges = new List<Geodesic>();
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
                this.edges.Add(edge);
                
                j++;
                if (j == points.Count)
                {
                    j = 0;
                }
                
            }
        }



    }
}