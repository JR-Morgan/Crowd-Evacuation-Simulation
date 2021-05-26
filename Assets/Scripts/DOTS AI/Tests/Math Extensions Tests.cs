using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS_AI.Extensions.Tests
{
    [TestFixture]
    public class MathE_Tests
    {
        private const int N = 7;
        private const float LOWER = float.MinValue, UPPER = float.MaxValue;

        [Test, Sequential]
        public void Angle_Test(
            [Random(LOWER, UPPER, N)] float a_x,
            [Random(LOWER, UPPER, N)] float a_y,
            [Random(LOWER, UPPER, N)] float a_z,
            [Random(LOWER, UPPER, N)] float b_x,
            [Random(LOWER, UPPER, N)] float b_y,
            [Random(LOWER, UPPER, N)] float b_z
        )
        {
            float expected = Vector3.Angle(new Vector3(a_x, a_y, a_z), new Vector3(b_x, b_y, b_z));
            float actual = MathE.Angle(new float3(a_x, a_y, a_z), new float3(b_x, b_y, b_z));
            Assert.AreEqual(expected, actual);
        }

    }
}
