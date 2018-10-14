using Game;
using UnityEngine;
using UnityEngine.Networking;

namespace Player
{
    public class PlayerManager : NetworkBehaviour
    {
        private int teamID = 0;
    
        public int TeamId1 => teamID;
    
        private string playerName = "";
        private NetworkPlayerConnection myNetworkPlayer;
        private RTSNetworkManager _networkManager;
        private GameLoadEventChannel _gameLoadEventChannel;

        public NetworkPlayerConnection MyNetworkPlayer => myNetworkPlayer;

        public int TeamId
        {
            get { return teamID; }
            set { teamID = value; }
        }
    
        public override void OnStartClient()
        {
            base.OnStartClient();
            _networkManager = NetworkManager.singleton.GetComponent<RTSNetworkManager>();
            CmdGamePlayerObjectSpawnedOnClient();
            Debug.Log("Start GamePlayerObject");
        }
    
        public void Init(NetworkInstanceId myNetworkPlayerNetID)
        {
            Debug.Log(playerName+ "Init his gamePlayer");
            myNetworkPlayer = ClientScene.FindLocalObject(myNetworkPlayerNetID).GetComponent<NetworkPlayerConnection>();
            playerName = myNetworkPlayer.PlayerName;
            TeamId = myNetworkPlayer.TeamId;
            if(hasAuthority)
            CmdOnClientGamePlayerObjectInitialized();
        }
    
        [Command]
        private void CmdGamePlayerObjectSpawnedOnClient()
        {
            if (_gameLoadEventChannel == null)
                _gameLoadEventChannel = _networkManager.GetComponent<GameLoadEventChannel>();
            _gameLoadEventChannel.OnPlayersObjectSpawned += CmdOnAllClientsGameLoadPlayerObjectSpawned;
            _gameLoadEventChannel.NotifyPlayersObjectSpawned();
        }
    
        [Command]
        private void CmdOnAllClientsGameLoadPlayerObjectSpawned()
        {
            RpcOnAllClientsGamePlayerObjectSpawned();
        }
    
        [ClientRpc]
        private void RpcOnAllClientsGamePlayerObjectSpawned()
        {
            _networkManager.GetComponent<GameLoader>().OnPlayersSpawned();
        }
    
        [Command]
        private void CmdOnClientGamePlayerObjectInitialized()
        {
            _gameLoadEventChannel.OnPlayerInitialized += CmdOnAllGameLoadPlayerObjectsInitialized;
            _networkManager.GetComponent<GameLoadEventChannel>().NotifyPlayersObjectInitialized();
        }
    
        [Command]
        private void CmdOnAllGameLoadPlayerObjectsInitialized()
        {
            _gameLoadEventChannel.OnPlayerInitialized -= CmdOnAllGameLoadPlayerObjectsInitialized;
            RpcOnAllGamePlayersObjectsInitialized();
        }
    
        [ClientRpc]
        private void RpcOnAllGamePlayersObjectsInitialized()
        {
            _networkManager.GetComponent<GameLoader>().OnPlayerInitialized();
        }


}
}