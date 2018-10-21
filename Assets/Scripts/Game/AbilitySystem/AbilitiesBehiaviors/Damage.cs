using UnitComponent;
using UnityEngine;

namespace AbilitySystem
{
    public class Damage:AbilityBehavior
    {
        private float damage;
        public Damage(float damageValue,BehiaviorTime timeOfBehiaviorAction) : base(timeOfBehiaviorAction)
        {
            damage = damageValue;
        }

        public override void Do(AbilityCaster caster)
        { 
            if(caster.CurrentTarget!=null)
            caster.CurrentTarget.GetComponent<Status>().Damage(damage);
        }

        public override void Collision(Collider2D other)
        {
            other.GetComponent<Status>().Damage(damage);
        }
    }
}