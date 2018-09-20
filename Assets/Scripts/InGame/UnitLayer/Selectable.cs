using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public class Selectable : NetworkBehaviour
    {
        private SelectionManager manager;
        private UnitController controller;

        public void Init()
        {
            manager = transform.root.GetComponent<SelectionManager>();

            controller = GetComponent<UnitController>();

            if (transform.root.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                manager.OnSelectionEnd += OnSelectionEnd;
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
            if (!hasAuthority)
                manager.SelectedUnits.Add(controller);
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