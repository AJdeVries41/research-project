﻿using System;
using System.Drawing;
using System.Dynamic;

namespace research_project
{
    public class Geodesic
    {
        public Circle c;
        public float startAngleDegree;
        public float diffAngleDegree;

        public Geodesic(Circle c, float startAngleDegree, float diffAngleDegree)
        {
            this.c = c;
            this.startAngleDegree = startAngleDegree;
            this.diffAngleDegree = diffAngleDegree;
        }

        //Construct Geodesic based on a circle and two points that lie on that circle
        public Geodesic(Circle c, (double, double) point1, (double, double) point2)
        {
            this.c = c;
            this.ComputeAngles(point1, point2);
        }

        public void Draw(Graphics g)
        {
            g.DrawArc(Pens.Aquamarine, this.c.GetRectangle(), this.startAngleDegree, this.diffAngleDegree);
        }

        //Compute which part of the geodesic should be drawn based on two points that are on the circle
        private void ComputeAngles((double, double) point1, (double, double) point2)
        {
            var cX = this.c.centerPoint.Item1;
            var cY = this.c.centerPoint.Item2;

            var angle1 = GeomUtils.ConvertToPolar(c, point1);
            var angle2 = GeomUtils.ConvertToPolar(c, point2);
            
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
            var startAngleDegree = (float) ((180 / Math.PI) * fromAngle);
            var diffAngleDegree = (float) ((180 / Math.PI) * diffAngle);
            this.startAngleDegree = startAngleDegree;
            this.diffAngleDegree = diffAngleDegree;
        }
        
        //reflect the edge a into edge b
        //returns the geodesic representing the new edge
        public static Geodesic ReflectIntoEdge(Geodesic a, Geodesic b)
        {
            //We want to create 3 points such that we can return a circle that represents that an
            //edge was reflected into the b edge
            //to do this, we take 3 points of this edge, namely points[a], points[a+1] and the midpoint
            //and reflect all those into the b edge

            Circle c = a.c;
            double startAngleRad = (Math.PI / 180) * a.startAngleDegree;
            double midAngleRad = (Math.PI / 180) * (a.startAngleDegree + (a.diffAngleDegree / 2));
            double endAngleRad = (Math.PI / 180) * (a.startAngleDegree + a.diffAngleDegree);

            (double, double) startPoint = GeomUtils.ConvertFromPolar(c, startAngleRad);
            (double, double) midPoint = GeomUtils.ConvertFromPolar(c, midAngleRad);
            (double, double) endPoint = GeomUtils.ConvertFromPolar(c, endAngleRad);

            var reflectionCircle = b.c;

            var reflectPoint1 = GeomUtils.InvertPoint(startPoint, reflectionCircle);
            var reflectPoint2 = GeomUtils.InvertPoint(midPoint, reflectionCircle);
            var reflectPoint3 = GeomUtils.InvertPoint(endPoint, reflectionCircle);

            var resultingCircle = GeomUtils.CircleFromThreePoints(reflectPoint1, reflectPoint2, reflectPoint3);

            var angle1 = GeomUtils.ConvertToPolar(resultingCircle, reflectPoint1);
            var angle2 = GeomUtils.ConvertToPolar(resultingCircle, reflectPoint3);
            
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