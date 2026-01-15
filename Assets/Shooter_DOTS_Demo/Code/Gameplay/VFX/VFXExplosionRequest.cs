using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.VFX;

namespace Shooter_DOTS_Demo.Code.Gameplay.VFX
{
    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    [StructLayout(LayoutKind.Sequential)]
    public struct VFXExplosionRequest
    {
        public Vector3 Position;
        public float Size;
    }
}