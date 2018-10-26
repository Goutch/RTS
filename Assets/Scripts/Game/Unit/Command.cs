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

        private bool _singleCommand;

        public CommandType Type => type;

        public Transform TargetTransform => targetTransform;

        public Vector2 Target => target;

        public bool SingleCommand => _singleCommand;


        public Command(CommandType type, Vector2 target,bool singleCommand)
        {
            this.target = target;
            this.type = type;
            this._singleCommand =singleCommand;
        }

        public Command(CommandType type, Transform targetTransform,bool singleCommand)
        {
            this.targetTransform = targetTransform;
            this.type = type;
            this._singleCommand =singleCommand;
        }

        public Command()
        {
            
        }
    }
}