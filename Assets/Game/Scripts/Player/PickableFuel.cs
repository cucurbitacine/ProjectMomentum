using Game.Scripts.Core;
using Game.Scripts.Interactions;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PickableFuel : MonoBehaviour, IPickable
    {
        [SerializeField] private bool usePercent = false;
        [SerializeField] private float fuelAmount = 100;
        
        [Space]
        [SerializeField] private GameObject pickupEffectPrefab;
        
        public bool IsValid(GameObject actor)
        {
            if (actor.TryGetComponent<PlayerController>(out var player) && !player.Health.IsDead)
            {
                return player.Spaceship.Fuel.Value < player.Spaceship.Fuel.Max;
            }

            return false;
        }

        public void Pickup(GameObject actor)
        {
            if (actor.TryGetComponent<PlayerController>(out var player))
            {
                if(player.Spaceship.Fuel.IsEmpty) player.Spaceship.Fuel.Fill();
                
                if (usePercent)
                {
                    var amount = (int)(fuelAmount * 0.01f * player.Spaceship.Fuel.Max);
                    
                    player.Spaceship.Fuel.Increase(amount);
                }
                else
                {
                    player.Spaceship.Fuel.Increase(fuelAmount);
                }
                
                SmartPrefab.SmartDestroy(gameObject);
                
                if (pickupEffectPrefab)
                {
                    var pickupEffect = SmartPrefab.SmartInstantiate(pickupEffectPrefab, actor.transform.position, actor.transform.rotation);
                    
                    pickupEffect.PlaySafe();
                }
            }
        }
    }
}