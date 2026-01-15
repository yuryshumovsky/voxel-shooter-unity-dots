using System;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.Players.Configs
{
    [CreateAssetMenu(menuName = "DOTS Demo/Create players config", fileName = "PlayersConfig")]
    public class PlayersConfig : ScriptableObject
    {
        public int count;
        public int defaultDistanceToFigure;
        public CameraFollowConfig cameraFollowConfig;
    }

    [Serializable]
    public class CameraFollowConfig
    {
        public float backDistance;
        public float heightOffset;
    }
}