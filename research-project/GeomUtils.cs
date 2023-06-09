using System;

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
        public static double Distance((double, double) p1, (double, double) p2)
        {
            var xDiff = p1.Item1 - p2.Item1;
            var yDiff = p1.Item2 - p2.Item2;
            return Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
        }

        public static (double, double) AddPoints((double, double) p1, (double, double) p2)
        {
            return (p1.Item1 + p2.Item1, p1.Item2 + p2.Item2);
        }

        //Convert a point on the circle c to the relative angle phi from that centerpoint
        //Returns an angle [0, 2*pi]
        public static double ConvertToPolar(Circle c, (double, double) p)
        {
            var cX = c.centerPoint.Item1;
            var cY = c.centerPoint.Item2;
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

        public static (double, double) ConvertFromPolar(Circle c, double angle)
        {
            double cX = c.centerPoint.Item1;
            double cY = c.centerPoint.Item2;
            double r = c.r;
            double centerToX = r * Math.Cos(angle);
            double centerToY = r * Math.Sin(angle);
            return AddPoints(c.centerPoint, (centerToX, centerToY));
        }
        
        //see https://www.malinc.se/noneuclidean/en/circleinversion.php
        //Takes a point inside a circle and returns a point outside of the circle
        public static (double, double) InvertPoint((double, double) point, Circle c)
        {
            
            double x = point.Item1;
            double y = point.Item2;
            double centerX = c.centerPoint.Item1;
            double centerY = c.centerPoint.Item2;
            double r = c.r;
            
            double EPSILON = 0.000001;

            double centerToX = x - centerX;
            double centerToY = y - centerY;
            
            var hypot = Math.Sqrt(Math.Pow(centerToX, 2) + Math.Pow(centerToY, 2));

            if (Math.Abs(hypot) < EPSILON)
            {
                throw new ArithmeticException(
                    "Cannot invert a centerPoint of a circle (this would result in division by zero)");
            }
            
            var phi = Math.Atan2(centerToY, centerToX);

            var distCenterToInversion = Math.Pow(r, 2) / hypot;

            var invX = centerX + (distCenterToInversion * Math.Cos(phi));
            var invY = centerY + (distCenterToInversion * Math.Sin(phi));

            return (invX, invY);
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
                throw new SystemException("a1*b2 - b2*a2 = 0. This would return in division by zero");
            }

            double centerX = (c1 * b2 - b1 * c2) / (a1 * b2 - b1 * a2);
            double centerY = (a1 * c2 - c1 * a2) / (a1 * b2 - b1 * a2);

            

            //calculate r using any equation, e.g. the first
            double r = Math.Sqrt(Math.Pow(a - centerX, 2) + Math.Pow(b - centerY, 2));

            return new Circle(r, (centerX, centerY));
        }
        
        
    }
}