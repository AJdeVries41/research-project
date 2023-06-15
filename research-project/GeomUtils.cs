using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace research_project
{
    public class GeomUtils
    {

        public static bool NearlyEqual(double d1, double d2)
        {
            if (d1 == d2)
            {
                return true;
            }
            var epsilon = 0.0001;
            return Math.Abs(d1 - d2) <= epsilon;
        }

        public static (double, double) SolveQuadraticEquation(double a, double b, double c)
        {
            double discr = Math.Pow(b, 2) - 4 * a * c;
            double sol1 = (-b - Math.Sqrt(discr)) / (2 * a);
            double sol2 = (-b + Math.Sqrt(discr)) / (2 * a);
            return (sol1, sol2);
        }
        public static double Distance((double, double) p1, (double, double) p2)
        {
            var xDiff = p1.Item1 - p2.Item1;
            var yDiff = p1.Item2 - p2.Item2;
            return Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
        }

        public static (double, double) GetMiddlePoint((double, double) p1, (double, double) p2)
        {
            var sumX = p1.Item1 + p2.Item1;
            var sumY = p1.Item2 + p2.Item2;
            return (sumX / 2, sumY / 2);
        }

        public static Circle CircleWithCenterThroughPoint((double, double) center, (double, double) p)
        {
            var dist = Distance(center, p);
            return new Circle(center, dist);
        }

        public static (double, double) AddPoints((double, double) p1, (double, double) p2)
        {
            return (p1.Item1 + p2.Item1, p1.Item2 + p2.Item2);
        }
        
        /// <summary>
        /// Convert a point on the circle c to the angle phi from the centerpoint of c
        /// The angle returned is always in the domain [0,2pi]
        /// </summary>
        /// <param name="c">Circle with the centerpoint to consider</param>
        /// <param name="p">The point to convert</param>
        /// <returns>The angle in polar coordinates from the centerpoint of c in domain [0, 2pi]</returns>
        public static double ConvertToPolar(Circle c, (double, double) p)
        {
            var cX = c.CenterPoint.Item1;
            var cY = c.CenterPoint.Item2;
            var pX = p.Item1;
            var pY = p.Item2;
            var phi = Math.Atan2(pY - cY, pX - cX);
            return AngleConverter(phi);
        }
        
        //Converts an angle like -PI/4 to 7*PI/4
        //Positive angles remain the same
        public static double AngleConverter(double angle)
        {
            if (angle < 0)
            {
                return (2 * Math.PI) - Math.Abs(angle);
            }

            return angle;
        }

        /// <summary>
        /// Given a circle c and angle, convert that to Euclidean coordinates
        /// </summary>
        /// <param name="c"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static (double, double) ConvertFromPolar(Circle c, double angle)
        {
            double r = c.r;
            double centerToX = r * Math.Cos(angle);
            double centerToY = r * Math.Sin(angle);
            return AddPoints(c.CenterPoint, (centerToX, centerToY));
        }
        
        //see https://www.malinc.se/noneuclidean/en/circleinversion.php
        //Takes a point inside a circle and returns a point outside of the circle
        public static (double, double) InvertPoint((double, double) point, Circle c)
        {
            
            double x = point.Item1;
            double y = point.Item2;
            double centerX = c.CenterPoint.Item1;
            double centerY = c.CenterPoint.Item2;
            double r = c.r;
            
            double EPSILON = 0.000001;

            double centerToX = x - centerX;
            double centerToY = y - centerY;
            
            var hypot = Math.Sqrt(Math.Pow(centerToX, 2) + Math.Pow(centerToY, 2));

            if (Math.Abs(hypot) < EPSILON)
            {
                throw new SystemException("cannot invert centerpoint of a circl without a direction");
            }
            
            var phi = Math.Atan2(centerToY, centerToX);

            var distCenterToInversion = Math.Pow(r, 2) / hypot;

            var invX = centerX + (distCenterToInversion * Math.Cos(phi));
            var invY = centerY + (distCenterToInversion * Math.Sin(phi));

            return (invX, invY);
        }

        /// <summary>
        /// You cannot invert a centerpoint directly. So this method constructs a point in the direction of point inDirection
        /// Which is a really small distance away from the centerpoint
        /// </summary>
        /// <param name="c"></param>
        /// <param name="inDirection"></param>
        /// <returns></returns>
        public static (double, double) InvertCenterPoint(Circle c, (double, double) inDirection)
        {
            var dy = inDirection.Item2 - c.CenterPoint.Item2;
            var dx = inDirection.Item1 - c.CenterPoint.Item1;
            var EPSILON = 0.01;
            var toInvert = (c.CenterPoint.Item1 + EPSILON * dx, c.CenterPoint.Item2 + EPSILON * dy);
            return InvertPoint(toInvert, c);
        }
        
        


        public static Circle CircleFromThreePoints((double, double) p1, (double, double) p2, (double, double) p3)
        {
            double a = p1.Item1;
            double b = p1.Item2;
            double c = p2.Item1;
            double d = p2.Item2;
            double e = p3.Item1;
            double f = p3.Item2;
            
            double a1 = -2 * c + 2 * a;
            double b1 = -2 * d + 2 * b;
            double c1 = Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2) - Math.Pow(d, 2);

            double a2 = -2 * e + 2 * a;
            double b2 = -2 * f + 2 * b;
            double c2 = Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(e, 2) - Math.Pow(f, 2);

            //Solve system of equations using Cramer's rule

            //assert a1b2 - b1a2 != 0

            if (a1 * b2 - b1 * a2 == 0)
            {
                double centerXFake = (c1 * b2 - b1 * c2) / 0.0001;
                double centerYFake = (a1 * c2 - c1 * a2) / 0.0001;
                //calculate r using any equation, e.g. the first
                double rFake = Math.Sqrt(Math.Pow(a - centerXFake, 2) + Math.Pow(b - centerYFake, 2));
                return new Circle((centerXFake, centerYFake), rFake);
            }

            double centerX = (c1 * b2 - b1 * c2) / (a1 * b2 - b1 * a2);
            double centerY = (a1 * c2 - c1 * a2) / (a1 * b2 - b1 * a2);
            
            //calculate r using any equation, e.g. the first
            double r = Math.Sqrt(Math.Pow(a - centerX, 2) + Math.Pow(b - centerY, 2));

            return new Circle((centerX, centerY), r);
        }
        
        /// <summary>
        /// Construction 1.6 from "GoodmanStrauss"
        /// </summary>
        /// <param name="unitCircle"></param>
        /// <param name="A"></param>
        /// <returns></returns>
        public static Circle OrthogonalCircle(Circle unitCircle, (double, double) A, Graphics g)
        {
            var M = GetMiddlePoint(unitCircle.CenterPoint, A);
            double r1 = GeomUtils.Distance(M, unitCircle.CenterPoint);
            var c1 = new Circle(M, r1);
            g.DrawEllipse(Pens.Black, c1.GetRectangle());
            var intersections = c1.Intersect(unitCircle);
            //any of the 2 intersections is fine, since the resulting circle will pass thru both
            var intersection1 = intersections[0];
            var desiredR = GeomUtils.Distance(A, intersection1);
            var desiredCircle = new Circle(A, desiredR);
            return desiredCircle;
        }

        public static Line EuclideanLine((double, double) A, (double, double) B)
        {
            var dy = B.Item2 - A.Item2;
            var dx = B.Item1 - A.Item1;
            var slope = dy / dx;
            var intercept = (-slope * A.Item1) + A.Item2;
            return new Line(slope, intercept);
        }

        public static Circle HyperbolicBisectorFromCenter1((double, double) B, Circle unitCircle, Graphics g)
        {
            var invertCenter = InvertCenterPoint(unitCircle, B);
            var invB = InvertPoint(B, unitCircle);
            var line1 = EuclideanLine(unitCircle.CenterPoint, B);
            var line2 = EuclideanLine(invertCenter, invB);
            var intersection = line1.Intersect(line2);
            return OrthogonalCircle(unitCircle, intersection, g);
        }

        public static Circle HyperbolicBisectorFromCenter2((double, double) B, Circle unitCircle, Graphics g)
        {
            //To move the initial tile to the given point, we construct a hyperbolic bisector between
            //the origin and the given point. Then, we invert the entire initial tile within that bisector
            var M = GetMiddlePoint(unitCircle.CenterPoint, B);
            var circleThruB = new Circle(unitCircle.CenterPoint, GeomUtils.Distance(unitCircle.CenterPoint, B));
            var circleThruOrigin = new Circle(M, GeomUtils.Distance(M, unitCircle.CenterPoint));
            g.DrawEllipse(Pens.Green, circleThruB.GetRectangle());
            g.DrawEllipse(Pens.Green , circleThruOrigin.GetRectangle());
            var intersections = circleThruB.Intersect(circleThruOrigin);
            //Construct a circle that goes through both intersections and both inv(intersections)
            //This is your hyperbolic bisector
            var invIntersection1 = GeomUtils.InvertPoint(intersections[1], unitCircle);
            var resultingCircle = GeomUtils.CircleFromThreePoints(intersections[0], intersections[1], invIntersection1);
            g.DrawEllipse(Pens.Aqua, resultingCircle.GetRectangle());
            return resultingCircle;
        }
        
        
    }
}