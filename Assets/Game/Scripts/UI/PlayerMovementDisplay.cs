using Game.Scripts.Control;
using Game.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class PlayerMovementDisplay : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        
        [Header("UI")]
        [SerializeField] private RectTransform root;
        [SerializeField] private Image northImage;
        [SerializeField] private TMP_Text speedText;
        [SerializeField] private TMP_Text angularSpeedText;
        
        private static Camera CameraMain => Camera.main;
        private static Vector2 ScreenSize => new Vector2(Screen.width, Screen.height);
        private static Vector2 ScreenCenter => ScreenSize * 0.5f;
        private static Vector2 WorldPointScreenCenter => CameraMain.ScreenToWorldPoint(ScreenCenter);

        public Spaceship Spaceship => player.Spaceship;
        
        private static Vector2 WorldToScreenVector(Vector2 vector)
        {
            return ((Vector2)CameraMain.WorldToScreenPoint(WorldPointScreenCenter + vector) - ScreenCenter).normalized;
        }
        
        private void LateUpdate()
        {
            northImage.rectTransform.rotation = Quaternion.LookRotation(Vector3.forward, WorldToScreenVector(Vector2.up));

            speedText.text = $"{Spaceship.velocity.magnitude:F1} unit/s{(Spaceship.KeepPosition ? " Keep" : "")}";
            angularSpeedText.text = $"{Spaceship.angularVelocity:F1} deg/s{(Spaceship.KeepRotation ? " Keep" : "")}";
        }
    }
}
