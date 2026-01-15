using System.Collections.Generic;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Configs
{
    [CreateAssetMenu(menuName = "DOTS Demo/Create figure config", fileName = "FigureConfig")]
    public class FigureConfig : ScriptableObject
    {
        public List<Color> colors;
        public float blockSize;
        public Vector3Int figureSize;
    }
}