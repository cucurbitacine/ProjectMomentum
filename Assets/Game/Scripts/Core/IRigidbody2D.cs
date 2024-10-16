using UnityEngine;

namespace Game.Scripts.Core
{
    public interface IRigidbody2D
    {
        public float mass { get; }
        
        public void AddForce(Vector2 force, ForceMode2D mode);
        public void AddTorque(float torque, ForceMode2D mode);

        public void Simulate(bool value);
    }
}
