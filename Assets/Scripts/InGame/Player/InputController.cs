using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public class InputController : NetworkBehaviour
    {
        [SerializeField] private KeyCode addKey = KeyCode.LeftControl;
        private SelectionManager selectionManager;
        private PlayerManager playerManager;

        private void Awake()
        {
            selectionManager = GetComponent<SelectionManager>();
            playerManager = GetComponent<PlayerManager>();
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
                //if didnt click on the ui
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    RaycastHit2D hit =
                        Physics2D.Raycast(
                            new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                                Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f);
                    //hit the ground
                    if (!hit.collider)
                    {
                        if (Input.GetKey(addKey))
                        {
                            selectionManager.SelectedUnits.AddCommand(new Command(
                                Command.CommandType.Move,
                                Camera.main.ScreenToWorldPoint(Input.mousePosition),
                                false));
                        }
                        else
                        {
                            Command command = new Command(
                                Command.CommandType.Move,
                                Camera.main.ScreenToWorldPoint(Input.mousePosition),
                                false);
                            selectionManager.SelectedUnits.OverrideCommand(command);
                            CmdSendBasicCommand(command.Target, (int) command.Type, command.IsSingle);
                        }
                    }
                    //other unit
                    else if (hit.collider.tag == "Unit")
                    {
                        //other team
                        //if (hit.collider.GetComponent<UnitController>().TeamId != playerManager.TeamId)
                        //{
                        Command command = new Command(
                            Command.CommandType.Attack,
                            hit.transform,
                            false);
                        selectionManager.SelectedUnits.OverrideCommand(command);
                        CmdSendComplexeCommmand(command.TargetTransform.GetComponent<NetworkIdentity>().netId,
                            (int) command.type, command.IsSingle);
                        // }
                        //my team
                        //else
                        //{

                        //}
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                    selectionManager.BeginSelection(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                    selectionManager.DrawSelectionSquare(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    if (Input.GetKey(addKey))
                        selectionManager.EndSelection(true);
                    else
                        selectionManager.EndSelection(false);
                }
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
        private void CmdSendBasicCommand(Vector2 pos, int type, bool isSingle)
        {
            RpcSendBasicCommand(pos, type, isSingle);
        }

        [ClientRpc]
        private void RpcSendBasicCommand(Vector2 pos, int type, bool isSingle)
        {
            if (!hasAuthority)
            {
                Command.CommandType commandType = (Command.CommandType) type;
                selectionManager.SelectedUnits.OverrideCommand(new Command(commandType, pos, isSingle));
            }
        }

        [Command]
        private void CmdSendComplexeCommmand(NetworkInstanceId netID, int type, bool isSingle)
        {
            RpcSendComplexeCommand(netID, type, isSingle);
        }

        [ClientRpc]
        private void RpcSendComplexeCommand(NetworkInstanceId netID, int type, bool isSingle)
        {
            if (!hasAuthority)
            {
                Command.CommandType commandType = (Command.CommandType) type;
                selectionManager.SelectedUnits.OverrideCommand(new Command(commandType,
                    ClientScene.FindLocalObject(netID).transform, isSingle));
            }
        }
    }
}