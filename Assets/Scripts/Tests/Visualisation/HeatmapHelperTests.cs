using NUnit.Framework;
using System.Linq;
using UnityEngine;
using RangeAttribute = NUnit.Framework.RangeAttribute;

namespace PedestrianSimulation.Visualisation.Tests
{

    [TestFixture]
    public class HeatmapHelperTests
    {
        //[Test]
        //public void ReturnSize_Test([Random(0, HeatmapHelper.MAX_ARRAY_SIZE, 10)] int size)
        //{
        //    Vector4[] r = HeatmapHelper.GeneratePositionArray(new Transform[0], size);
        //    Assert.AreEqual(r.Length, size);
        //}

        [TestCase(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f)]
        [TestCase(2f, 16f, 128f, 4f, 32f, 256f, 8f, 64f, 512f)]
        [TestCase(0.2f, 0.16f, 0.128f, 0.4f, 0.32f, 0.256f, 0.8f, 0.64f, 0.512f, 3)]
        [Test]
        public void ReturnValues_Test(
            float x0, float y0, float z0,
            float x1, float y1, float z1,
            float x2, float y2, float z2
            )
        {
            GameObject g1 = new GameObject(), g2 = new GameObject(), g0 = new GameObject();
            Vector3 p0 = g0.transform.position = new Vector3(x0, y0, z0);
            Vector3 p1 = g1.transform.position = new Vector3(x1, y1, z1);
            Vector3 p2 = g2.transform.position = new Vector3(x2, y2, z2);

            Transform[] transforms = new Transform[] { g0.transform, g1.transform, g2.transform };

            Vector4[] r = ShaderHelper.ToHomogeneousCoordinates(transforms).ToArray();

            Vector4 r0 = r[0], r1 = r[1], r2 = r[2];

            Assert.AreEqual(r0.x, p0.x, "x0");
            Assert.AreEqual(r0.y, p0.y, "y0");
            Assert.AreEqual(r0.z, p0.z, "z0");
            Assert.AreEqual(r0.w, 1,    "w0");

            Assert.AreEqual(r1.x, p1.x, "x1");
            Assert.AreEqual(r1.y, p1.y, "y1");
            Assert.AreEqual(r1.z, p1.z, "z1");
            Assert.AreEqual(r1.w, 1,    "w2");

            Assert.AreEqual(r2.x, p2.x, "x2");
            Assert.AreEqual(r2.y, p2.y, "y2");
            Assert.AreEqual(r2.z, p2.z, "z2");
            Assert.AreEqual(r2.w, 1,    "w2");
        }

    }
}