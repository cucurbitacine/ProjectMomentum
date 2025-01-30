using System;
using UnityEngine;

namespace Game.Scripts.Control
{
    public class Fuel : MonoBehaviour
    {
        [field: SerializeField] public float Value { get; private set; } = 1000f;
        [field: SerializeField] public float Max { get; private set; } = 1000f;

        [field: Space]
        [field: SerializeField] public bool IsEmpty { get; private set; } = false;
        [field: SerializeField] public bool Infinity { get; set; } = false;
        
        public event Action<float> OnMaxChanged;
        public event Action<float> OnValueChanged; 

        public event Action<float> OnDecreased; 
        public event Action<float> OnIncreased; 
        
        public event Action OnEmptied; 
        public event Action OnFilled; 
        
        public void SetValue(float newValue)
        {
            if (IsEmpty) return;
            
            newValue = Mathf.Clamp(newValue, 0, Max);

            if (Mathf.Approximately(Mathf.Abs(Value - newValue), 0f)) return;

            Value = newValue;
                
            OnValueChanged?.Invoke(Value);

            if (Value <= 0f)
            {
               Empty();
            }
        }
        
        public void SetMax(float newMax)
        {
            newMax = Mathf.Max(1, newMax);
            
            if (Mathf.Approximately(Mathf.Abs(Max - newMax), 0f)) return;

            Max = newMax;

            SetValue(Value);
            
            OnMaxChanged?.Invoke(Value);
        }
        
        public void Decrease(float amount)
        {
            if (IsEmpty) return;

            if (Infinity) return;
            
            amount = Mathf.Min(amount, Value);
            
            if (amount <= 0) return;
            
            SetValue(Value - amount);
            
            OnDecreased?.Invoke(amount);
        }
        
        public void Increase(float amount)
        {
            if (IsEmpty) return;
            
            amount = Mathf.Min(amount, Max - Value);
            
            if (amount <= 0) return;

            SetValue(Value + amount);
            
            OnIncreased?.Invoke(amount);
        }

        public void Empty()
        {
            if (IsEmpty) return;
            
            if (Infinity) return;

            Value = 0f;
            IsEmpty = true;

            OnEmptied?.Invoke();
        }

        public void Fill()
        {
            if (!IsEmpty) return;
            
            IsEmpty = false;
            
            OnFilled?.Invoke();
        }

        private void OnValidate()
        {
            if (Value < 0) Value = 0f;
        }
    }
}