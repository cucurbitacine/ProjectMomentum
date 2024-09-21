using Game.Scripts.Control;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class SpaceshipTargetDisplay : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private string displayName = "Target";
        [SerializeField] private Transform target;
        
        [Header("Settings")]
        [SerializeField] [Range(0f, 1f)] private float radiusScale = 0.8f;
        [SerializeField] [Min(0f)] private float minDistance = 5f;
        [SerializeField] private SpaceshipController spaceship;
        
        [Header("UI")]
        [SerializeField] private Image arrowImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text distanceText;

        [Space]
        [SerializeField] private RectTransform infoPanel;

        private static Camera CameraMain => Camera.main;
        private static float RadiusDisplayArrow => Mathf.Min(Screen.width, Screen.height) * 0.5f;
        
        private void Awake()
        {
            if (target == null) target = transform;
        }

        private void Update()
        {
            if (spaceship == null) return;
            if (target == null) return;

            var distance = Vector2.Distance(spaceship.position, target.position);

            arrowImage.gameObject.SetActive(distance > minDistance);
            infoPanel.gameObject.SetActive(distance > minDistance);
            
            if (distance < minDistance) return;
            
            var targetScreenPosition = CameraMain.WorldToScreenPoint(target.position) - new Vector3(Screen.width, Screen.height, 0f) * 0.5f;
            var spaceshipScreenPosition = CameraMain.WorldToScreenPoint(spaceship.position) - new Vector3(Screen.width, Screen.height, 0f) * 0.5f;

            var arrowScreenPosition = Vector2.ClampMagnitude(targetScreenPosition, RadiusDisplayArrow * radiusScale);
            
            arrowImage.rectTransform.anchoredPosition = arrowScreenPosition;
            arrowImage.rectTransform.rotation = Quaternion.LookRotation(Vector3.forward, targetScreenPosition - spaceshipScreenPosition);
/*
            var pivot = distanceText.rectTransform.pivot;
            pivot.x = arrowScreenPosition.x > 0f ? 1f : 0f;
            pivot.y = arrowScreenPosition.y > 0f ? 1f : 0f;
            distanceText.rectTransform.pivot = pivot;
*/
            titleText.text = $"{displayName}";
            distanceText.text = $"{distance:F1} units";
            
            infoPanel.anchoredPosition = arrowScreenPosition;
        }
    }
}