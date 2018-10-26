using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitComponent;
using UnityEngine;

namespace DefaultNamespace
{
    public class Crowd
    {
        private List<UnitController> composants;
        private UnitController crowdLeader;

        public Vector2 CrowdLeaderPosition => crowdLeader.transform.position;

        public Crowd()
        {
            composants = new List<UnitController>();
        }

        public Crowd(List<UnitController> unitsList)
        {
            composants = new List<UnitController>(unitsList);
        }

        public List<UnitController> Composants
        {
            get
            {
                composants.RemoveAll(it => it == null);
                return composants;
            }
            set { composants = value; }
        }

        public void OverrideCommand(Command command)
        {
            DetermineLeader();
            if (command.SingleCommand)
            {
                bool executed = false;
                foreach (UnitController u in composants)
                {
                    if(u.OverrideCommand(command))
                        break;
                }
            }
            foreach (UnitController u in Composants)
            {
                u.OverrideCommand(command);
            }
        }

        public bool AddCommand(Command command)
        {
            DetermineLeader();
            if (command.SingleCommand)
            {
                bool executable = false;
                foreach (UnitController u in composants)
                {
                    executable = u.AddCommand(command);
                    if (executable == true)
                    {
                        return executable;
                    }
                }

                return executable;
            }

            foreach (UnitController u in composants)
            {
                u.AddCommand(command);
            }

            return true;
        }

        private void DetermineLeader()
        {
            if (crowdLeader == null&&composants.Any())
            {
                crowdLeader = composants.First();
            }
        }
        public void SetSelected(bool isSelected)
        {
            foreach (UnitController u in Composants)
            {
                u?.SetSelected(isSelected);
            }
        }

        public UnitData GetData()
        {
            foreach (UnitController u in Composants)
            {
                return u.Data;
            }
            return null;
        }

        public Sprite GetSprite()
        {
            return composants.First().Data.Sprite;
        }

        public void Add(UnitController unit)
        {
            if (!composants.Contains(unit))
                Composants.Add(unit);
        }

        public void Remove(UnitController unit)
        {
            Composants.Remove(unit);
        }
    }
}