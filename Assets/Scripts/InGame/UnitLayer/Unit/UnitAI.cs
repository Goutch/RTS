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
        private UnitData unitData;
        private Command command;
        private UnitController myController;
        private AbilityCaster abilityCaster;
        private UnitAnimationController animationController;
        public bool HasCommand => command != null;
        public Command CurrentCommand => command;

        public void Init(Mover mover, Sight sight, Animator animator, AbilityCaster abilityCaster,
            UnitAnimationController animationController, UnitData data)
        {
            this.animationController = animationController;
            myController = mover.GetComponent<UnitController>();
            this.mover = mover;
            mover.OnMovingChange += HandleMovingChange;
            this.sight = sight;
            this.animator = animator;
            this.abilityCaster = abilityCaster;
            this.unitData = data;
        }

        private void HandleMovingChange(bool moving)
        {
            if (!moving)
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
                        Move(command.Target);
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
                mover.ClearPath();
                this.command = command;
                return true;
            }

            return false;
        }


        private void OnCommandExecuted()
        {
            command = null;
        }

        private void Move(Vector2 target)
        {
            mover.MoveAndRotateToward(target, unitData.speed.Value);
        }

        private void Attack(Transform targetTransform)
        {
            if (targetTransform == null)
            {
                OnCommandExecuted();
                return;
            }

            if (IsAbilityInRange(targetTransform))
            {
                abilityCaster.CastAbility(0);
            }
            else
                mover.MoveAndRotateToward(targetTransform.position, unitData.speed.Value);
        }

        private bool IsAbilityInRange(Transform targetPos)
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