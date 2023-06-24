using System;
using System.Drawing;

namespace research_project
{
    public class GeomUtils
    {
        /// <summary>
        /// Returns true if the distance between the 2 points is at most EPSILON
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Euclidean distance between 2 points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Distance((double, double) p1, (double, double) p2)
        {
            var xDiff = p1.Item1 - p2.Item1;
            var yDiff = p1.Item2 - p2.Item2;
            return Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
        }

        /// <summary>
        /// Gets the middle point on the Euclidean line from p1 to p2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static (double, double) MidPoint((double, double) p1, (double, double) p2)
        {
            var sumX = p1.Item1 + p2.Item1;
            var sumY = p1.Item2 + p2.Item2;
            return (sumX / 2, sumY / 2);
        }

        /// <summary>
        /// Convert a A on the circle c to the angle phi from the centerpoint of c
        /// The angle returned is always in the domain [0,2pi]
        /// </summary>
        /// <param name="c">Circle with the centerpoint to consider</param>
        /// <param name="p">The A to convert</param>
        /// <returns>The angle in polar coordinates from the centerpoint of c in domain [0, 2pi]</returns>
        public static double ConvertToPolar(Circle c, (double, double) p)
        {
            var cX = c.CenterPoint.Item1;
            var cY = c.CenterPoint.Item2;
            var pX = p.Item1;
            var pY = p.Item2;
            var phi = Math.Atan2(pY - cY, pX - cX);
            if (phi < 0)
            {
                return (2 * Math.PI) - Math.Abs(phi);
            }
            return phi;
        }

        /// <summary>
        /// Inverts a A in the given circle
        /// </summary>
        /// <param name="A"></param>
        /// <param name="c">Circle with centerpoint O and radius r</param>
        /// <returns>a point B such that OA * OB = r^2</returns>
        public static (double, double) InvertPoint((double, double) A, Circle c)
        {
            double x = A.Item1;
            double y = A.Item2;
            double centerX = c.CenterPoint.Item1;
            double centerY = c.CenterPoint.Item2;
            double r = c.r;
            
            double EPSILON = 0.000001;
            double centerToX = x - centerX;
            double centerToY = y - centerY;
            var hypot = Math.Sqrt(Math.Pow(centerToX, 2) + Math.Pow(centerToY, 2));
            if (Math.Abs(hypot) < EPSILON)
            {
                hypot = EPSILON;
                //throw new SystemException("cannot invert centerpoint of a circl without a direction");
            }
            var phi = Math.Atan2(centerToY, centerToX);
            var distCenterToInversion = Math.Pow(r, 2) / hypot;
            var invX = centerX + (distCenterToInversion * Math.Cos(phi));
            var invY = centerY + (distCenterToInversion * Math.Sin(phi));
            return (invX, invY);
        }

        /// <summary>
        /// Construct a circle that goes through 3 points by solving a system of equations algebraically
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
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
        /// Shorthand for computing the line between 2 points in the Poincaré disk
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="unitCircle"></param>
        /// <returns></returns>
        public static Circle CircleBetweenPointsInDisk((double, double) a, (double, double) b, Circle unitCircle)
        {
            (double, double) invA = InvertPoint(a, unitCircle);
            return CircleFromThreePoints(a, b, invA);
        }
        
        /// <summary>
        /// Construction 1.6 from "GoodmanStrauss"
        /// </summary>
        /// <param name="unitCircle"></param>
        /// <param name="A">Point in the exterior of unitCircle</param>
        /// <returns></returns>
        public static Circle OrthogonalCircle(Circle unitCircle, (double, double) A)
        {
            var M = MidPoint(unitCircle.CenterPoint, A);
            //Construct a circle with M as centerpoint that passes thru origin
            double r1 = GeomUtils.Distance(M, unitCircle.CenterPoint);
            var c1 = new Circle(M, r1);
            var intersections = c1.Intersect(unitCircle);
            //any of the 2 intersections is fine, since the resulting circle will pass thru both
            var intersection1 = intersections[0];
            var desiredR = GeomUtils.Distance(A, intersection1);
            var desiredCircle = new Circle(A, desiredR);
            return desiredCircle;
        }

        /// <summary>
        /// Inverts a point B to get point invB in the given unitCircle, then constructs the OrhtogonalCircle
        /// of invB with unitCircle. This constructs a hyperbolic middle line between the origin point of the
        /// unit circle and point B
        /// </summary>
        /// <param name="B"></param>
        /// <param name="unitCircle"></param>
        /// <returns></returns>
        /// <exception cref="ArithmeticException"></exception>
        public static Circle HyperbolicBisectorFromCenter((double, double) B, Circle unitCircle)
        {
            if (Distance(unitCircle.CenterPoint, B) >= unitCircle.r)
            {
                throw new ArithmeticException("This is not a valid A to move the intial tile to");
            }
            double EPSILON = 0.0001;
            if (Distance(unitCircle.CenterPoint, B) <= EPSILON)
            {
                throw new ArithmeticException("Cannot construct a hyperbolic bisector for the centerpoint itself");
            }
            (double, double) invB = InvertPoint(B, unitCircle);
            return OrthogonalCircle(unitCircle, invB);
        }
    }
}