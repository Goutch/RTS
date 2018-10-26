using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Player;
using UnityEngine;
using UnityEngine.Networking;

namespace UnitComponent
{
    public class UnitController : NetworkBehaviour
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
            abilityCaster = GetComponent<AbilityCaster>();
            unitAnimationController = GetComponent<UnitAnimationController>();
            visual.GetComponent<Animator>().runtimeAnimatorController = data.AnimsController;
            Selectable selectable = this.GetComponent<Selectable>();
            AI = Instantiate(data.AI);
            AI.Init(mover, sight, abilityCaster, unitAnimationController,selectable, data);

            unitAnimationController.Init(visual.GetComponent<Animator>(), abilityCaster, mover);
            abilityCaster.Init(data, AI,parent.GetComponent<PlayerManager>().MyNetworkPlayer.GetComponent<Spawner>());
            mover.Init(transform, visual.transform);
            selectedCircle.transform.localScale = (data.size.Value / 24)* Vector2.one;

            this.name = data.Name;
            this.GetComponent<Status>().Init(data);
            selectable.InitClient();
          this.GetComponent<Rigidbody2D>().mass = data.mass.Value;
            this.GetComponentInChildren<SpriteRenderer>().sprite = data.Sprite;
            GetComponent<CircleCollider2D>().radius= (data.size.Value / 32)*.35f;
            sight.GetComponent<CircleCollider2D>().radius = data.sightRange;

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