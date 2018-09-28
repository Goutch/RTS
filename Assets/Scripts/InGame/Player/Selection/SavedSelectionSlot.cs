using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SavedSelectionSlot : MonoBehaviour
    {
        private UnitGroup savedGroup;
        private Text numberUnitInGroup;
        private Image icon;

        public UnitGroup SavedGroup
        {
            get
            {
                if (savedGroup == null)
                    return new UnitGroup();
                return new UnitGroup(savedGroup.Composants);
            }
            set
            {
                savedGroup = new UnitGroup(value.Composants);
                icon.sprite = value.GetSprite();
                numberUnitInGroup.text = value.GetNumber().ToString();
            }
        }

        public void addToGroup(UnitGroup groupToAdd)
        {
            SavedGroup.Add(new UnitGroup(groupToAdd.Composants));
            numberUnitInGroup.text = SavedGroup.GetNumber().ToString();
        }

        private void Awake()
        {
            numberUnitInGroup = GetComponentInChildren<Text>();
            icon = this.transform.GetChild(0).GetComponent<Image>();
        }

        public void OnRemove()
        {
            savedGroup = new UnitGroup();
            this.gameObject.SetActive(false);
        }
    }
}