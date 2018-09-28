using UnityEngine;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public class InputController : NetworkBehaviour
    {
        [SerializeField] private KeyCode addKey = KeyCode.LeftControl;
        private SelectionManager selectionManager;

        private void Awake()
        {
            selectionManager = GetComponent<SelectionManager>();
        }

        private void Update()
        {
            if (!hasAuthority)
            {
                return;
            }

            #region mouse

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (Input.GetKey(addKey))
                {
                    //call rpc
                    selectionManager.SelectedUnits.AddCommand(new Command(
                        Command.CommandType.Move,
                        Camera.main.ScreenToWorldPoint(Input.mousePosition),
                        false));
                }
                else
                {
                    //call rpc
                    Command command = new Command(
                        Command.CommandType.Move,
                        Camera.main.ScreenToWorldPoint(Input.mousePosition),
                        false);
                    selectionManager.SelectedUnits.OverrideCommand(command);
                    CmdSendBasicCommand(command.Target, command.Type.ToString(), command.IsSingle);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                selectionManager.BeginSelection(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                selectionManager.DrawSelectionSquare(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (Input.GetKey(addKey))
                    selectionManager.EndSelection(true);
                else
                    selectionManager.EndSelection(false);
            }

            #endregion

            #region selectionSaving

            if (Input.GetKeyDown(KeyCode.F1))
            {
                selectionManager.SaveSelection(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectionManager.SelectSavedSelection(0);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                selectionManager.SaveSelection(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                selectionManager.SelectSavedSelection(1);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                selectionManager.SaveSelection(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                selectionManager.SelectSavedSelection(2);
            }

            #endregion
        }

        //todo:optimize
        [Command]
        private void CmdSendBasicCommand(Vector2 pos, string type, bool isSingle)
        {
            if (!isLocalPlayer)
            {
                Command.CommandType commandType;
                Command.CommandType.TryParse(type, true, out commandType);
                selectionManager.SelectedUnits.OverrideCommand(new Command(commandType, pos, isSingle));
            }

            RpcSendBasicCommand(pos, type, isSingle);
        }

        [ClientRpc]
        private void RpcSendBasicCommand(Vector2 pos, string type, bool isSingle)
        {
            if (!isLocalPlayer)
            {
                Command.CommandType commandType;
                Command.CommandType.TryParse(type, true, out commandType);
                selectionManager.SelectedUnits.OverrideCommand(new Command(commandType, pos, isSingle));
            }
        }
    }
}