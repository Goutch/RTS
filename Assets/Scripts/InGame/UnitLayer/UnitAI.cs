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
        private bool isMoving = false;
        private UnitController myController;

        public void Init(Mover mover, Sight sight, Animator animator, UnitData data)
        {
            myController = mover.GetComponent<UnitController>();
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
                        Attack(command.TargetTransform);
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
            //if (sight.EnemyUnitsInSight.Any())
            //{
            //    command=new Command(Command.CommandType.Attack,sight.EnemyUnitsInSight[0],true);
            //}
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
            animator.SetBool("isMoving", false);
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

                path.Add(target);
                animator.SetBool("isMoving", true);
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

        private void Attack(Transform targetTransform)
        {
            if (targetTransform == null)
            {
                OnCommandExecuted();
                return;
            }

            if (Vector2.Distance(mover.transform.position, targetTransform.position) -
                mover.GetComponent<CircleCollider2D>().radius -
                targetTransform.GetComponent<CircleCollider2D>().radius <= _data.Abilities[0].Range)
            {
                if (_data.Abilities[0].IsTargetted)
                    myController.CastComplexeAbility(0, targetTransform);
                else
                {
                    myController.CastBasicAbility(0, targetTransform.position);
                }
            }
            else
                Move(targetTransform.position, 0.2f);
        }
    }
}