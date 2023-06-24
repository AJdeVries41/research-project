namespace research_project
{
    public enum Step
    {
        //Forward
        F = 0,
        //Left
        L = 1,
        //Right
        R = 2,
        //Don't care (this happens when you generate the initial layer of new tiles, since those don't use steps yet)
        Dc = -1
    }
}