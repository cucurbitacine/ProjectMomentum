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

        private static Camera CameraMain => Camera.main;
        private static Vector2 ScreenSize => new Vector2(Screen.width, Screen.height);
        private static Vector2 ScreenCenter => ScreenSize * 0.5f;
        private static Vector2 WorldPointScreenCenter => CameraMain.ScreenToWorldPoint(ScreenCenter);
        
        private static Vector2 WorldToScreenVector(Vector2 vector)
        {
            return ((Vector2)CameraMain.WorldToScreenPoint(WorldPointScreenCenter + vector) - ScreenCenter).normalized;
        }
        
        private void LateUpdate()
        {
            var originWorldPoint = origin.position;
            
            var originScreenPosition = (Vector2)CameraMain.WorldToScreenPoint(originWorldPoint) - ScreenCenter;
            
            verticalImage.rectTransform.anchoredPosition = originScreenPosition;
            horizontalImage.rectTransform.anchoredPosition = originScreenPosition;
            
            var rotation = Quaternion.LookRotation(Vector3.forward, WorldToScreenVector(origin.up));
            verticalImage.rectTransform.rotation = rotation;
            horizontalImage.rectTransform.rotation = rotation;
            
            northImage.rectTransform.rotation = Quaternion.LookRotation(Vector3.forward, WorldToScreenVector(Vector2.up));
        }
    }
}
