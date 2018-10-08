using UnityEngine;

namespace DefaultNamespace.Abilities
{
    public abstract class Ability : ScriptableObject
    {
        [SerializeField] private float cooldown;
        [Tooltip("Lock the unit until animation trigger OnCastFinish ")]
        [SerializeField] private bool castLock;
        [Tooltip("range of the ability ")]
        [SerializeField]  private float rangeInTiles;

        public float Range => rangeInTiles*.32f;
        
        protected AbilityCaster caster;
        public bool CastLock => castLock;
        
        public float Cooldown => cooldown;
        public abstract void Cast();

        public virtual void Init(AbilityCaster caster)
        {
            this.caster = caster;
        }
    }
}