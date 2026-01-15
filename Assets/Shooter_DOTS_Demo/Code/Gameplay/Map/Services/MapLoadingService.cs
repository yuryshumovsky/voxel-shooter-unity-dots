using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Shooter_DOTS_Demo.Code.Gameplay.Map.Services
{
    public class MapLoadingService
    {
        public Dictionary<int3, byte> LoadVoxelMap(string fileName)
        {
            Dictionary<int3, byte> Voxels = new Dictionary<int3, byte>();

            TextAsset file = Resources.Load<TextAsset>(fileName);
            if (file == null)
            {
                Debug.LogError($"File {fileName} not found in Resources!");
                return null;
            }

            string[] lines = file.text.Split('\n');

            foreach (string lineRaw in lines)
            {
                string line = lineRaw.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("#")) continue;

                string[] parts = line.Split(',');
                if (parts.Length != 4) continue;

                int x = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);
                int z = int.Parse(parts[2]);
                int type = int.Parse(parts[3]);

                var pos = new int3(x, y, z);

                Voxels[pos] = (byte)type;
            }

            return Voxels;
        }
    }
}