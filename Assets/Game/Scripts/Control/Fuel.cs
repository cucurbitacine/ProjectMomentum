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
        
        public event Action<float> MaxValueChanged;
        public event Action<float> ValueChanged; 

        public event Action<float> Decreased; 
        public event Action<float> Increased; 
        
        public event Action Emptied; 
        public event Action Filled; 
        
        public void SetValue(float newValue)
        {
            if (IsEmpty) return;
            
            newValue = Mathf.Clamp(newValue, 0, Max);

            if (Mathf.Approximately(Mathf.Abs(Value - newValue), 0f)) return;

            Value = newValue;
                
            ValueChanged?.Invoke(Value);

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
            
            MaxValueChanged?.Invoke(Value);
        }
        
        public void Decrease(float amount)
        {
            if (IsEmpty) return;

            if (Infinity) return;
            
            amount = Mathf.Min(amount, Value);
            
            if (amount <= 0) return;
            
            SetValue(Value - amount);
            
            Decreased?.Invoke(amount);
        }
        
        public void Increase(float amount)
        {
            if (IsEmpty) return;
            
            amount = Mathf.Min(amount, Max - Value);
            
            if (amount <= 0) return;

            SetValue(Value + amount);
            
            Increased?.Invoke(amount);
        }

        public void Empty()
        {
            if (IsEmpty) return;
            
            if (Infinity) return;

            Value = 0f;
            IsEmpty = true;

            Emptied?.Invoke();
        }

        public void Fill()
        {
            if (!IsEmpty) return;
            
            IsEmpty = false;
            
            Filled?.Invoke();
        }

        private void OnValidate()
        {
            if (Value < 0) Value = 0f;
        }
    }
}