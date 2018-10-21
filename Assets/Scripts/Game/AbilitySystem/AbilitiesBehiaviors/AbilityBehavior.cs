using UnitComponent;
using UnityEngine;

namespace AbilitySystem
{
    public abstract class AbilityBehavior
    {
        private BehiaviorTime actTime;
        public enum BehiaviorTime
        {
            Start,
            Collision,
            Cast,
            End
        }

        public BehiaviorTime ActTime => actTime;

        public AbilityBehavior(BehiaviorTime timeOfBehiaviorAction)
        {
            this.actTime = timeOfBehiaviorAction;
        }

        public abstract void Do(AbilityCaster caster);

        public virtual void Collision(Collider2D other)
        {
            
        }
    }
}