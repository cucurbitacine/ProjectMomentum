using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class SpaceshipOrientationDisplay : MonoBehaviour
    {
        [SerializeField] private Transform origin;

        [Space]
        [SerializeField] private Image verticalImage;
        [SerializeField] private Image horizontalImage;
        [SerializeField] private Image northImage;

        private Vector2 _screenCenter;
        
        private void LateUpdate()
        {
            _screenCenter.x = Screen.width;
            _screenCenter.y = Screen.height;
            _screenCenter *= 0.5f;

            var originPosition = origin.position;
            
            var screenOriginPosition = (Vector2)Camera.main.WorldToScreenPoint(originPosition) - _screenCenter;
            var screenOriginUpPosition = (Vector2)Camera.main.WorldToScreenPoint(originPosition + origin.up) - _screenCenter;
            var screenOriginUp = (screenOriginUpPosition - screenOriginPosition).normalized;

            var screenRotation = Quaternion.LookRotation(Vector3.forward, screenOriginUp);
            
            verticalImage.rectTransform.rotation = screenRotation;
            horizontalImage.rectTransform.rotation = screenRotation;
            
            verticalImage.rectTransform.anchoredPosition = screenOriginPosition;
            horizontalImage.rectTransform.anchoredPosition = screenOriginPosition;
            
            var screenWorldUpPosition = (Vector2)Camera.main.WorldToScreenPoint(originPosition + Vector3.up) - _screenCenter;
            var screenWorldUp = (screenWorldUpPosition - screenOriginPosition).normalized;
            northImage.rectTransform.rotation = Quaternion.LookRotation(Vector3.forward, screenWorldUp);
        }
    }
}
