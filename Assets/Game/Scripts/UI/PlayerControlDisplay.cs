using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Interactions;
using Game.Scripts.InventorySystem;
using Game.Scripts.Player;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class PlayerControlDisplay : MonoBehaviour
    {
        [SerializeField] private PlayerController player;

        [Header("UI")]
        [SerializeField] private TMP_Text interactText;
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private RectTransform controlRoot;

        private bool hintIsShown => !hintText.enabled;
        
        private void UpdateInteractor()
        {
            interactText.enabled = player.Interactor.Interactions.Any(s => s.IsReadyBeInteracted(player.gameObject));
        }
        
        private void OnListUpdated(List<IInteractable> list)
        {
            UpdateInteractor();
        }
        
        private void OnInventoryUpdated(IInventory inventory, ISlot slot)
        {
            UpdateInteractor();
        }

        private void ShowHint(bool value)
        {
            hintText.enabled = !value;
            controlRoot.gameObject.SetActive(value);
        }
        
        private void OnEnable()
        {
            player.Interactor.ListUpdated += OnListUpdated;
            player.InventoryUpdated += OnInventoryUpdated;
        }
        
        private void OnDisable()
        {
            player.Interactor.ListUpdated -= OnListUpdated;
            player.InventoryUpdated -= OnInventoryUpdated;
        }

        private void Start()
        {
            OnListUpdated(player.Interactor.Interactions);

            ShowHint(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                ShowHint(!hintIsShown);
            }
        }
    }
}
