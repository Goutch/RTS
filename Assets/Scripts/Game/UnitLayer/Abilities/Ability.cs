using UnityEngine;

namespace DefaultNamespace.Abilities
{
    public class Ability:ScriptableObject,IAbility
    {
        [SerializeField] private float Value;
        [SerializeField] private float Range;
        [SerializeField] private float cooldown;

        public float Cooldown => cooldown;
    }
}