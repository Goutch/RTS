using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

namespace Selection
{
    
public class SavedSelectionGrid : MonoBehaviour
{
    private SelectionManager manager;
    private SavedSelectionSlot[] savedSelectionSlots;
    private void Start()
    {
        savedSelectionSlots = GetComponentsInChildren<SavedSelectionSlot>();
        foreach (SavedSelectionSlot slot in savedSelectionSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }

    public void SaveSelection(UnitGroup group,int index)
    {
        savedSelectionSlots[index].gameObject.active = true;
        savedSelectionSlots[index].SavedGroup=group;
        //cant acces sprite from group
    }

    public void AddToSavedSelection(UnitGroup group,int index)
    {
        savedSelectionSlots[index].addToGroup(group);
    }

    public bool IsGroupSaved(int index)
    {
        if (savedSelectionSlots[index].isActiveAndEnabled)
        {
            return true;
        }

        return false;
    }

    public UnitGroup GetSelection(int index)
    {
        return savedSelectionSlots[index].SavedGroup;
    }

}
}