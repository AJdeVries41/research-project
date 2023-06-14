using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace research_project
{
    public class HolonomyTiling : Tiling
    {
        //Kind of an arbitrary table, but you should read it as
        //StepToDirection[currentForwardDirection][step]
        //which, when cast to a Direction, gives the direction that "step" represents
        private static readonly int[,] StepToDirection = new int[4, 3] { { 0, 1, 3 }, { 1, 2, 0 }, { 2, 3, 1 }, { 3, 0, 2 } };

        public HolonomyTiling(int smallestResolution, double initialRotation) : base(4, 5, smallestResolution)
        {
            int d = CalculateInitialDistance(smallestResolution);
            var initialPoints = this.CalculateInitialPoints(d, initialRotation);
            var initialCircles = this.CalculateInitialCircles(initialPoints);
            this.InitialTile = new HolonomyTile(initialPoints, initialCircles);
            this.KnownTiles.Add(this.InitialTile);
        }
        
        
        
        public override void GenerateTiling()
        {
            Direction[] dirs = { Direction.N, Direction.W, Direction.S, Direction.E };
            Step[] steps = { Step.F, Step.L, Step.R };
            int NUM_DESIRED_TILES = 400;
            int generatedTiles = 0;
            Queue<HolonomyTile> q = new Queue<HolonomyTile>();
            HolonomyTile initial = (HolonomyTile) this.InitialTile;

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
            int dirInt = Convert.ToInt32(currentForwardDirection);
            int stepInt = Convert.ToInt32(s);
            return (Direction) StepToDirection[dirInt, stepInt];
        }
    }
}