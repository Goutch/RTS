
using Game;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour
{
    private int teamID = 0;
    private string playerName = "";
    private NetworkPlayerConnection myNetworkPlayer;
    private RTSNetworkManager _networkManager;

    
    public int TeamId
    {
        get { return teamID; }
        set { teamID = value; }
    }

    public override void OnStartClient()
    {
        
        base.OnStartClient();
        _networkManager = NetworkManager.singleton.GetComponent<RTSNetworkManager>();
        _networkManager.GetComponent<GameEventChannel>().NotifyPlayersObjectSpawned();
        Debug.Log("Start GamePlayerObject");
    }

    public void Init(NetworkInstanceId myNetworkPlayerNetID)
    {
        
        myNetworkPlayer = ClientScene.FindLocalObject(myNetworkPlayerNetID).GetComponent<NetworkPlayerConnection>();
        playerName = myNetworkPlayer.PlayerName;
        TeamId = myNetworkPlayer.TeamId;
        _networkManager.GetComponent<GameEventChannel>().NotifyPlayersObjectInitialized();
        Debug.Log("Init GamePlayerObject");
    }
}