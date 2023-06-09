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
        //     Tiling t = new Tiling(4, 5, 800, Math.PI / 4);
        //     Tile initial = new Tile(t.initialPoints, t.initialCircles);
        //     Geodesic[] otherEdges = new Geodesic[initial.edges.Length];
        //     for (int i = 0; i < initial.edges.Length; i++)
        //     {
        //         otherEdges[i] = initial.edges[i];
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
        //     Tiling t = new Tiling(4, 5, 800, Math.PI / 4);
        //     Tile initial = new Tile(t.initialPoints, t.initialCircles);
        //
        //     Geodesic topEdge = initial.edges[0];
        //     
        //     //Reflecting twice in an edge should result in the original tile
        //     Tile reflection = initial.ReflectIntoEdge(topEdge);
        //     Tile reflectTwice = reflection.ReflectIntoEdge(topEdge);
        //
        //     for (int i = 0; i < initial.edges.Length; i++)
        //     {
        //         bool result = initial.edges[i].Equals(reflectTwice.edges[i]);
        //         Console.WriteLine(result);
        //     }
        //
        //     // bool resultTest = initial.Equals(reflectTwice);
        //     // bool bool1 = initial.edges.SequenceEqual(reflectTwice.edges);
        //     // bool bool2 = initial.edges.Equals(reflectTwice.edges);
        //     // var setDifference = initial.edges.Except(reflectTwice.edges);
        //
        //     Assert.AreEqual(initial, reflectTwice);
        // }
    }
}