using CucuTools;
using Game.Scripts.Interactions;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PickableHeal : MonoBehaviour, IPickable, IInteractable
    {
        [SerializeField] private bool usePercent = false;
        [SerializeField] [Min(0)] private int healAmount = 100;

        [Space]
        [SerializeField] private GameObject pickupEffectPrefab;
        
        public bool IsReadyBePicked(GameObject actor)
        {
            if (actor.TryGetComponent<PlayerController>(out var player) && !player.Health.IsDead)
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

                if (pickupEffectPrefab)
                {
                    var pickupEffect = SmartPrefab.SmartInstantiate(pickupEffectPrefab, actor.transform.position, actor.transform.rotation);
                    
                    pickupEffect.PlaySafe();
                }
            }
        }

        public bool IsReadyBeInteracted(GameObject actor)
        {
            return IsReadyBePicked(actor);
        }

        public void StartInteraction(GameObject actor)
        {
            Pickup(actor);
        }

        public void StopInteraction(GameObject actor)
        {
        }
    }
}