using System;
using System.Collections.Generic;
using System.Drawing;

namespace research_project
{
    public class Tile
    {
        //all of these fields are ordered
        //points[0] and points[1] have circles[0] and angles[0]
        //points[1] and points[2] have circles[1] and angles[1]
        //etc..
        public List<(double, double)> points;
        public List<Circle> circles;
        //angles saves startAngle, endAngle, diffAngle
        public List<(float, float, float)> angles;
        

        public Tile(List<(double, double)> points, List<Circle> circles)
        {
            this.points = points;
            this.circles = circles;
            this.angles = new List<(float, float, float)>();
            this.SetAngles();
        }

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
        
        
    }
}