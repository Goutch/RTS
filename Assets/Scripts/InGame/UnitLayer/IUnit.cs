using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public interface IUnit
    {
        bool OverrideCommand(Command command);
        bool AddCommand(Command command);
        void SetSelected(bool isSelected);
        Sprite GetSprite();
        int GetNumber();
        bool Contains(IUnit unit);
    }
}