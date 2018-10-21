using UnitComponent;
using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu( menuName = "Ability/MeleeAttack")]
    public class MeleeAttack:Ability
    {
        [SerializeField] private float damageValue;

        private void OnEnable()
        {  
            AbilityBehaviors.Add(new Damage(damageValue,AbilityBehavior.BehiaviorTime.Cast));
        }
    }
}