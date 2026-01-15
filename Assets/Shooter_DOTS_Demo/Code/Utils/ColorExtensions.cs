using Unity.Mathematics;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Utils
{
    public static class ColorExtensions
    {
        public static float4 ToFloat4(this Color color) => new float4(color.r, color.g, color.b, color.a);
        public static Color ToColor(this float4 f) => new Color(f.x, f.y, f.z, f.w);
    }
}