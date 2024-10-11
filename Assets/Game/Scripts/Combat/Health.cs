using System;
using UnityEngine;

namespace Game.Scripts.Combat
{
    [DisallowMultipleComponent]
    public class Health : MonoBehaviour
    {
        [field: SerializeField] public int Value { get; private set; } = 100;
        [field: SerializeField] public int Max { get; private set; } = 100;

        [field: Space]
        [field: SerializeField] public bool IsDead { get; private set; } = false;
        [field: SerializeField] public bool Immortal { get; set; } = false;
        
        public event Action<int> MaxValueChanged;
        public event Action<int> ValueChanged; 

        public event Action<int> Damaged; 
        public event Action<int> Healed; 
        
        public event Action Died; 
        public event Action Revived; 
        
        public void SetValue(int newValue)
        {
            if (IsDead) return;
            
            newValue = Mathf.Clamp(newValue, 0, Max);
            
            if (Value == newValue) return;

            Value = newValue;
                
            ValueChanged?.Invoke(Value);

            if (Value == 0)
            {
               Die();
            }
        }
        
        public void SetMax(int newMax)
        {
            newMax = Mathf.Max(1, newMax);
            
            if (Max == newMax) return;

            Max = newMax;

            SetValue(Value);
            
            MaxValueChanged?.Invoke(Value);
        }
        
        public void Damage(int amount)
        {
            if (IsDead) return;

            if (Immortal) return;
            
            amount = Mathf.Min(amount, Value);
            
            if (amount <= 0) return;
            
            SetValue(Value - amount);
            
            Damaged?.Invoke(amount);
        }
        
        public void Heal(int amount)
        {
            if (IsDead) return;
            
            amount = Mathf.Min(amount, Max - Value);
            
            if (amount <= 0) return;

            SetValue(Value + amount);
            
            Healed?.Invoke(amount);
        }

        public void Die()
        {
            if (IsDead) return;
            
            if (Immortal) return;
            
            IsDead = true;

            Died?.Invoke();
        }

        public void Revive()
        {
            if (!IsDead) return;
            
            IsDead = false;
            
            if (Value == 0)
            {
                Heal(1);
            }
            
            Revived?.Invoke();
        }
        
        private void OnValidate()
        {
            if (Value < 0) Value = 0;
        }
    }
}
