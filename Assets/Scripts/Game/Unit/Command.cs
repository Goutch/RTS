using UnityEngine;

namespace DefaultNamespace
{
    public class Command
    {
        public enum CommandType
        {
            Move,
            Attack,
            Mine,
            Harvest,
            ComplexeCast,
            BasicCast
        }

        public CommandType type;

        private Transform targetTransform;

        private Vector2 target;

        private bool isSingle;

        public CommandType Type => type;

        public Transform TargetTransform => targetTransform;

        public Vector2 Target => target;

        public bool IsSingle => isSingle;


        public Command(CommandType type, Vector2 target,bool isSingle)
        {
            this.target = target;
            this.type = type;
            this.isSingle =isSingle;
        }

        public Command(CommandType type, Transform targetTransform,bool isSingle)
        {
            this.targetTransform = targetTransform;
            this.type = type;
            this.isSingle =isSingle;
        }

        public Command()
        {
            
        }
    }
}