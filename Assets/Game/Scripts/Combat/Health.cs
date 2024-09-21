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
        
        public event Action<int> OnMaxChanged;
        public event Action<int> OnValueChanged; 

        public event Action<int> OnDamaged; 
        public event Action<int> OnHealed; 
        
        public event Action OnDied; 
        
        public void SetValue(int newValue)
        {
            if (IsDead) return;
            
            newValue = Mathf.Clamp(newValue, 0, Max);
            
            if (Value == newValue) return;

            Value = newValue;
                
            OnValueChanged?.Invoke(Value);

            if (Value == 0)
            {
                IsDead = true;

                OnDied?.Invoke();
            }
        }
        
        public void SetMax(int newMax)
        {
            newMax = Mathf.Max(1, newMax);
            
            if (Max == newMax) return;

            Max = newMax;

            SetValue(Value);
            
            OnMaxChanged?.Invoke(Value);
        }
        
        public void Damage(int amount)
        {
            if (IsDead) return;
            
            amount = Mathf.Min(amount, Value);
            
            if (amount <= 0) return;
            
            SetValue(Value - amount);
            
            OnDamaged?.Invoke(amount);
        }
        
        public void Heal(int amount)
        {
            if (IsDead) return;
            
            amount = Mathf.Min(amount, Max - Value);
            
            if (amount <= 0) return;

            SetValue(Value + amount);
            
            OnHealed?.Invoke(amount);
        }

        private void OnValidate()
        {
            if (Value < 0) Value = 0;
        }
    }
}
