using AbilitySystem;
using UnityEngine;
using UnityEngine.UI;

namespace Selection
{
    public class AbilitySlot:MonoBehaviour
    {
        private Ability ability;
        private Image icon;
        public void  SetAbility(Ability ability)
        {
            icon = GetComponent<Image>();
            this.ability = ability;
            icon.sprite = ability.Icon;
            
        }

        public void Hide()
        {
            transform.parent.gameObject.SetActive(false);
        }

        public void Display()
        {
            transform.parent.gameObject.SetActive(true);
        }
        private void OnMouseOver()
        {
            //display info about ability
        }
    }
}