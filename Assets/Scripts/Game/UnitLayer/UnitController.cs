using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public class UnitController : NetworkBehaviour, IUnit
    {
        private UnitInfo info;
        [SerializeField] private SpriteRenderer selectedCircle;
        private int teamID;

        public int TeamId => teamID;

        private UnitAI AI;
        private Mover mover;
        private Sight sight;
        private SpriteRenderer visual;

        private List<bool> AbilitesAvalable;


        public void Init(UnitInfo infoPrefab, int teamID, Transform parent)
        {
            transform.parent = parent;
            this.teamID = teamID;
            this.info = Instantiate(infoPrefab);
            this.info.Init();
            AbilitesAvalable = new List<bool>();
            mover = GetComponent<Mover>();
            sight = GetComponentInChildren<Sight>();
            visual = GetComponentInChildren<SpriteRenderer>();

            AI = Instantiate(info.AI);
            AI.Init(mover, sight, info);

            selectedCircle.transform.localScale = (info.size.Value / 32) * .5f * Vector2.one;

            this.name = info.Name;
            this.visual.color = info.Color;

            this.GetComponent<Rigidbody2D>().mass = info.mass.Value;
            this.GetComponentInChildren<SpriteRenderer>().sprite = info.Sprite;
            this.GetComponent<CircleCollider2D>().radius = info.size.Value / 200;

            this.GetComponent<Selectable>().Init();
        }

        private void Update()
        {
            if (AI != null)
                AI.DoSomeThing();
        }

        public bool OverrideCommand(Command command)
        {
            return AI.Execute(command);
        }

        public void SetSelected(bool isSelected)
        {
            selectedCircle.enabled = isSelected;
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

        public bool AddCommand(Command command)
        {
            return AI.AddCommand(command);
        }
        public void CastAbility(int Index)
        {
            if (hasAuthority)
                if (AbilitesAvalable[Index])
                {
                    StartCoroutine("StartAbilityRoutine",Index);
                    CmdCastAbility(Index);
                }
        }

        private IEnumerable StartAbilityRoutine(int Index)
        {
            AbilitesAvalable[Index] = false;
            //start animation
            //todo: network.spawn projectile or serveronly callback on projevtiles so it only get called on server
            yield return new WaitForSeconds(info.Abilities[Index].Cooldown);

            AbilitesAvalable[Index] = true;
        }

        [Command]
        private void CmdCastAbility(int index)
        {
            if (!isLocalPlayer)
                if (AbilitesAvalable[index])
                {
                    StartCoroutine("StartAbilityRoutine", index);
                    RpcCastAbility(index);
                }
        }

        [ClientRpc]
        private void RpcCastAbility(int index)
        {
            StartCoroutine("StartAbilityRoutine", index);
        }
    }
}