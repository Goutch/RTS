
using DefaultNamespace;
using UnityEngine;


    public interface IUnit
    {
        bool OverrideCommand(Command command);
        bool AddCommand(Command command);
        void SetSelected(bool isSelected);
        UnitData GetData();
        Sprite GetSprite();
        int GetNumber();
        bool Contains(IUnit unit);
    }
