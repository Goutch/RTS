using UnitComponent;
using UnityEngine;

namespace AbilitySystem
{
    public class Stun:AbilityBehavior
    {
        public Stun(float value,BehiaviorTime timeOfBehiaviorAction) : base(timeOfBehiaviorAction)
        {
        }

        public override void Do(AbilityCaster caster)
        {
           
        }
    }
}