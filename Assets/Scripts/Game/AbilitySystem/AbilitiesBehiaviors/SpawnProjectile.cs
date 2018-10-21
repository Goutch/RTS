using AbilitySystem.Abilities;
using UnitComponent;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace AbilitySystem
{
    public class SpawnProjectile:AbilityBehavior
    {
        private Projectile projectile;
        public SpawnProjectile(Projectile projectile,BehiaviorTime timeOfBehiaviorAction):base(timeOfBehiaviorAction)
        {
            this.projectile = projectile;
        }
        
        public override void Do(AbilityCaster caster)
        {
            caster.SpawnProjectile(projectile);
        }
    }
}