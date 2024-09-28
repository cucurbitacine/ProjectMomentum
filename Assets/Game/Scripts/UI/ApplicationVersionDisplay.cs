using System;
using Game.Scripts.Core;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TMP_Text))]
    public class ApplicationVersionDisplay : MonoBehaviour
    {
        private LazyComponent<TMP_Text> _lazyText;

        public TMP_Text Text => (_lazyText ??= new LazyComponent<TMP_Text>(gameObject)).Value;

        public string GetVersion()
        {
            return $"{Application.platform}_{Application.unityVersion}_{Application.version}";
        }

        public void UpdateVersion()
        {
            Text.text = $"{GetVersion()}";
        }
        
        private void OnEnable()
        {
            UpdateVersion();
        }
    }
}
