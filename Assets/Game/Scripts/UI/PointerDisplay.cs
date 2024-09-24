using System;
using System.Collections;
using Game.Scripts.Control;
using Game.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Pointer = Game.Scripts.Levels.Pointer;

namespace Game.Scripts.UI
{
    public class PointerDisplay : Pointer
    {
        [field: SerializeField] public override bool IsPointing { get; protected set; }
        
        [Header("Target")]
        [SerializeField] private string displayName = "Target";
        [SerializeField] private Transform target;
        
        [Header("Settings")]
        [SerializeField] [Range(0f, 1f)] private float radiusScale = 0.8f;
        [SerializeField] [Min(0f)] private float minDistance = 5f;
        
        [Header("UI")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image arrowImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text distanceText;

        [Space]
        [SerializeField] private RectTransform infoPanel;

        private static Camera CameraMain => Camera.main;
        private static float RadiusDisplayArrow => Mathf.Min(Screen.width, Screen.height) * 0.5f;

        private static SpaceshipController Spaceship => PlayerController.Player ? PlayerController.Player.Spaceship : null;
        
        public override void SetName(string newDisplayName)
        {
            displayName = newDisplayName;

            titleText.text = $"{displayName}";
        }

        public override void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public override void Show(bool value)
        {
            IsPointing = value;

            canvas.enabled = value;
        }
        
        private void Awake()
        {
            if (target == null) target = transform;
        }

        private void Start()
        {
            Show(IsPointing);
        }

        private void Update()
        {
            if (!IsPointing) return;
            
            if (target == null) return;
            if (Spaceship == null) return;
            
            var distance = Vector2.Distance(Spaceship.position, target.position);

            arrowImage.gameObject.SetActive(distance > minDistance);
            infoPanel.gameObject.SetActive(distance > minDistance);
            
            if (distance < minDistance) return;
            
            var targetScreenPosition = CameraMain.WorldToScreenPoint(target.position) - new Vector3(Screen.width, Screen.height, 0f) * 0.5f;
            var spaceshipScreenPosition = CameraMain.WorldToScreenPoint(Spaceship.position) - new Vector3(Screen.width, Screen.height, 0f) * 0.5f;

            var arrowScreenPosition = Vector2.ClampMagnitude(targetScreenPosition, RadiusDisplayArrow * radiusScale);
            
            infoPanel.anchoredPosition = arrowScreenPosition;
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
        }
    }
}