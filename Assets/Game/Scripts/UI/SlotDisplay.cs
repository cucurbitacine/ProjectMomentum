using Game.Scripts.Core;
using Game.Scripts.InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class SlotDisplay : MonoBehaviour, ISlotDisplay
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;
        
        public void Display(ISlot slot)
        {
            if (slot.IsFree())
            {
                root.SetActive(false);

                icon.sprite = null;
            }
            else
            {
                root.SetActive(true);

                if (slot.GetItem() is IIconSource iconSource)
                {
                    icon.sprite = iconSource.GetIcon();
                }
                else
                {
                    icon.sprite = null;
                }
            }
            
            text.text = $"{slot.CountItems}";
        }
    }

    public interface ISlotDisplay
    {
        public void Display(ISlot slot);
    }
}