using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

namespace Selection
{
    public class SavedSelectionSlot : MonoBehaviour
    {
        private Crowd savedGroup;
        private Text numberUnitInGroup;
        private Image icon;

        public Crowd SavedGroup
        {
            get
            {
                if (savedGroup == null)
                    return new Crowd();
                return new Crowd(savedGroup.Composants);
            }
            set
            {
                savedGroup = new Crowd(value.Composants);
                icon.sprite = value.GetSprite();
                numberUnitInGroup.text = value.Composants.Count.ToString();
            }
        }

        public void addToGroup(Crowd groupToAdd)
        {
            foreach (var u in groupToAdd.Composants)
            {
                SavedGroup.Add(u);
            }
            numberUnitInGroup.text = SavedGroup.Composants.Count.ToString();
        }

        private void Awake()
        {
            numberUnitInGroup = GetComponentInChildren<Text>();
            icon = this.transform.GetChild(0).GetComponent<Image>();
        }

        public void OnRemove()
        {
            savedGroup = new Crowd();
            this.gameObject.SetActive(false);
        }
    }
}