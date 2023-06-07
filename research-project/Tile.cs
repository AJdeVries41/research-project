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

        //reflect the edge circles[a] into circles[b]
        //returns the geodesic representing the new edge
        public Geodesic ReflectIntoEdge(int a, int b)
        {
            //We want to create 3 points such that we can return a circle that represents that an
            //edge was reflected into the b edge
            //to do this, we take 3 points of this edge, namely points[a], points[a+1] and the midpoint
            //and reflect all those into the b edge
            double rA = circles[a].r;
            (double, double) cA = circles[a].centerPoint;

            Circle reflectInto = circles[b];

            var startAngleOrig = angles[a].Item1;
            var diffAngleOrig = angles[a].Item3;

            var midAngleDegrees = startAngleOrig + (diffAngleOrig / 2);
            var midAngleRad = (Math.PI / 180) * midAngleDegrees;
            
            var midPoint = GeomUtils.AddPoints(cA, (rA * Math.Cos(midAngleRad), rA * Math.Sin(midAngleRad)));

            (double, double) reflectedPoint1 = GeomUtils.InvertPoint(points[a], reflectInto);
            (double, double) reflectedPoint2;
            //if at last edge, the next point is 0 and not IndexOutOfBounds
            if (a == points.Count - 1)
            {
                reflectedPoint2 = GeomUtils.InvertPoint(points[0], reflectInto);
            }
            else
            {
                reflectedPoint2 = GeomUtils.InvertPoint(points[a + 1], reflectInto);
            }
            (double, double) reflectedPoint3 = GeomUtils.InvertPoint(midPoint, reflectInto);

            var resultingCircle = GeomUtils.CircleFromThreePoints(reflectedPoint1, reflectedPoint2, reflectedPoint3);

            var angle1 = GeomUtils.ConvertToPolar(resultingCircle, reflectedPoint1);
            var angle2 = GeomUtils.ConvertToPolar(resultingCircle, reflectedPoint2);
            
            //always draw from the lower angle
            var fromAngle = Math.Min(angle1, angle2);
            var toAngle = Math.Max(angle1, angle2);
            double diffAngle;
            if ((toAngle - fromAngle) > Math.PI)
            {
                (fromAngle, toAngle) = (toAngle, fromAngle);
                diffAngle = (toAngle + 2 * Math.PI) - fromAngle;
            }
            else
            {
                diffAngle = toAngle - fromAngle;
            }

            var fromAngleDegree = (float) ((180 / Math.PI) * fromAngle);
            var diffAngleDegree = (float) ((180 / Math.PI) * diffAngle);

            Geodesic res = new Geodesic(resultingCircle, fromAngleDegree, diffAngleDegree);
            
            return res;

        }
        
        
    }
}