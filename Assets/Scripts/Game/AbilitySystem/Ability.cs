using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnitComponent;
using UnityEngine;

namespace AbilitySystem
{
    public abstract class Ability : ScriptableObject
    {
        [SerializeField] private string name;
        [SerializeField] private Sprite icon;
        [SerializeField] private String description;
        [SerializeField] private float cooldown;
        [SerializeField] private bool hasCastDelay;
        [SerializeField] private bool canCastOnSelf;
        [SerializeField] private bool requireTarget;
        [SerializeField] private float rangeInTiles;     
        [SerializeField] private List<AbilityTargetType> targetTypes;
        [SerializeField] private List<AbilityBehavior> abilityBehaviors;

        public string Name => name;
        public Sprite Icon => icon;
        public string Description => description;
        public float Cooldown => cooldown;
        public bool HasCastDelay => hasCastDelay;
        public bool CanCastOnSelf => canCastOnSelf;
        public bool RequireTarget => requireTarget;
        public float Range => rangeInTiles * .32f;
        public List<AbilityTargetType> TargetTypes => targetTypes;
        public List<AbilityBehavior> AbilityBehaviors=>abilityBehaviors;

        protected AbilityCaster caster;

        public virtual void Init(AbilityCaster caster)
        {
            this.caster = caster;
        }
        
        public abstract void Cast();
    }
}