// using System;
// using System.Collections.Generic;
// using System.Drawing;
//
// namespace research_project
// {
//     public abstract class Tiling
//     {
//         protected int P { get; set; }
//         protected int Q { get; set; }
//         public Circle UnitCircle { get; set; }
//         
//         public Tile InitialTile;
//         protected List<Tile> KnownTiles;
//
//         public Tiling()
//         {
//             
//         }
//
//         public Tiling(int p, int q, int smallestResolution, double initialRotation)
//         {
//             if (!IsValidTiling(p, q))
//             {
//                 throw new InvalidOperationException($"Invalid tiling of {{{p}, {q}}} given");
//             }
//             this.P = p;
//             this.Q = q;
//             this.UnitCircle = new Circle((0, 0), smallestResolution / 2);
//             this.KnownTiles = new List<Tile>();
//         }
//
//        
//     }
//     
//     
// }