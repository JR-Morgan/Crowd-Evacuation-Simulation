using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS_AI.Extensions.Tests
{
    [TestFixture]
    public class MathE_Tests
    {
        private const int I = 3;
        private const float LOWER = float.MinValue, UPPER = float.MaxValue;

        [Test]
        public void Angle_Test(
            [Random(LOWER, UPPER, I)] float a_x,
            [Random(LOWER, UPPER, I)] float a_y,
            [Random(LOWER, UPPER, I)] float a_z,
            [Random(LOWER, UPPER, I)] float b_x,
            [Random(LOWER, UPPER, I)] float b_y,
            [Random(LOWER, UPPER, I)] float b_z
        )
        {
            float expected = Vector3.Angle(new Vector3(a_x, a_y, a_z), new Vector3(b_x, b_y, b_z));
            float actual = MathE.Angle(new float3(a_x, a_y, a_z), new float3(b_x, b_y, b_z));
            Assert.AreEqual(expected, actual);
        }

    }
}
