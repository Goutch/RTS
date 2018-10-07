using DefaultNamespace;
using UnityEngine;
using UnityEngine.Networking;


public class Selectable : NetworkBehaviour
{
    private SelectionManager manager;
    private UnitController controller;



    public void Init()
    {
        manager = transform.root.GetComponent<SelectionManager>();
        manager.OnSelectionEnd += OnSelectionEnd;
        controller = GetComponent<UnitController>();
        if (hasAuthority)
        {
            manager.OnSelectionEnd += OnSelectionEnd;
        }
    }

    private void OnDisable()
    {
        if (hasAuthority)
        {
            //manager.OnSelectionEnd -= OnSelectionEnd;
        }
    }

    private void OnSelectionEnd(Rect bounds, Camera cam)
    {
        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
        screenPos.y = Screen.height - screenPos.y;
        if (bounds.Contains(screenPos) && !manager.SelectedUnits.Composants.Contains(controller))
        {
            manager.SelectedUnits.Add(controller);
            CmdOnAddSelection();
        }
    }

    [Command]
    private void CmdOnAddSelection()
    {
        RpcOnAddSelection();
    }

    [ClientRpc]
    private void RpcOnAddSelection()
    {
        if (!hasAuthority)
            manager.SelectedUnits.Add(controller);
    }
}