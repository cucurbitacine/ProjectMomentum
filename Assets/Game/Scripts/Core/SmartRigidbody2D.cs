using System.Collections.Generic;
using CucuTools;
using UnityEngine;

namespace Game.Scripts.Core
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class SmartRigidbody2D : MonoBehaviour, IRigidbody2D
    {
        private Vector2 _velocity;
        private float _angularVelocity;

        private readonly Queue<IRigidbody2DCommand> _commands = new Queue<IRigidbody2DCommand>();
        
        private LazyComponent<Rigidbody2D> _lazyRigidbody2D;

        public bool Simulating
        {
            get => Rigid2d.bodyType != RigidbodyType2D.Static;
            private set => Rigid2d.bodyType = value ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
        }

        public Rigidbody2D Rigid2d => (_lazyRigidbody2D ??= new LazyComponent<Rigidbody2D>(gameObject)).Value;

        public float mass => Rigid2d.mass;

        public void AddForce(Vector2 force, ForceMode2D mode)
        {
            var cmd = new AddForceCommand() { rigidbody2D = Rigid2d, force = force, mode = mode };

            if (Simulating)
            {
                cmd.Execute();
            }
            else
            {
                _commands.Enqueue(cmd);
            }
        }

        public void AddTorque(float torque, ForceMode2D mode)
        {
            var cmd = new AddTorqueCommand() { rigidbody2D = Rigid2d, torque = torque, mode = mode };
            
            if (Simulating)
            {
                cmd.Execute();
            }
            else
            {
                _commands.Enqueue(cmd);
            }
        }
        
        public void Simulate(bool value)
        {
            if (Simulating == value) return;

            if (value)
            {
                // Turn On
                // Restore velocity
                // Execute all requests
                
                Simulating = true;

                RestoreVelocity();
                
                ExecuteRequests();
            }
            else
            {
                StoreVelocity();

                Simulating = false;
                
                // Store velocity
                // Turn Off
            }
        }

        private void RestoreVelocity()
        {
            Rigid2d.velocity = _velocity;
            Rigid2d.angularVelocity = _angularVelocity;
        }

        private void StoreVelocity()
        {
            _velocity = Rigid2d.velocity;
            _angularVelocity = Rigid2d.angularVelocity;
        }
        
        private void ExecuteRequests()
        {
            while (_commands.TryDequeue(out var cmd))
            {
                cmd.Execute();
            }
        }
    }

    public interface IRigidbody2DCommand : ICommand
    {
    }
    
    public struct AddForceCommand : IRigidbody2DCommand
    {
        public Rigidbody2D rigidbody2D;
        public Vector2 force;
        public ForceMode2D mode;
        
        public void Execute()
        {
            rigidbody2D.AddForce(force, mode);
        }

        public void Undo()
        {
            rigidbody2D.AddForce(-force, mode);
        }
    }
    
    public struct AddTorqueCommand : IRigidbody2DCommand
    {
        public Rigidbody2D rigidbody2D;
        public float torque;
        public ForceMode2D mode;
        
        public void Execute()
        {
            rigidbody2D.AddTorque(torque, mode);
        }

        public void Undo()
        {
            rigidbody2D.AddTorque(-torque, mode);
        }
    }
}