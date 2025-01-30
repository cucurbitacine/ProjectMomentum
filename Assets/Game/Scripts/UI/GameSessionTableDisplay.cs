using Game.Scripts.Levels;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class GameSessionTableDisplay : MonoBehaviour
    {
        [SerializeField] private bool isDisplaying = false;
        [SerializeField] [Min(1)] private int countDisplay = 10;
        [SerializeField] private GameSessionTable table;
        
        [Space]
        [SerializeField] private Button openCloseButton;
        [SerializeField] private GameObject root;
        [SerializeField] private RectTransform container;
        
        [Space]
        [SerializeField] private GameSessionDataDisplay sessionDataDisplayPrefab;

        public void Display(bool value)
        {
            isDisplaying = value;

            root.SetActive(isDisplaying);
        }
        
        private void Build()
        {
            table = GameSessionTable.Load();

            if (table == null) return;
            if (table.sessions == null) return;
            
            table.sessions.Sort(CompareGameSessionData);
            
            var count = Mathf.Min(countDisplay, table.sessions.Count);
                            
            for (var i = 0; i < count; i++)
            {
                var sessionData = table.sessions[table.sessions.Count - 1 - i];
                var dataDisplay = Instantiate(sessionDataDisplayPrefab, container, false);

                dataDisplay.SetData(sessionData);
            }
        }

        private void HandleOpenCloseButton()
        {
            Display(!isDisplaying);
        }
        
        private void OnEnable()
        {
            openCloseButton.onClick.AddListener(HandleOpenCloseButton);
        }

        private void OnDisable()
        {
            openCloseButton.onClick.RemoveListener(HandleOpenCloseButton);
        }

        private void Start()
        {
            Build();
            
            Display(isDisplaying);
        }
        
        private static int CompareGameSessionData(GameSessionData x, GameSessionData y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            
            var savedAmountComparison = x.savedAmount.CompareTo(y.savedAmount);
            if (savedAmountComparison != 0) return savedAmountComparison;
            
            var timeSpentComparison = x.timeSpent.CompareTo(y.timeSpent);
            if (timeSpentComparison != 0) return -timeSpentComparison;
            
            var healthSpentComparison = x.healthSpent.CompareTo(y.healthSpent);
            if (healthSpentComparison != 0) return -healthSpentComparison;
            
            return -x.fuelSpent.CompareTo(y.fuelSpent);
        }
    }
}