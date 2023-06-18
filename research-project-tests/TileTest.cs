using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using research_project;

namespace research_project_tests
{
    [TestFixture]
    public class TileTest
    {
        [Test]
        public void Test1()
        {
            Assert.True(true);
        }

        // [Test]
        // public void TestTileEquals()
        // {
        //     RegularTiling t = new RegularTiling(4, 5, 800, Math.PI / 4);
        //     Tile initial = new Tile(t.InitialPoints, t.InitialCircles);
        //     Geodesic[] otherEdges = new Geodesic[initial.Edges.Length];
        //     for (int i = 0; i < initial.Edges.Length; i++)
        //     {
        //         otherEdges[i] = initial.Edges[i];
        //     }
        //     (otherEdges[0], otherEdges[2]) = (otherEdges[2], otherEdges[0]);
        //     Tile initialDifferentOrder = new Tile(otherEdges);
        //     Assert.AreEqual(initial, initialDifferentOrder);
        // }
        //
        // [Test]
        // public void TestReflectOriginTwice()
        // {
        //     int resolution = 800;
        //
        //     RegularTiling t = new RegularTiling(4, 5, 800, Math.PI / 4);
        //     Tile initial = new Tile(t.InitialPoints, t.InitialCircles);
        //
        //     Geodesic topEdge = initial.Edges[0];
        //     
        //     //Reflecting twice in an edge should result in the original tile
        //     Tile reflection = initial.ReflectAlongEdge(topEdge);
        //     Tile reflectTwice = reflection.ReflectAlongEdge(topEdge);
        //
        //     for (int i = 0; i < initial.Edges.Length; i++)
        //     {
        //         bool result = initial.Edges[i].Equals(reflectTwice.Edges[i]);
        //         Console.WriteLine(result);
        //     }
        //
        //     // bool resultTest = initial.Equals(reflectTwice);
        //     // bool bool1 = initial.Edges.SequenceEqual(reflectTwice.Edges);
        //     // bool bool2 = initial.Edges.Equals(reflectTwice.Edges);
        //     // var setDifference = initial.Edges.Except(reflectTwice.Edges);
        //
        //     Assert.AreEqual(initial, reflectTwice);
        // }
    }
}