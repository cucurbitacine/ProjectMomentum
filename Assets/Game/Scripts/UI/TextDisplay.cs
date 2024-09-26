using System;
using Game.Scripts.Levels;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class TextDisplay : TextDisplayBase
    {
        [SerializeField] private bool isDisplaying = false;
        
        [Header("UI")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private TMP_Text text;
        
        public override void Display(bool value)
        {
            isDisplaying = value;

            canvas.enabled = isDisplaying;
        }
        
        public override void SetText(string message)
        {
            text.text = message;
        }
        
        public override void SetColor(Color color)
        {
            text.color = color;
        }
        
        public void SetText(string message, Color color)
        {
            SetText(message);
            SetColor(color);
        }
        
        public void Start()
        {
            Display(isDisplaying);
        }

        private void OnValidate()
        {
            if (canvas)
            {
                canvas.enabled = isDisplaying;
            }
        }
    }
}