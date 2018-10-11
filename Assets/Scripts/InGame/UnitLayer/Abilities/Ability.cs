using System;
using UnityEngine;

namespace DefaultNamespace.Abilities
{
    public abstract class Ability : ScriptableObject
    {
        [Header("UI")] [SerializeField] private Sprite icon;

        [SerializeField] private String description;
        
        [Header("Game")]
        [SerializeField] private float cooldown;

        [Tooltip("Lock the unit until animation trigger OnCastFinish ")] [SerializeField]
        private bool hasCastDelay;

        [Tooltip("range of the ability ")] [SerializeField]
        private float rangeInTiles;

        public Sprite Icon => icon;

        public string Description => description;

        public float Range => rangeInTiles * .32f;

        protected AbilityCaster caster;
        public bool HasCastDelay => hasCastDelay;

        public float Cooldown => cooldown;
        public abstract void Cast();

        public virtual void Init(AbilityCaster caster)
        {
            this.caster = caster;
        }
    }
}