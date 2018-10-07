using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "BaseAI", menuName = "AI/Base")]
    public class UnitAI : ScriptableObject
    {
        [SerializeField] private Command.CommandType[] executableCommands;
        private Mover mover;
        private Sight sight;
        private Animator animator;
        private UnitData _data;
        private List<Vector3> path;
        private Command command;
        private Queue<Command> commandsQueue;


        public void Init(Mover mover, Sight sight, Animator animator, UnitData data)
        {
            this.mover = mover;
            this.sight = sight;
            this.animator = animator;
            this._data = data;
            path = new List<Vector3>();
            commandsQueue = new Queue<Command>();
        }


        //idle
        public virtual void DoSomeThing()
        {
            if (command == null && commandsQueue.Any())
            {
                command = commandsQueue.Dequeue();
            }

            if (command != null)
            {
                switch (command.type)
                {
                    case Command.CommandType.Move:
                        Move(command.Target, 0.2f);
                        break;
                    case Command.CommandType.Attack:

                        break;
                }
            }
            else
            {
                idle();
            }
        }

        //what the unit do when it doesent have any commands
        protected virtual void idle()
        {
            
        }

        //set Command
        public bool Execute(Command command)
        {
            if (executableCommands.Contains(command.type))
            {
                this.command = command;
                commandsQueue.Clear();
                if (path != null)
                    path.Clear();
                return true;
            }

            return false;
        }

        public bool AddCommand(Command command)
        {
            if (executableCommands.Contains(command.type))
            {
                commandsQueue.Enqueue(command);
                return true;
            }

            return false;
        }

        private void OnCommandExecuted()
        {
            command = null;
            animator.SetTrigger("Idle");
        }

        private void Move(Vector2 target, float precision)
        {
            if (path == null || !path.Any())
            {
                path = Pathfinder.INSTANCE.FindPath(mover.transform.position, target);
                //path was impossible
                if (path == null)
                {
                    command = null;
                    return;
                }
                animator.SetTrigger("Walk");
            }

            if (path.Any() && Vector2.Distance(mover.transform.position, path.First()) < precision)
            {
                path.RemoveAt(0);
                //reached destination
                if (!path.Any())
                {                 
                    OnCommandExecuted();
                    return;
                }
            }


            mover.MoveAndRotateToward(path.First(), _data.speed.Value);
        }

        private void Attack(Transform targeTransform)
        {
            if (targeTransform == null)
                OnCommandExecuted();
            if (Vector2.Distance(mover.transform.position, targeTransform.position) <= _data.attackRange.Value)
            {
            }
            else
                Move(targeTransform.position, 0.2f);
        }
    }
}