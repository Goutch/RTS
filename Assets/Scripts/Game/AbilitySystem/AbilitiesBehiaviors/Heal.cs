using UnitComponent;
using UnityEngine;

namespace AbilitySystem
{
    public class Heal:AbilityBehavior
    {
        public Heal(float value,BehiaviorTime timeOfBehiaviorAction) : base(timeOfBehiaviorAction)
        {
        }


        public override void Do(AbilityCaster caster)
        {
            
        }
    }
}