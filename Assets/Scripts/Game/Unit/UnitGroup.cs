using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class UnitGroup : IUnit
    {
        private List<IUnit> composants;

        public UnitGroup()
        {
            composants = new List<IUnit>();
        }

        public UnitGroup(List<IUnit> unitsList)
        {
            composants = new List<IUnit>(unitsList);
        }

        public List<IUnit> Composants
        {
            get
            {
                composants.RemoveAll(it => it == null);
                return composants;
            }
            set { composants = value; }
        }

        public bool OverrideCommand(Command command)
        {
            if (command.IsSingle)
            {
                bool executable = false;
                foreach (IUnit u in composants)
                {
                    executable = u.OverrideCommand(command);
                    if (executable == true)
                    {
                        return executable;
                    }
                }

                return executable;
            }

            foreach (IUnit u in Composants)
            {
                u.OverrideCommand(command);
            }

            return true;
        }

        public bool AddCommand(Command command)
        {
            if (command.IsSingle)
            {
                bool executable = false;
                foreach (IUnit u in composants)
                {
                    executable = u.AddCommand(command);
                    if (executable == true)
                    {
                        return executable;
                    }
                }

                return executable;
            }

            foreach (IUnit u in composants)
            {
                u.AddCommand(command);
            }

            return true;
        }

        public void SetSelected(bool isSelected)
        {
            
            foreach (IUnit u in Composants)
            {
                u?.SetSelected(isSelected);
            }
        }

        public UnitData GetData()
        {
            foreach (IUnit u in Composants)
            {
                return u.GetData();
            }

            return null;
        }

        public Sprite GetSprite()
        {
            return composants.First().GetSprite();
        }

        public int GetNumber()
        {
            int number = 0;
            for (int i = 0; i < composants.Count; i++)
            {
                number += composants[i].GetNumber();
            }

            return number;
        }

        public bool Contains(IUnit unit)
        {
            bool contain = false;
            for (int i = 0; i < composants.Count; i++)
            {
                if (composants[i].Contains(unit))
                    contain = true;
            }

            return contain;
        }

        public void Add(IUnit unit)
        {
            for (int i = 0; i < composants.Count; i++)
            {
                unit.Contains(composants[i]);
            }

            Composants.Add(unit);
        }

        public void Remove(IUnit unit)
        {
            Composants.Remove(unit);
        }
    }
}