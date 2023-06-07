namespace research_project
{
    public class Geodesic
    {
        public Circle c;
        public float startAngle;
        public float diffAngle;

        public Geodesic(Circle c, float startAngle, float diffAngle)
        {
            this.c = c;
            this.startAngle = startAngle;
            this.diffAngle = diffAngle;
        }
    }
}