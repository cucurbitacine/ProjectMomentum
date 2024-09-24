using Game.Scripts.Core;
using Game.Scripts.Interactions;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PickableHeal : MonoBehaviour, IPickable
    {
        [SerializeField] private int healAmount = 100;
        
        public bool IsValid(GameObject actor)
        {
            if (actor.TryGetComponent<PlayerController>(out var player))
            {
                return player.Health.Value < player.Health.Max;
            }

            return false;
        }

        public void Pickup(GameObject actor)
        {
            if (actor.TryGetComponent<PlayerController>(out var player))
            {
                player.Health.Heal(healAmount);
                
                SmartPrefab.SmartDestroy(gameObject);
            }
        }
    }
}