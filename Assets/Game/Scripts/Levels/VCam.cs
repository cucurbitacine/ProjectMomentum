using System;
using System.Collections.Generic;
using Cinemachine;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Levels
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class VCam : MonoBehaviour
    {
        [SerializeField] private bool activeOnStart = false;
        
        private LazyComponent<CinemachineVirtualCamera> _lazyCamera;

        public CinemachineVirtualCamera virtualCamera => (_lazyCamera ??= new LazyComponent<CinemachineVirtualCamera>(gameObject)).Value;

        public void SetActive()
        {
            VCam.ActiveCamera = this;
        }
        
        private void OnEnable()
        {
            VCam.Register(this);
        }

        private void OnDisable()
        {
            VCam.Unregister(this);
        }

        private void Start()
        {
            if (activeOnStart)
            {
                SetActive();
            }
        }

        public const int InactivePriority = 0;
        public const int ActivePriority = 100;
        
        private static readonly List<VCam> Cams = new List<VCam>();
        private static VCam _activeCamera;
        public static VCam ActiveCamera
        {
            get => _activeCamera;
            set
            {
                if (_activeCamera)
                {
                    _activeCamera.virtualCamera.Priority = InactivePriority;
                }

                _activeCamera = value;
                
                if (_activeCamera)
                {
                    _activeCamera.virtualCamera.Priority = ActivePriority;
                }
            }
        }

        public static void Register(VCam vCam)
        {
            if (Cams.Contains(vCam)) return;
            
            Cams.Add(vCam);
        }
        
        public static void Unregister(VCam vCam)
        {
            Cams.Remove(vCam);
        }

        public static void NextCamera()
        {
            if (Cams.Count < 2) return; 
            
            var index = Cams.IndexOf(ActiveCamera);

            if (0 <= index && index < Cams.Count)
            {
                ActiveCamera = Cams[(index + 1) % Cams.Count];
            }
        }
    }
}
