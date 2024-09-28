using Game.Scripts.Levels;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class PauseDisplay : MonoBehaviour
    {
        [SerializeField] private LevelController level;
        
        [Space]
        [SerializeField] private bool paused = false;
        
        [Space]
        [SerializeField] private GameObject menuObject;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;

        public void Pause(bool pause)
        {
            paused = pause;
            
            menuObject.gameObject.SetActive(paused);
        }
        
        private void HandleContinueButton()
        {
            Pause(false);
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
        
        private void OnEnable()
        {
            continueButton.onClick.AddListener(HandleContinueButton);
            restartButton.onClick.AddListener(HandleRestartButton);
            exitButton.onClick.AddListener(HandleExitButton);
        }

        private void OnDisable()
        {
            continueButton.onClick.RemoveListener(HandleContinueButton);
            restartButton.onClick.RemoveListener(HandleRestartButton);
            exitButton.onClick.RemoveListener(HandleExitButton);
        }

        private void Start()
        {
            Pause(paused);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Pause(!paused);
            }
        }
    }
}
