using System;
using System.Collections.Generic;
using Game.Scripts.Inputs;
using Game.Scripts.Levels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class PauseDisplay : MonoBehaviour
    {
        [SerializeField] private LevelController level;
        
        [Space]
        [SerializeField] private UIInput uiInput;
        
        [Space]
        [SerializeField] private GameObject menuObject;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;
        
        public void OnLevelPaused(bool paused)
        {
            menuObject.gameObject.SetActive(paused);

            if (!LevelPause.IsPaused)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        
        private void HandleContinueButton()
        {
            LevelPause.Pause(false);
        }
        
        private void HandleRestartButton()
        {
            if (level.State != LevelController.LevelState.Playing) return;
            
            gameObject.SetActive(false);
            
            level.RestartLevel();
        }
        
        private void HandleExitButton()
        {
            if (level.State != LevelController.LevelState.Playing) return;
            
            gameObject.SetActive(false);
            
            level.ExitLevel();
        }
        
        private void OnMenuEvent()
        {
            if (!LevelPause.IsPaused)
            {
                LevelPause.Pause(true);
            }
        }

        private void OnCancelEvent()
        {
            if (LevelPause.IsPaused)
            {
                LevelPause.Pause(false);
            }
        }

        private void OnNavigateEvent(Vector2 vector)
        {
            if (LevelPause.IsPaused)
            {
                if (!Mathf.Approximately(vector.y, 0f))
                {
                    var currentSelectedObject = EventSystem.current.currentSelectedGameObject;

                    if (!currentSelectedObject || !buttons.Contains(currentSelectedObject))
                    {
                        EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
                    }
                }
            }
        }
        
        private void OnEnable()
        {
            LevelPause.LevelPaused += OnLevelPaused;
            
            continueButton.onClick.AddListener(HandleContinueButton);
            restartButton.onClick.AddListener(HandleRestartButton);
            exitButton.onClick.AddListener(HandleExitButton);
            
            uiInput.MenuEvent += OnMenuEvent;
            uiInput.CancelEvent += OnCancelEvent;
            uiInput.NavigateEvent += OnNavigateEvent;
        }

        private void OnDisable()
        {
            LevelPause.LevelPaused -= OnLevelPaused;
            
            uiInput.MenuEvent -= OnMenuEvent;
            uiInput.CancelEvent -= OnCancelEvent;
            uiInput.NavigateEvent -= OnNavigateEvent;
            
            continueButton.onClick.RemoveListener(HandleContinueButton);
            restartButton.onClick.RemoveListener(HandleRestartButton);
            exitButton.onClick.RemoveListener(HandleExitButton);
        }

        [Space]
        [SerializeField] private GameObject selected;
        [SerializeField] private List<GameObject> buttons = new List<GameObject>();
        
        private void Start()
        {
            buttons.Add(continueButton.gameObject);
            buttons.Add(restartButton.gameObject);
            buttons.Add(exitButton.gameObject);
            
            OnLevelPaused(LevelPause.IsPaused);
        }

        private void Update()
        {
            selected = EventSystem.current.currentSelectedGameObject;
        }
    }
}
