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
                return player.Spaceship.Fuel < player.Spaceship.FuelMax;
            }

            return false;
        }

        public void Pickup(GameObject actor)
        {
            if (actor.TryGetComponent<PlayerController>(out var player))
            {
                if (usePercent)
                {
                    var amount = (int)(fuelAmount * 0.01f * player.Spaceship.FuelMax);
                    player.Spaceship.AddFuel(amount);
                }
                else
                {
                    player.Spaceship.AddFuel(fuelAmount);
                }
                
                SmartPrefab.SmartDestroy(gameObject);
                
                if (pickupEffectPrefab)
                {
                    var pickupEffect = SmartPrefab.SmartInstantiate(pickupEffectPrefab, actor.transform.position, actor.transform.rotation);
                    
                    pickupEffect.Play();
                }
            }
        }
    }
}