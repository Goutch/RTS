using UnityEngine;
using UnityEngine.UI;

namespace Selection
{
    public class SelectedUnitAbilitiesUI : MonoBehaviour
    {
        [SerializeField] private GameObject abilitySlotPrefab;
        private AbilitySlot[] abilitySlots;
        private SelectionManager selectionManager;
        private UnitData currentUnitData;
        private Image backGround;

        private void Start()
        {
            selectionManager = transform.root.GetComponent<SelectionManager>();
            abilitySlots = new AbilitySlot[8];
            for (int i = 0; i < abilitySlots.Length; i++)
            {
                abilitySlots[i] = Instantiate(abilitySlotPrefab, transform).GetComponentInChildren<AbilitySlot>();
                abilitySlots[i].Hide();
            }

            backGround = GetComponent<Image>();

            backGround.enabled = false;
        }

        public void UpdateAbilitesSlots(UnitData data)
        {
            if (data != null)
            {
                backGround.enabled = true;
                for (int i = 0; i < abilitySlots.Length; i++)
                {
                    if (i < data.Abilities.Count)
                    {
                        abilitySlots[i].Display();
                        abilitySlots[i].SetAbility(data.Abilities[i]);
                    }
                    else
                    {
                        abilitySlots[i].Hide();
                    }
                }
            }
            else
            {
                for (int i = 0; i < abilitySlots.Length; i++)
                {
                    abilitySlots[i].Hide();
                }

                backGround.enabled = false;
            }
        }
    }
}