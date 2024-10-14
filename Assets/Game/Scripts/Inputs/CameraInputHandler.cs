using Cinemachine;
using Game.Scripts.Levels;
using UnityEngine;

namespace Game.Scripts.Inputs
{
    public class CameraInputHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float zoomRate = 1f;
        [SerializeField] [Min(0f)] private float aimScale = 0.5f;
        [SerializeField] [Min(0f)] private float aimDamp = 4f;
            
        [Header("Input")]
        [SerializeField] private CameraInput cameraInput;

        private float _size = -100f;
        private float _zoom = 0f;
        private float _aim = 0f;
        private Vector3 _aimOffset = Vector3.zero;
        
        private void UpdateZoom()
        {
            if (!VCam.ActiveCamera) return;
            
            if (_size < 0f)
            {
                _size = VCam.ActiveCamera.virtualCamera.m_Lens.OrthographicSize;
            }
            
            if (!Mathf.Approximately(_zoom, 0f))
            {
                _size += -_zoom * zoomRate * _size * Time.deltaTime;

                _size = Mathf.Clamp(_size, 1f, 20f);
                
                VCam.ActiveCamera.virtualCamera.m_Lens.OrthographicSize = _size;
            }
        }
        
        private void UpdateAim()
        {
            if (!VCam.ActiveCamera) return;
            
            var virtualCamera = VCam.ActiveCamera.virtualCamera;
            var composer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

            if (composer)
            {
                var lerp = _aim * aimScale;
                
                var offset = Vector3.LerpUnclamped(Vector3.zero, Vector3.up * _size, lerp);

                _aimOffset = Vector3.Lerp(_aimOffset, offset, aimDamp * Time.deltaTime);
                
                composer.m_TrackedObjectOffset = _aimOffset;
            }
        }
        
        private void HandleZoom(float zoom)
        {
            _zoom = zoom;
        }
        
        private void HandleChangeCamera()
        {
            VCam.NextCamera();
            
            if (!VCam.ActiveCamera) return;
            
            VCam.ActiveCamera.virtualCamera.m_Lens.OrthographicSize = _size;
        }

        private void HandleAimEvent(float value)
        {
            _aim = value;
        }
        
        private void OnEnable()
        {
            cameraInput.ZoomEvent += HandleZoom;
            cameraInput.ChangeCameraEvent += HandleChangeCamera;
            cameraInput.AimEvent += HandleAimEvent;
        }

        private void OnDisable()
        {
            cameraInput.ZoomEvent -= HandleZoom;
            cameraInput.ChangeCameraEvent -= HandleChangeCamera;
            cameraInput.AimEvent -= HandleAimEvent;
        }
        
        private void Update()
        {
            UpdateZoom();

            UpdateAim();
        }
    }
}