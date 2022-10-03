using UnityEngine;

namespace MommyChicken.World.SO
{
    [CreateAssetMenu(fileName = "WorldGeneration", menuName = "BiomeSetting", order = 0)]
    public class BiomeSettingSo : ScriptableObject
    {
        public GameObject[] FloorTiles;

        public GameObject RandomFloorTile => FloorTiles[Random.Range(0, FloorTiles.Length - 1)];
    }
}