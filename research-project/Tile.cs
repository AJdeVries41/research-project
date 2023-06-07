using System;
using System.Collections.Generic;
using System.Drawing;

namespace research_project
{
    public class Tile
    {
        public List<Geodesic> edges;
        //all of these fields are ordered
        //points[0] and points[1] have circles[0] and angles[0]
        //points[1] and points[2] have circles[1] and angles[1]
        //etc..
        public List<(double, double)> points;
        public List<Circle> circles;
        //angles saves startAngle, endAngle, diffAngle
        public List<(float, float, float)> angles;
        
        
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
                g.DrawArc(Pens.Red, geo.c.GetRectangle(), geo.startAngleDegree, geo.diffAngleDegree);
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
                var startAngle = GeomUtils.ConvertToPolar(c, start);
                var destAngle = GeomUtils.ConvertToPolar(c, dest);
                
                if (destAngle < startAngle)
                {
                    destAngle += 2 * Math.PI;
                }
                
                //we can now assume that destAngle is necessarily larger than startAngle
                var diffAngle = destAngle - startAngle;
                float degreeStartAngle = (float) ((180/Math.PI) * startAngle);
                float degreeDiffAngle = (float) ((180 / Math.PI) * diffAngle);

                Geodesic edge = new Geodesic(c, degreeStartAngle, degreeDiffAngle);
                this.edges.Add(edge);
                
                j++;
                if (j == points.Count)
                {
                    j = 0;
                }
            }
        }

        // public Tile(List<(double, double)> points, List<Circle> circles)
        // {
        //     this.points = points;
        //     this.circles = circles;
        //     this.angles = new List<(float, float, float)>();
        //     this.SetAngles();
        // }

        public void SetAngles()
        {
            //Draw the initial tile, based on the initial points
            //for each pair of adjacent points, draw an arc counterclockwise from inits[j] to inits[i]
            int j = 1;
            for (int i = 0; i < points.Count; i++)
            {
                var circleCentreX = circles[i].centerPoint.Item1;
                var circleCentreY = circles[i].centerPoint.Item2;
                //we are drawing the circle counterclockwise from the 2nd point to the 1st point
                var startX = points[j].Item1;
                var startY = points[j].Item2;
                var destX = points[i].Item1;
                var destY = points[i].Item2;

                var startAngle = GeomUtils.AngleConverter(Math.Atan2(startY - circleCentreY, startX - circleCentreX));
                var destAngle = GeomUtils.AngleConverter(Math.Atan2(destY - circleCentreY, destX - circleCentreX));
                //like if startAngle is 7*pi/6 and destAngle is pi/4 (you cross over the 2*pi part)
                if (destAngle < startAngle)
                {
                    destAngle += 2 * Math.PI;
                }
                //we can now assume that destAngle is necessarily larger than startAngle
                var diffAngle = destAngle - startAngle;
                float degreeStartAngle = (float) ((180/Math.PI) * startAngle);
                float degreeDestAngle = (float)((180 / Math.PI) * destAngle);
                float degreeDiffAngle = (float) ((180 / Math.PI) * diffAngle);
                
                this.angles.Add((degreeStartAngle, degreeDestAngle, degreeDiffAngle));

                //g.DrawArc(Pens.Red, circles[i].GetRectangle(), degreeStartAngle, degreeDiffAngle);
                
                j++;
                if (j == points.Count)
                {
                    j = 0;
                }
            }
        }
        
        //reflect a into b
        // public static Geodesic ReflectIntoGeodesic(Geodesic a, Geodesic b)
        // {
        //     
        // }

        
        
        
    }
}