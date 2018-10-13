using Game;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour
{
    private int teamID = 0;

    public int TeamId1 => teamID;

    private string playerName = "";
    private NetworkPlayerConnection myNetworkPlayer;
    private RTSNetworkManager _networkManager;
    private GameEventChannel _gameEventChannel;


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
        if (_gameEventChannel == null)
            _gameEventChannel = _networkManager.GetComponent<GameEventChannel>();
        _gameEventChannel.OnPlayersObjectSpawned += CmdOnAllClientsGamePlayerObjectSpawned;
        _gameEventChannel.NotifyPlayersObjectSpawned();
    }

    [Command]
    private void CmdOnAllClientsGamePlayerObjectSpawned()
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
        _gameEventChannel.OnPlayerInitialized += CmdOnAllGamePlayerObjectsInitialized;
        _networkManager.GetComponent<GameEventChannel>().NotifyPlayersObjectInitialized();
    }

    [Command]
    private void CmdOnAllGamePlayerObjectsInitialized()
    {
        _gameEventChannel.OnPlayerInitialized -= CmdOnAllGamePlayerObjectsInitialized;
        RpcOnAllGamePlayersObjectsInitialized();
    }

    [ClientRpc]
    private void RpcOnAllGamePlayersObjectsInitialized()
    {
        _networkManager.GetComponent<GameLoader>().OnPlayerInitialized();
    }
}