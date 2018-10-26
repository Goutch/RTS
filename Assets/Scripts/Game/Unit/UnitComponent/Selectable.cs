using DefaultNamespace;
using Selection;
using UnityEngine;
using UnityEngine.Networking;

namespace UnitComponent
{
    
public class Selectable : NetworkBehaviour
{
    private SelectionManager manager;
    private UnitController controller;
    private Crowd currentCrowd;

    public Crowd CurrentCrowd => currentCrowd;

    public void InitClient()
    {
        manager = transform.root.GetComponent<SelectionManager>();
        controller = GetComponent<UnitController>();
    }

    public void InitAuthority()
    {
        manager.OnSelectionEnd += OnSelectionEnd;
    }

    public override void OnNetworkDestroy()
    {
        if (hasAuthority)
        {
            manager.OnSelectionEnd -= OnSelectionEnd;
        }
        base.OnNetworkDestroy();
    }

    private void OnSelectionEnd(Rect bounds, Camera cam)
    {
        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
        screenPos.y = Screen.height - screenPos.y;
        if (bounds.Contains(screenPos)) // && !manager.SelectedUnits.Composants.Contains(controller))
        {
            manager.SelectedUnits.Composants.Add(controller);
            controller.SetSelected(true);
            CmdOnAddSelection();
            currentCrowd = manager.SelectedUnits;
            return;
        }

        currentCrowd = null;
        controller.SetSelected(false);
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
}
