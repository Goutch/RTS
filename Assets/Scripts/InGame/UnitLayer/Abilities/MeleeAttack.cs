using UnityEngine;

namespace DefaultNamespace.Abilities
{
    [CreateAssetMenu( menuName = "Ability/MeleeAttack")]
    public class MeleeAttack:Ability
    {
        [SerializeField] private float damageValue;
        [SerializeField] private float rangeInTile;
        public override void Cast()
        {
            caster.CurrentTarget.GetComponent<Health>().Damage(damageValue);       
        }
    }
}