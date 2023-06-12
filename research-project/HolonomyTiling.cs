using System.Collections.Generic;

namespace research_project
{
    public class HolonomyTiling : Tiling
    {
        //public int[,] stepToDirection;

        public HolonomyTiling(int smallestResolution, double initialRotation) : base(4, 5, smallestResolution,
            initialRotation)
        {
            //this.stepToDirection = new int[4, 3] { { 0, 1, 3 }, { 1, 2, 0 }, { 2, 3, 1 }, { 3, 0, 2 } };
        }
        
        public override void GenerateTiling()
        {
            Direction[] dirs = { Direction.N, Direction.W, Direction.S, Direction.E };
            Step[] steps = { Step.F, Step.L, Step.R };
            int NUM_DESIRED_TILES = 400;
            int generatedTiles = 0;
            HolonomyTile initial = new HolonomyTile(this.InitialPoints, this.InitialCircles);
            this.KnownTiles.Add(initial);
            Queue<HolonomyTile> q = new Queue<HolonomyTile>();

            foreach (var dir in dirs)
            {
                HolonomyTile reflectedTile = initial.ReflectIntoDirection(dir, Step.DC);
                q.Enqueue(reflectedTile);
                this.KnownTiles.Add(reflectedTile);
            }
            
            // //Now we somehow have to generate only Forward, Left and Right tiles iff that is allowed according to the underlying graph
            while (q.Count != 0 && generatedTiles < NUM_DESIRED_TILES)
            {
                HolonomyTile current = q.Dequeue();
                Direction currentForwardDir = current.CurrentForwardDirection;
                //Reflect the tile into each of the "steps" which are currently allowed
                foreach (var step in steps)
                {
                    if (!current.IsStepLegal(step))
                    {
                        continue;
                    }
                    else
                    {
                        //Direction reflectIn = this.stepToDirection[currentForwardDir, step];
                        Direction reflectIn = ConvertStepToDirection(step, currentForwardDir);
                        HolonomyTile reflectedTile = current.ReflectIntoDirection(reflectIn, step);
                        generatedTiles++;
                        this.KnownTiles.Add(reflectedTile);
                        q.Enqueue(reflectedTile);
                    }
                }
            }
        }
        
        private static Direction ConvertStepToDirection(Step s, Direction currentForwardDirection)
        {
            switch (currentForwardDirection)
            {
                case Direction.N:
                    switch (s)
                    {
                        case Step.F:
                            return Direction.N;
                        case Step.L:
                            return Direction.W;
                        case Step.R:
                            return Direction.E;
                    }
                    break;
                case Direction.W:
                    switch (s)
                    {
                        case Step.F:
                            return Direction.W;
                        case Step.L:
                            return Direction.S;
                        case Step.R:
                            return Direction.N;
                    }

                    break;
                case Direction.S:
                    switch (s)
                    {
                        case Step.F:
                            return Direction.S;
                        case Step.L:
                            return Direction.E;
                        case Step.R:
                            return Direction.W;
                    }
                    break;
                case Direction.E:
                    switch (s)
                    {
                        case Step.F:
                            return Direction.E;
                        case Step.L:
                            return Direction.N;
                        case Step.R:
                            return Direction.S;
                    }
                    break;
            }
            //this never happens
            return Direction.E;
        }
    }
}