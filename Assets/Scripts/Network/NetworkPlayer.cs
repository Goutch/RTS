using System;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Networking;

namespace AppLayer
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private GameObject lobbyPlayerPrefab;
        [SerializeField] private GameObject GamePlayerPrefab;


        [SyncVar(hook = nameof(OnMyName))] private string playerName = "";
        [SyncVar(hook = nameof(OnMyFaction))] private int factionIndex = 0;
        [SyncVar(hook = nameof(OnMyTeam))] private int teamID = 0;

        private RTSNetworkManager _networkManager;

        private GameObject lobbyPlayerObject;
        private GameObject gamePlayerObject;

        public override void OnStartClient()
        {
            DontDestroyOnLoad(this);
            _networkManager = RTSNetworkManager.singleton.GetComponent<RTSNetworkManager>();
            _networkManager.RegisterPlayer(this);
            _networkManager.OnNetworkStateChange += HandleNetworkStateChange;
            Debug.Log("Client Network Player start");
        }

        public override void OnNetworkDestroy()
        {
            Debug.Log("Client Network Player OnNetworkDestroy");

            if (lobbyPlayerObject != null)
            {
                Destroy(lobbyPlayerObject.gameObject);
            }

            if (gamePlayerObject != null)
            {
                Destroy(gamePlayerObject.gameObject);
            }

            _networkManager.UnRegisterPlayer(this);
            base.OnNetworkDestroy();
        }

        private void HandleNetworkStateChange()
        {
            switch (_networkManager.CurrentNetworkState)
            {
                case RTSNetworkManager.NetworkState.Disconected:
                    OnNetworkDestroy();
                    break;
                case RTSNetworkManager.NetworkState.LoadingGame:
                    break;
                case RTSNetworkManager.NetworkState.PreGameLobby:
                    OnEnterLobby();
                    break;
                case RTSNetworkManager.NetworkState.InGame:
                    OnEnterGame();
                    break;
                case RTSNetworkManager.NetworkState.EndGameLobby:
                    break;
            }
        }

        #region SyncVarCallBacks

        private void OnMyName(String newName)
        {
            playerName = newName;
            if (gamePlayerObject != null)
            {
                gamePlayerObject.name = playerName;
            }

            if (lobbyPlayerObject != null)
            {
                lobbyPlayerObject.GetComponent<LobbyPlayerManager>().SetPlayerName(newName);
            }
        }

        private void OnMyFaction(int newIndex)
        {
            factionIndex = newIndex;
            if (gamePlayerObject != null)
            {
            }

            if (lobbyPlayerObject != null)
            {
                lobbyPlayerObject.GetComponent<LobbyPlayerManager>().SetFactionIndex(newIndex);
            }
        }

        private void OnMyTeam(int newTeamID)
        {
            teamID = newTeamID;
            if (lobbyPlayerObject != null)
            {
                lobbyPlayerObject.GetComponent<LobbyPlayerManager>().SetTeamId(newTeamID);
            }
        }

        #endregion


        [Client]
        public void OnEnterGame()
        {
            Debug.Log("OnEnterGame");
            if (isLocalPlayer)
            {
                gamePlayerObject = Instantiate(GamePlayerPrefab);
                NetworkServer.SpawnWithClientAuthority(gamePlayerObject, connectionToClient);
            }
        }

        [Client]
        public void OnEnterLobby()
        {
            Debug.Log("OnEnterLobby");
            if (isLocalPlayer)
            {
                lobbyPlayerObject = Instantiate(lobbyPlayerPrefab);
                NetworkServer.SpawnWithClientAuthority(lobbyPlayerObject, connectionToClient);
                
            }
        }


        #region syncVarChangeRequest

        public void AskServerToChangeName(string newName)
        {
            CmdChangeName(newName);
        }

        public void AskServerToChangeFaction(int newFactionIndex)
        {
            CmdChangeFaction(newFactionIndex);
        }

        public void AskServerToChangeTeamID(int newTeamID)
        {
            CmdChangeTeamID(newTeamID);
        }

        [Command]
        private void CmdChangeName(string newName)
        {
            playerName = newName;
        }

        [Command]
        private void CmdChangeFaction(int newFactionIndex)
        {
            factionIndex = newFactionIndex;
        }

        [Command]
        private void CmdChangeTeamID(int newTeamID)
        {
            teamID = newTeamID;
        }

        #endregion
    }
}