namespace research_project
{
    public enum Direction
    {
        //North
        N = 0,
        //West
        W = 1,
        //South
        S = 2,
        //East
        E = 3,
        //Origin, which is used when we don't reflect in any direction, because it's the origin tile
        O = -1
    }

    static class DirectionUtils
    {
        public static Direction Opposite(this Direction dir)
        {
            switch (dir)
            {
                case Direction.N:
                    return Direction.S;
                case Direction.W:
                    return Direction.E;
                case Direction.S:
                    return Direction.N;
                case Direction.E:
                    return Direction.W;
            }
            return Direction.O;
        }

        public static (Direction, Direction) Orthogonals(this Direction dir)
        {
            switch (dir)
            {
                case Direction.N:
                    return (Direction.W, Direction.E);
                case Direction.W:
                    return (Direction.N, Direction.S);
                case Direction.S:
                    return (Direction.W, Direction.E);
                case Direction.E:
                    return (Direction.N, Direction.S);
            }
            return (Direction.O, Direction.O);
        }
    }
}