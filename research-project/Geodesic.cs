using System;

namespace research_project
{
    /// <summary>
    /// The class that represents any edge of a tile
    /// A Geodesic is basically just a part of a circle that should be drawn, limited by the startPoint and endPoint
    /// which both lie on the circle
    /// </summary>
    public class Geodesic
    {
        public Circle c;
        public (double, double) startPoint;
        public (double, double) endPoint;

        /// <summary>
        /// Also specify from where to draw this circle, but in a different format since g.DrawArc requires the part of a circle to be drawn
        /// given in degrees.
        /// </summary>
        public float startAngleDegree;
        public float sweepAngleDegree;
        
        public Geodesic(Circle c, (double, double) start, (double, double) end)
        {
            this.c = c;
            this.startPoint = start;
            this.endPoint = end;
            this.startAngleDegree = (float) ((180/Math.PI) * GeomUtils.ConvertToPolar(this.c, this.startPoint));
            this.sweepAngleDegree = this.ComputeSweepAngle();
        }

        /// <summary>
        /// From the startpoint and endpoint, compute the difference in degrees from the startpoint to the endpoint
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Reflects this edge along another edge represented by the reflectionCircle of that edge
        /// </summary>
        /// <param name="reflectionCircle"></param>
        /// <returns>A new edge that is reflected into the other edge</returns>
        public Geodesic ReflectIntoEdge(Circle reflectionCircle, Circle unitCircle)
        {
            //For all reflections holds: the new start point will be the old end point
            //and the new end point will be the old start point
            var newStartPoint = GeomUtils.InvertPoint(this.endPoint, reflectionCircle);
            var newEndPoint = GeomUtils.InvertPoint(this.startPoint, reflectionCircle);

            var resultingCircle = GeomUtils.CircleBetweenPointsInDisk(newStartPoint, newEndPoint, unitCircle);

            Geodesic res = new Geodesic(resultingCircle, newStartPoint, newEndPoint);
            return res;
        }

        public override string ToString()
        {
            return $"{nameof(c)}: {c}, {nameof(startPoint)}: {startPoint}, {nameof(endPoint)}: {endPoint}";
        }
    }
}