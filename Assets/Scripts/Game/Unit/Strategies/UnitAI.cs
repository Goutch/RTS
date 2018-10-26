using System.Linq;
using UnitComponent;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "BaseAI", menuName = "AI/Base")]
    public class UnitAI : ScriptableObject
    {
        [SerializeField] private Command.CommandType[] executableCommands;
        protected Mover mover;
        protected Sight sight;
        protected UnitData unitData;
        protected Command command;
        protected UnitController myController;
        protected AbilityCaster abilityCaster;
        protected UnitAnimationController animationController;
        protected Selectable selectable;
        protected Vector2 crowdDestination;
        public bool HasCommand => command != null;
        public Command CurrentCommand => command;

        public void Init(Mover mover, Sight sight,  AbilityCaster abilityCaster,
            UnitAnimationController animationController,Selectable selectable ,UnitData data)
        {
            this.animationController = animationController;
            myController = mover.GetComponent<UnitController>();
            this.mover = mover;
            mover.OnMovingChange += HandleMovingChange;
            this.selectable = selectable;
            this.sight = sight;
            this.abilityCaster = abilityCaster;
            this.unitData = data;
        }

        protected  void HandleMovingChange(bool moving)
        {
            if (CurrentCommand!=null&&CurrentCommand.type == Command.CommandType.Move && !moving)
            {
                OnCommandExecuted();
            }
        }

        //idle
        public virtual void DoSomeThing()
        {
            if (command != null)
            {
                switch (command.type)
                {
                    case Command.CommandType.Move:
                        Move(crowdDestination);
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
            if (sight.EnemyUnitsInSight.Any())
            {
                command=new Command(Command.CommandType.Attack,sight.EnemyUnitsInSight[0],true);
            }
        }

        //set Command
        public bool Execute(Command command)
        {
            if (executableCommands.Contains(command.type))
            {
                Vector2 leaderOffSet = (Vector2)myController.transform.position-selectable.CurrentCrowd.CrowdLeaderPosition;
                crowdDestination = command.Target + leaderOffSet;
                mover.ClearPath();
                this.command = command;
                return true;
            }

            return false;
        }


        public void OnCommandExecuted()
        {
            command = null;
            mover.ClearPath();
        }

        protected  void Move(Vector2 target)
        {
            
            mover.MoveAndRotateToward(target, unitData.speed.Value);
        }

        protected  void Attack(Transform targetTransform)
        {
            if (targetTransform == null)
            {
                OnCommandExecuted();
                return;
            }

            if (IsAbilityInRange(targetTransform))
            {
                mover.RotateToward(targetTransform.position);
                mover.ClearPath();
                abilityCaster.CastAbility(0);
            }
            else
                mover.MoveAndRotateToward(targetTransform.position, unitData.speed.Value);
        }

        protected  bool IsAbilityInRange(Transform targetPos)
        {
            return unitData.Abilities[0].Range == -1 || Vector2.Distance(mover.transform.position, targetPos.position) -
                   mover.GetComponent<CircleCollider2D>().radius -
                   targetPos.GetComponent<CircleCollider2D>().radius <= unitData.Abilities[0].Range;
        }

        public bool CanExecuteCommand(Command.CommandType type)
        {
            return executableCommands.Contains(type);
        }
    }
}