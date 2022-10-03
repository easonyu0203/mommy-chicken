using System;
using System.Collections;
using System.Collections.Generic;
using MommyChicken.World.SO;
using UnityEngine;

namespace MommyChicken.World
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private BiomeSettingSo _biomeSettingSo;
        [SerializeField] private float _intervalSize = 100;
        [Range(0f, 1f)] [SerializeField] private float _intervalOffsetPercentage = 0.5f;
        [SerializeField] private float _updateInterval = 1f;
        private int _floorTileWidth = 30;

        private LinkedList<GameObject> _tiles = new LinkedList<GameObject>();

        private void Start()
        {
            StartCoroutine(GenerateWorld());
        }

        private IEnumerator GenerateWorld()
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(_updateInterval);
            bool hasInitTile = false;
            while (true)
            {
                float targetZ = _target.position.z;
                float lowerBound = targetZ - _intervalSize * _intervalOffsetPercentage;
                float upperBound = targetZ + _intervalSize * (1 - _intervalOffsetPercentage);
                int lowerZ = ((int)lowerBound / _floorTileWidth) * _floorTileWidth;
                int upperZ = ((int)(upperBound + 1) / _floorTileWidth + 1) * _floorTileWidth;
                if (hasInitTile)
                {
                    int lowestInListZ = Mathf.RoundToInt(_tiles.First.Value.transform.position.z);
                    int uppestInListZ = Mathf.RoundToInt(_tiles.Last.Value.transform.position.z);
                    // fill need
                    for (int i = lowestInListZ - _floorTileWidth; i >= lowerZ; i -= _floorTileWidth)
                    {
                        GameObject tile = _tiles.Last.Value;
                        tile.transform.position = new Vector3(0, 0, i);
                        _tiles.RemoveLast();
                        _tiles.AddFirst(tile);
                    }

                    for (int i = uppestInListZ + _floorTileWidth; i <= upperZ ; i += _floorTileWidth)
                    {
                        GameObject tile = _tiles.First.Value;
                        tile.transform.position = new Vector3(0, 0, i);
                        _tiles.RemoveFirst();
                        _tiles.AddLast(tile);
                    }
                }
                else
                {
                    // init tiles
                    for (int z = lowerZ; z <= upperZ  + _floorTileWidth; z += _floorTileWidth)
                    {
                        GameObject prefab = _biomeSettingSo.RandomFloorTile;
                        Vector3 pos = new Vector3(0, 0, z);
                        Quaternion rot = prefab.transform.rotation;
                        _tiles.AddLast(Instantiate(prefab, pos, rot, transform));
                    }
                }


                hasInitTile = true;
                yield return waitForSeconds;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector3 cen = (_target.transform.position.z + _intervalSize * (-_intervalOffsetPercentage + 0.5f)) *
                          Vector3.forward;
            Gizmos.DrawWireSphere(cen, _intervalSize * 0.5f);
        }
    }
}