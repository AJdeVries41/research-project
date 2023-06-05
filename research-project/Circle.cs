using System.Drawing;

namespace research_project
{
    public class Circle
    {
        public double r { get; }
        public (double, double) centerPoint { get; }

        public Circle(double r, (double, double) p)
        {
            this.r = r;
            this.centerPoint = p;
        }
        
        
    }
}