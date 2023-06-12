using System;
using System.Drawing;
using System.Dynamic;

namespace research_project
{
    public class Geodesic : IEquatable<Geodesic>
    {
        public Circle c;
        public float startAngleDegree;
        public float diffAngleDegree;

        //Construct Geodesic based on a circle and two points that lie on that circle
        public Geodesic(Circle c, (double, double) point1, (double, double) point2)
        {
            this.c = c;
            var angles = this.ComputeAngles(c, point1, point2);
            this.startAngleDegree = angles.Item1;
            this.diffAngleDegree = angles.Item2;
        }
        
        //Compute the start angle and sweep angle based on 2 points that lie on a circle
        //returns (startAngle, sweepAngle) as a tuple
        private (float, float) ComputeAngles(Circle c, (double, double) point1, (double, double) point2)
        {
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
            return (startAngleDegree, diffAngleDegree);
        }
        
        //reflect this edge into edge b
        //returns the geodesic representing the new edge
        public Geodesic ReflectIntoEdge(Geodesic b)
        {
            Circle c = this.c;
            double startAngleRad = (Math.PI / 180) * this.startAngleDegree;
            double midAngleRad = (Math.PI / 180) * (this.startAngleDegree + (this.diffAngleDegree / 2));
            double endAngleRad = (Math.PI / 180) * (this.startAngleDegree + this.diffAngleDegree);

            (double, double) startPoint = GeomUtils.ConvertFromPolar(c, startAngleRad);
            (double, double) midPoint = GeomUtils.ConvertFromPolar(c, midAngleRad);
            (double, double) endPoint = GeomUtils.ConvertFromPolar(c, endAngleRad);

            var reflectionCircle = b.c;

            var reflectStartPoint = GeomUtils.InvertPoint(startPoint, reflectionCircle);
            var reflectMidPoint = GeomUtils.InvertPoint(midPoint, reflectionCircle);
            var reflectEndPoint = GeomUtils.InvertPoint(endPoint, reflectionCircle);

            var resultingCircle = GeomUtils.CircleFromThreePoints(reflectStartPoint, reflectMidPoint, reflectEndPoint);

            Geodesic res = new Geodesic(resultingCircle, reflectStartPoint, reflectEndPoint);
            return res;
        }

        public void Draw(Graphics g)
        {
            try
            {
                g.DrawArc(Pens.Orange, this.c.GetRectangle(), this.startAngleDegree, this.diffAngleDegree);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"Got argument exception when trying to draw {this.ToString()}");
                //Don't draw this arc (this occurs seemingly at random, and I don't really know why)
                //Though I suppose it could have to do with drawing really small stuff near the border of the unit circle
                return;
            }
            
        }

        public override string ToString()
        {
            return $"Geodesic<{nameof(c)}: {c}, {nameof(startAngleDegree)}: {startAngleDegree}, {nameof(diffAngleDegree)}: {diffAngleDegree}>";
        }


        public bool Equals(Geodesic other)
        {
            return Equals(c, other.c)
                   && GeomUtils.NearlyEqual(this.startAngleDegree, other.startAngleDegree)
                   && GeomUtils.NearlyEqual(this.diffAngleDegree, other.diffAngleDegree);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Geodesic)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (c != null ? c.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ startAngleDegree.GetHashCode();
                hashCode = (hashCode * 397) ^ diffAngleDegree.GetHashCode();
                return hashCode;
            }
        }
    }
}