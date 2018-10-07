using UnityEngine;

namespace DefaultNamespace.Abilities
{
    public abstract class Ability : ScriptableObject
    {
        [SerializeField] private float cooldown;
        [SerializeField] private bool isTargetted;
        [SerializeField] private float rangeInTile;
        [Tooltip("Lock the unit until animation trigger OnCastFinish in Controller")]
        [SerializeField] private bool castLock;

        public bool CastLock => castLock;

        public bool IsTargetted => isTargetted;
        public float Cooldown => cooldown;
        public float Range => rangeInTile*.32f;
        public abstract void Cast(Vector2 position);
        public abstract void Cast(Transform target);
    }
}