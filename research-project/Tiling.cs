using System;
using System.Collections.Generic;

namespace research_project
{
    public class Tiling
    {
        private int p;
        private int q;
        private int d;
        public Circle unitCircle;
        

        public Tiling(int p, int q, int smallestResolution)
        {
            if (!IsValidTiling(p, q))
            {
                throw new InvalidOperationException($"Invalid tiling of {{{p}, {q}}} given");
            }
            this.p = p;
            this.q = q;
            //see https://www.malinc.se/noneuclidean/en/poincaretiling.php why this.d is calculated like this
            var numerator = Math.Tan((Math.PI / 2) - (Math.PI / q)) - Math.Tan(Math.PI/p);
            var denominator = Math.Tan((Math.PI / 2) - (Math.PI / q)) + Math.Tan(Math.PI / p);
            double d = Math.Sqrt(numerator / denominator);
            this.d = (int) Math.Round(d * (smallestResolution / 2));
            this.unitCircle = new Circle(smallestResolution / 2, (0, 0));
        }

        public static bool IsValidTiling(int p, int q)
        {
            return (p-2)*(q-2) > 4;
        }
        
        /// <summary>
        /// Calculates the first p points a distance d from the origin
        /// The first point is always generated at x=d, y=0
        /// </summary>
        /// <param name="d"></param>
        /// <returns>A list of initial points from the origin point (0, 0)</returns>
        public List<(double, double)> InitialVertices(double initialRotation)
        {

            double angle = 2 * Math.PI / p;
            List<(double, double)> result = new List<(double, double)>();

            double curAngle = 0 + initialRotation;
            for (int i = 0; i < p; i++)
            {
                double x = d * Math.Cos(curAngle);
                double y = d * Math.Sin(curAngle);
                curAngle += angle;
                result.Add((x, y));
            }
            return result;
        }

        public List<Circle> InitialCircles(List<(double, double)> inits)
        {
            List<Circle> circles = new List<Circle>();
            //for each pair of adjacent points
            int j = 1;
            for (int i = 0; i < inits.Count; i++)
            {
                (double, double) inversion = GeomUtils.InvertPoint(inits[i], unitCircle);
                Circle connectingCircle = GeomUtils.CircleFromThreePoints(inversion, inits[i], inits[j]);
                circles.Add(connectingCircle);
                j++;
                if (j == inits.Count)
                {
                    j = 0;
                }
            }

            return circles;
        }
    }
}