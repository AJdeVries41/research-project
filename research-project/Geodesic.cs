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
        
        //Compute which part of the circle should be drawn based on two points that are on the circle
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

        public void Draw(Graphics g)
        {
            g.DrawArc(Pens.Orange, this.c.GetRectangle(), this.startAngleDegree, this.diffAngleDegree);
        }

        //reflect this edge into edge b
        //returns the geodesic representing the new edge
        public Geodesic ReflectIntoEdge(Geodesic b)
        {
            //We want to create 3 points such that we can return a circle that represents that an
            //edge was reflected into the b edge
            //to do this, we take 3 points of this edge, namely points[a], points[a+1] and the midpoint
            //and reflect all those into the b edge

            Circle c = this.c;
            double startAngleRad = (Math.PI / 180) * this.startAngleDegree;
            double midAngleRad = (Math.PI / 180) * (this.startAngleDegree + (this.diffAngleDegree / 2));
            double endAngleRad = (Math.PI / 180) * (this.startAngleDegree + this.diffAngleDegree);

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

        public override string ToString()
        {
            return $"{nameof(c)}: {c}, {nameof(startAngleDegree)}: {startAngleDegree}, {nameof(diffAngleDegree)}: {diffAngleDegree}";
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