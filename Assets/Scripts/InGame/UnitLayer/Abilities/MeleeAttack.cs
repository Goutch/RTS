using UnityEngine;

namespace DefaultNamespace.Abilities
{
    [CreateAssetMenu( menuName = "Ability/MeleeAttack")]
    public class MeleeAttack:Ability
    {
        [SerializeField] private float damageValue;
        public override void Cast(Vector2 position)
        {
            
        }

        public override void Cast(Transform target)
        {
            target.GetComponent<Health>().Damage(damageValue);       
        }
    }
}