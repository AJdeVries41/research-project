﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace research_project
{
    public class RegularTiling : Tiling
    {

        // Saves all relevant info for the tiling
        // Also constructs the initial tile
        public RegularTiling(int p, int q, int smallestResolution, double initialRotation) : base(p, q, smallestResolution)
        {
            
            int d = CalculateInitialDistance(smallestResolution);
            var initialPoints = this.CalculateInitialPoints(d, initialRotation);
            var initialCircles = this.CalculateInitialCircles(initialPoints);
            this.InitialTile = new Tile(initialPoints, initialCircles);
            this.KnownTiles.Add(this.InitialTile);
        }
        
        
        public override void GenerateTiling()
        {
            Queue<Tile> q = new Queue<Tile>();
            q.Enqueue(this.InitialTile);
            int NUM_ITERATIONS = 400;
            int iterationCount = 0;
            
            while (q.Count != 0 && iterationCount < NUM_ITERATIONS)
            {
                Tile current = q.Dequeue();
                for (int i = 0; i < current.Edges.Length; i++)
                {
                    Tile reflectedTile = current.ReflectIntoEdge(current.Edges[i].c);
                    this.KnownTiles.Add(reflectedTile);
                    q.Enqueue(reflectedTile);
                }
                iterationCount++;
            }
        }
    }
}