using Unity.Mathematics;

namespace DOTS_AI.Extensions
{
    public static class MathE
    {
        public static float Angle(in float3 a, in float3 b) => math.acos(math.dot(a, b) / math.dot(math.lengthsq(a), math.lengthsq(b)));
    }
}