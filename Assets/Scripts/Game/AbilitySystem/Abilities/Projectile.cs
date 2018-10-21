using UnitComponent;
using UnityEngine;

namespace AbilitySystem.Abilities
{
    [CreateAssetMenu( menuName = "Ability/Projectile")]
    public class Projectile:Ability
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private float projectileDamage;
        [SerializeField] private float speed;
        [SerializeField] private float collisionRadius;
        [SerializeField] private float lifeSpan;
        public float CollisionRadius => collisionRadius*.32f;
        public Sprite Sprite => sprite;
        public float Speed => speed;
        public float LifeSpan=>lifeSpan;

        private void OnEnable()
        {  
            AbilityBehaviors.Add(new Damage(projectileDamage,AbilityBehavior.BehiaviorTime.Collision));
            AbilityBehaviors.Add(new SpawnProjectile(this, AbilityBehavior.BehiaviorTime.Cast));
        }
    }
}