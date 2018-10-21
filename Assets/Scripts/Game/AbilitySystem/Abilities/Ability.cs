using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnitComponent;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace AbilitySystem
{
    [CreateAssetMenu]
    public abstract class Ability : ScriptableObject
    {
        [SerializeField] private string name;
        [SerializeField] private Sprite icon;
        [SerializeField] private String description;
        [SerializeField] private float cooldown;
        [SerializeField] private bool hasCastDelay;
        [SerializeField] private bool requireTarget;
        [SerializeField] private float rangeInTiles;
        [SerializeField] private List<AbilityTargetType> targetTypes;
        private readonly List<AbilityBehavior> abilityBehaviors = new List<AbilityBehavior>();

        public string Name => name;
        public Sprite Icon => icon;
        public string Description => description;
        public float Cooldown => cooldown;
        public bool HasCastDelay => hasCastDelay;
        public bool RequireTarget => requireTarget;
        public float Range => rangeInTiles * .32f;
        public List<AbilityTargetType> TargetTypes => targetTypes;
        public List<AbilityBehavior> AbilityBehaviors => abilityBehaviors;

        protected AbilityCaster caster;

        public virtual bool CanCast(AbilityCaster caster)
        {
            if (TargetTypes.Contains(caster.TargetType))
                if (IsInRange(caster))
                    return true;
            return false;
        }

        public virtual bool IsInRange(AbilityCaster caster)
        {
            return true;
        }

        public virtual void StartAbility(AbilityCaster caster)
        {
            this.caster = caster;
            DoBehiaviours(AbilityBehavior.BehiaviorTime.Start);
        }

        public virtual void Cast()
        {
            DoBehiaviours(AbilityBehavior.BehiaviorTime.Cast);
        }

        public virtual void OnCollision(Collider2D other)
        {
            foreach (var behiavior in abilityBehaviors)
            {
                if (behiavior.ActTime == AbilityBehavior.BehiaviorTime.Collision)
                    behiavior.Collision(other);
            }
        }

        private void DoBehiaviours(AbilityBehavior.BehiaviorTime time)
        {
            foreach (var behiavior in abilityBehaviors)
            {
                if (behiavior.ActTime == time)
                    behiavior.Do(caster);
            }
        }
    }
}