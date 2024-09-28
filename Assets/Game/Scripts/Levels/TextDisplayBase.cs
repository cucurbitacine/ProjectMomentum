using UnityEngine;

namespace Game.Scripts.Levels
{
    public abstract class TextDisplayBase : MonoBehaviour
    {
        public abstract void Display(bool value);

        public abstract void SetText(string message);
        public abstract void SetColor(Color color);

        public void Display(string message, Color color)
        {
            SetText(message);
            SetColor(color);

            Display(true);
        }
    }
}