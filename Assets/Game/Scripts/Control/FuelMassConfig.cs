using UnityEngine;

namespace Game.Scripts.Control
{
    [CreateAssetMenu(menuName = "Game/Create FuelMass Config", fileName = "FuelMass Config", order = 0)]
    public class FuelMassConfig : ScriptableObject
    {
        public static FuelMassConfig Instance { get; private set; }

        [field: SerializeField] public float MassPerUnit { get; private set; } = 1f;
        [field: SerializeField, Min(0f)] public float Power { get; private set; } = 1f;

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}