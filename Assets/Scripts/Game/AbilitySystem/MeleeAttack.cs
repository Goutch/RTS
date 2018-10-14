using UnitComponent;
using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu( menuName = "Ability/MeleeAttack")]
    public class MeleeAttack:Ability
    {
        [SerializeField] private float damageValue;
        public override void Cast()
        {
            caster.CurrentTarget?.GetComponent<Health>().Damage(damageValue);       
        }
    }
}