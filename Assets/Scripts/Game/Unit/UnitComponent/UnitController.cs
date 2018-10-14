using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Player;
using UnityEngine;
using UnityEngine.Networking;

namespace UnitComponent
{
    public class UnitController : NetworkBehaviour, IUnit
    {
        [SerializeField] private SpriteRenderer selectedCircle;
        private UnitData data;
        public UnitData Data => data;
        private UnitAI AI;
        private int teamID;
        private Mover mover;
        private Sight sight;
        private AbilityCaster abilityCaster;
        private SpriteRenderer visual;
        private UnitAnimationController unitAnimationController;

        public int TeamId => teamID;
        private Queue<Command> commandsQueue;

        public void Init(UnitData dataPrefab, int teamID, Transform parent)
        {
            transform.parent = parent;
            this.teamID = teamID;
            this.data = Instantiate(dataPrefab);
            this.data.Init();

            sight = GetComponentInChildren<Sight>();
            visual = GetComponentInChildren<SpriteRenderer>();
            mover = GetComponent<Mover>();
            abilityCaster = GetComponentInChildren<AbilityCaster>();
            unitAnimationController = GetComponent<UnitAnimationController>();
            visual.GetComponent<Animator>().runtimeAnimatorController = data.AnimsController;

            AI = Instantiate(data.AI);
            AI.Init(mover, sight, abilityCaster, unitAnimationController, data);

            unitAnimationController.Init(visual.GetComponent<Animator>(), abilityCaster, mover);
            abilityCaster.Init(data, AI,parent.GetComponent<PlayerManager>().MyNetworkPlayer.GetComponent<Spawner>());
            mover.Init(transform, visual.transform);
            selectedCircle.transform.localScale = (data.size.Value / 32) * .5f * Vector2.one;

            this.name = data.Name;
            this.GetComponent<Health>().Init(data);
            this.GetComponent<Selectable>().InitClient();
            this.GetComponent<Rigidbody2D>().mass = data.mass.Value;
            this.GetComponentInChildren<SpriteRenderer>().sprite = data.Sprite;
            this.GetComponent<CircleCollider2D>().radius = data.size.Value / 200;

            commandsQueue = new Queue<Command>();
        }

        public void InitAuthority()
        {
            this.GetComponent<Selectable>().InitAuthority();
        }

        private void FixedUpdate()
        {
            if (AI != null)
            {
                if (commandsQueue.Any() && !AI.HasCommand)
                {
                    AI.Execute(commandsQueue.Dequeue());
                }

                AI.DoSomeThing();
            }
        }

        public bool OverrideCommand(Command command)
        {
            commandsQueue.Clear();
            return AI.Execute(command);
        }

        public bool AddCommand(Command command)
        {
            if (AI.CanExecuteCommand(command.type))
            {
                commandsQueue.Enqueue(command);
                return true;
            }

            return false;
        }

        public void SetSelected(bool isSelected)
        {
            selectedCircle.enabled = isSelected;
        }

        public UnitData GetData()
        {
            return data;
        }

        public Sprite GetSprite()
        {
            return visual.sprite;
        }

        public int GetNumber()
        {
            return 1;
        }

        public bool Contains(IUnit unit)
        {
            if (unit == (IUnit) this)
            {
                return true;
            }

            return false;
        }

        public IEnumerator WaitForHauthorityRoutine()
        {
            while (!hasAuthority)
            {
                yield return 0;
            }

            InitAuthority();
        }
    }
}