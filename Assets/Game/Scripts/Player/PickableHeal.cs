using Game.Scripts.Core;
using Game.Scripts.Interactions;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PickableHeal : MonoBehaviour, IPickable
    {
        [SerializeField] private bool usePercent = false;
        [SerializeField] [Min(0)] private int healAmount = 100;
        
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
                if (usePercent)
                {
                    var amount = (int)(healAmount * 0.01f * player.Health.Max);
                    player.Health.Heal(amount);
                }
                else
                {
                    player.Health.Heal(healAmount);
                }
                
                SmartPrefab.SmartDestroy(gameObject);
            }
        }
    }
}