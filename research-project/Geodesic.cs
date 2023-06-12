using System;
using System.Drawing;
using System.Dynamic;

namespace research_project
{
    public class Geodesic
    {
        public Circle c;
        public (double, double) startPoint;
        public (double, double) endPoint;

        public float startAngleDegree;
        public float sweepAngleDegree;

        //Construct Geodesic based on a circle and two points that lie on that circle
        public Geodesic(Circle c, (double, double) start, (double, double) end)
        {
            this.c = c;
            this.startPoint = start;
            this.endPoint = end;
            this.startAngleDegree = (float) ((180/Math.PI) * GeomUtils.ConvertToPolar(this.c, this.startPoint));
            this.sweepAngleDegree = this.ComputeSweepAngle();
        }

        private float ComputeSweepAngle()
        {
            var startAngle = GeomUtils.ConvertToPolar(this.c, this.startPoint);
            var endAngle = GeomUtils.ConvertToPolar(this.c, this.endPoint);
            double sweepAngle;
            //Case 1: start is in Quadrant 4 and end is in Quadrant 1
            if (startAngle >= (3 * Math.PI / 2) && startAngle <= 2 * Math.PI && endAngle >= 0 &&
                endAngle <= (Math.PI / 2))
            {
                sweepAngle = ((2 * Math.PI + endAngle) - startAngle);
            }
            //Case 2: start is in Quadrant 1 and end is in Quadrant 4
            //Since we draw from start to end, we are now drawing in opposite direction,
            //which means we need to append a "-" before the angle
            else if (startAngle >= 0 && startAngle <= (Math.PI / 2) && endAngle >= (3 * Math.PI / 2) &&
                     endAngle <= 2 * Math.PI)
            {
                sweepAngle = -((2 * Math.PI + startAngle) - endAngle);
            }
            //Otherwise, just subtract the smallest from the largest
            //if the largest is end, don't put a "-" in front
            //if the largest is start, put a "-" in front
            //Case 3
            else if (endAngle > startAngle)
            {
                sweepAngle = (endAngle - startAngle);
            }
            //startAngle > endAngle
            else
            {
                sweepAngle = -(startAngle - endAngle);
            }
            return (float) ((180/Math.PI) * sweepAngle);
        }

        private (double, double) GenerateMidPoint()
        {
            double startAngleRad = GeomUtils.ConvertToPolar(this.c, this.startPoint);
            double endAngleRad = GeomUtils.ConvertToPolar(this.c, this.endPoint);

            var minAngle = Math.Min(startAngleRad, endAngleRad);
            var maxAngle = Math.Max(startAngleRad, endAngleRad);
            double diffAngleRad;
            if ((maxAngle - minAngle) > Math.PI)
            {
                (minAngle, maxAngle) = (maxAngle, minAngle);
                diffAngleRad = (maxAngle + 2 * Math.PI) - minAngle;
            }
            else
            {
                diffAngleRad = maxAngle - minAngle;
            }

            var midAngleRad = startAngleRad + (diffAngleRad / 2);
            (double, double) midPoint = GeomUtils.ConvertFromPolar(this.c, midAngleRad);
            return midPoint;
        }
        
        /// <summary>
        /// Reflects this edge into another edge represented by the reflectionCircle of that edge
        /// </summary>
        /// <param name="reflectionCircle"></param>
        /// <returns>A new edge that is reflected into the other edge</returns>
        public Geodesic ReflectIntoEdge(Circle reflectionCircle)
        {
            (double, double) midPoint = GenerateMidPoint();

            //For all reflections holds: the new start point will be the old end point
            //and the new end point will be the old start point
            var newStartPoint = GeomUtils.InvertPoint(this.endPoint, reflectionCircle);
            var newMidPoint = GeomUtils.InvertPoint(midPoint, reflectionCircle);
            var newEndPoint = GeomUtils.InvertPoint(this.startPoint, reflectionCircle);

            var resultingCircle = GeomUtils.CircleFromThreePoints(newStartPoint, newMidPoint, newEndPoint);

            Geodesic res = new Geodesic(resultingCircle, newStartPoint, newEndPoint);
            return res;
        }

        public override string ToString()
        {
            return $"{nameof(c)}: {c}, {nameof(startPoint)}: {startPoint}, {nameof(endPoint)}: {endPoint}";
        }
    }
}