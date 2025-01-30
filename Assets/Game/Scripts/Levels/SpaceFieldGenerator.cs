using System;
using Game.Scripts.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Levels
{
    public class SpaceFieldGenerator : MonoBehaviour
    {
        [SerializeField] private bool generateOnStart = true;

        [Space]
        [SerializeField] private Vector2 fieldSize = Vector2.one;
        [SerializeField] private Vector2Int fieldGrid = Vector2Int.one * 10;
        [SerializeField] [Min(1)] private int amountPerCell = 4;

        [Space]
        [SerializeField] private bool doPhysics = true;
        [SerializeField] [Min(0f)] private float torqueMax = 1f;
        [SerializeField] [Min(0f)] private float forceMax = 1f;
        [SerializeField] private LayerMask obstacleLayer = 1;

        [Space]
        [SerializeField] private PrefabList prefabList;
        [SerializeField] private Color gridColor = Color.white;
        
        private Vector2 fieldCenter => transform.position;
        
        public void Generate()
        {
            for (var i = 0; i < fieldGrid.x; i++)
            {
                for (var j = 0; j < fieldGrid.y; j++)
                {
                    for (var k = 0; k < amountPerCell; k++)
                    {
                        var point = GetRandomPointInCell(i, j);
                        var angle = Random.value * 360f;
                        var rotation = Quaternion.Euler(0, 0, angle);

                        var prefab = GetPrefab();
                        var prefabSize = GetSize(prefab);
                        
                        if (Physics2D.OverlapBox(point, prefabSize, angle, obstacleLayer))
                        {
                            continue;
                        }

                        var asteroid = SmartPrefab.SmartInstantiate(prefab, point, rotation, transform);

                        if (doPhysics && asteroid.TryGetComponent<Rigidbody2D>(out var rigid2d))
                        {
                            rigid2d.AddTorque((Random.value * 2f - 1f) * torqueMax, ForceMode2D.Impulse);
                            rigid2d.AddForce(Random.insideUnitCircle * forceMax, ForceMode2D.Impulse);
                        }
                    }
                }
            }
        }

        private Vector2 GetCellCenter(int i, int j)
        {
            i = Mathf.Clamp(i, 0, fieldGrid.x);
            j = Mathf.Clamp(j, 0, fieldGrid.y);

            var cellSize = GetCellSize();
            
            var x = i * cellSize.x + cellSize.x * 0.5f;
            var y = j * cellSize.y + cellSize.y * 0.5f;

            return new Vector2(x, y) + fieldCenter - fieldSize * 0.5f;
        }

        private Vector2 GetCellSize()
        {
            return new Vector2(fieldGrid.x > 0 ? fieldSize.x / fieldGrid.x : fieldSize.x,
                fieldGrid.y > 0 ? fieldSize.y / fieldGrid.y : fieldSize.y);
        }
        
        private Vector2 GetRandomPointInCell(int i, int j)
        {
            var cellCenter = GetCellCenter(i, j);
            var cellSize = GetCellSize();
            
            return cellCenter + Vector2.Scale(new Vector2(Random.value, Random.value) - Vector2.one * 0.5f, cellSize);
        }

        private GameObject GetPrefab()
        {
            return prefabList.GetRandomPrefab();
        }

        private Vector2 GetSize(GameObject prefab)
        {
            return prefab.TryGetComponent<Renderer>(out var renderer)
                ? renderer.bounds.size
                : Vector2.one;
        }
        
        private void Start()
        {
            if (generateOnStart)
            {
                Generate();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(fieldCenter, fieldSize);
        }

        private void OnDrawGizmosSelected()
        {
            var cellSize = GetCellSize();
            
            Gizmos.color = gridColor;
            for (var i = 0; i < fieldGrid.x; i++)
            {
                for (var j = 0; j < fieldGrid.y; j++)
                {
                    Gizmos.DrawWireCube(GetCellCenter(i, j), cellSize);
                }
            }
        }
    }
}