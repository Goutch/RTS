using System.Collections;
using System.ComponentModel;
using DefaultNamespace;
using Game;
using Player;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;


public class NetworkPlayerConnection : NetworkBehaviour
{
    [SerializeField] private GameObject lobbyPlayerPrefab;
    [SerializeField] private GameObject GamePlayerPrefab;
    

    [SyncVar(hook = nameof(OnMyName))] private string playerName = "";
    [SyncVar(hook = nameof(OnMyFaction))] private int factionIndex = 0;
    [SyncVar(hook = nameof(OnMyTeam))] private int teamID = 0;

    public string PlayerName => playerName;

    public int FactionIndex => factionIndex;

    public int TeamId => teamID;

    private RTSNetworkManager _networkManager;
    private GameLoadEventChannel _gameLoadEventChannel;
    private GameLoader _gameLoader;
    private LobbyPlayerManager lobbyPlayer;
    private PlayerManager gamePlayer;
    private Spawner spawner;

    public override void OnStartClient()
    {
        base.OnStartClient();
        DontDestroyOnLoad(this);
        _networkManager = RTSNetworkManager.singleton.GetComponent<RTSNetworkManager>();
        _networkManager.RegisterPlayer(this);
        _networkManager.OnNetworkStateChange += HandleNetworkStateChange;
        _networkManager.GetComponent<LobbyEventChannel>().OnLobbyPlayerSpawned += SetReferenceToLobbyPlayer;
        _gameLoadEventChannel = _networkManager.GetComponent<GameLoadEventChannel>();
        _gameLoader = _networkManager.GetComponent<GameLoader>();
        spawner = GetComponent<Spawner>();
        Debug.Log("Client Network Player start");
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        HandleNetworkStateChange();
    }

    public override void OnNetworkDestroy()
    {
        Debug.Log("Client Network Player OnNetworkDestroy");

        if (lobbyPlayer != null)
        {
            Destroy(lobbyPlayer.gameObject);
        }

        if (gamePlayer != null)
        {
            Destroy(gamePlayer.gameObject);
        }

        _networkManager.UnRegisterPlayer(this);
        base.OnNetworkDestroy();
    }

    private void HandleNetworkStateChange()
    {
        switch (_networkManager.CurrentNetworkState)
        {
            case RTSNetworkManager.NetworkState.Disconected:
                break;
            case RTSNetworkManager.NetworkState.PreGameLobby: //create lobbyPLayer
                OnEnterLobby();
                break;
            case RTSNetworkManager.NetworkState.LoadingGame: //ChangeScene & spawn PlayerObject set reference
                OnLoadingGame();
                break;
            case RTSNetworkManager.NetworkState.InGame: //spawned units and start playing
                OnEnterGame();
                break;
            case RTSNetworkManager.NetworkState.EndGameLobby:
                break;
        }
    }

    private void HandleLoadingGameStateChange(GameLoader.GameLoadingState newState)
    {
        switch (newState)
        {
            case GameLoader.GameLoadingState.SpawnPlayersObjects:
                OnSpawningPlayersObjects();
                break;
            case GameLoader.GameLoadingState.SettingPlayersObjectsReferences:
                break;
            case GameLoader.GameLoadingState.InitPlayersObjects:
                OnInitializingPlayers();
                break;
            case GameLoader.GameLoadingState.LoadFinish:
                OnLoadFinished();
                break;
        }
    }


    private void SetReferenceToLobbyPlayer(NetworkInstanceId lobbyPlayerNetID)
    {
        if (lobbyPlayer == null)
        {
            lobbyPlayer = ClientScene.FindLocalObject(lobbyPlayerNetID)?.GetComponent<LobbyPlayerManager>();
        }
        lobbyPlayer.OnNameChanged += AskServerToChangeName;
        lobbyPlayer.OnFactionChanged += AskServerToChangeFaction;
        lobbyPlayer.OnTeamChanged += AskServerToChangeTeamID;
    }


    #region SyncVarCallBacks

    private void OnMyName(string newName)
    {
        Debug.Log(playerName + ":Changed Faction to " + newName);
        playerName = newName;
        if (lobbyPlayer != null)
        {
            lobbyPlayer.SetPlayerName(newName);
        }

        if (gamePlayer != null)
        {
            gamePlayer.name = playerName;
        }
    }

    private void OnMyFaction(int newIndex)
    {
        Debug.Log(playerName + ":Changed Faction to " + _networkManager.PlayableFactions[newIndex].Name);
        factionIndex = newIndex;
        spawner.ChangeFaction(FactionIndex);
        
        if (lobbyPlayer != null)
        {
            lobbyPlayer.SetFactionIndex(newIndex);
        }
    }

    private void OnMyTeam(int newTeamID)
    {
        Debug.Log(playerName + ":Changed team from " + teamID + "to " + newTeamID);
        teamID = newTeamID;
        if (lobbyPlayer != null)
        {
            lobbyPlayer.SetTeamId(newTeamID);
        }
    }

    #endregion

    private void OnEnterLobby()
    {
        Debug.Log("OnEnterLobby");
        if (hasAuthority)
        {
            CmdSpawnLobbyPlayer();
        }
    }

    private void OnLoadingGame()
    {
        Debug.Log("OnLoadingGame");
    }

    public void OnEnterGameScene()
    {
        _networkManager.GetComponent<GameLoader>().OnGameLoadingStateChange +=
            HandleLoadingGameStateChange;
        Debug.Log("OnEnterGameScene");
        if (hasAuthority)
            CmdClientFinishedLoadingScene();
    }

    #region GameLoading

    private void OnSpawningPlayersObjects()
    {
        if (hasAuthority)
        {
            CmdSpawnGamePlayer();
        }
    }

    private void SetReferenceToGamePlayer(NetworkInstanceId GamePlayerNetID)
    {
        if (gamePlayer == null)
        {
            gamePlayer = ClientScene.FindLocalObject(GamePlayerNetID)?.GetComponent<PlayerManager>();
        }

        Debug.Log("Reference set for " + playerName);
        spawner.SetGamePlayer(gamePlayer.transform);
    }

    private void OnInitializingPlayers()
    {
        
        gamePlayer.Init(this.netId);
    }

    private void OnLoadFinished()
    {
        if (hasAuthority)
            CmdClientFinishedLoadingGame();
    }

    #endregion


    private void OnEnterGame()
    {
        Debug.Log("OnEnterGame");
        if (hasAuthority)
        {
            spawner.SpawnStartingUnits();
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


    [Command]
    private void CmdSpawnLobbyPlayer()
    {
        GameObject lobbyPlayerObject = Instantiate(lobbyPlayerPrefab);
        lobbyPlayer = lobbyPlayerObject.GetComponent<LobbyPlayerManager>();
        NetworkServer.SpawnWithClientAuthority(lobbyPlayerObject, connectionToClient);
    }

    [Command]
    private void CmdClientFinishedLoadingScene()
    {
        OnAllClientsFinishedLoadingScene();
    }

    [Server]
    private void OnAllClientsFinishedLoadingScene()
    {
        _gameLoadEventChannel.OnALlPlayerSceneLoaded += CmdClientsLoadedScene;
        _gameLoadEventChannel.NotifyPlayerSceneLoaded();
    }

    [Command]
    private void CmdClientsLoadedScene()
    {
        RpcClientsLoadedScene();
    }

    [ClientRpc]
    private void RpcClientsLoadedScene()
    {
        Debug.Log("OnClientsScenesLoadedRPC");
        _gameLoader.OnClientLoadedScene();
    }

    [Command]
    private void CmdSpawnGamePlayer()
    {
        if (!connectionToClient.isReady)
            NetworkServer.SetClientReady(connectionToClient);
        if (connectionToClient.isReady)
        {
            GameObject gamePlayerObject = Instantiate(GamePlayerPrefab, Vector3.one, Quaternion.identity);
            gamePlayer = gamePlayerObject.GetComponent<PlayerManager>();
            NetworkServer.SpawnWithClientAuthority(gamePlayerObject, connectionToClient);
            RpcSetReferenceGamePlayerObject(gamePlayer.GetComponent<NetworkIdentity>().netId);
            CmdSetGamePlayerReference();
        }
    }

    [Command]
    private void CmdSetGamePlayerReference()
    {
        RpcSetReferenceGamePlayerObject(gamePlayer.GetComponent<NetworkIdentity>().netId);
    }

    [ClientRpc]
    private void RpcSetReferenceGamePlayerObject(NetworkInstanceId instanceId)
    {
        SetReferenceToGamePlayer(instanceId);
        _gameLoadEventChannel.OnPlayerReferenceSet += CmdAllClientsPlayerReferenceSet;
        CmdOnReferenceSetForClient();
    }

    [Command]
    private void CmdOnReferenceSetForClient()
    {
        _gameLoadEventChannel.NotifyPlayerReferenceSet();
    }

    [Command]
    private void CmdAllClientsPlayerReferenceSet()
    {
        RpcAllClientsPlayersReferenceSet();
    }

    [ClientRpc]
    private void RpcAllClientsPlayersReferenceSet()
    {
        _gameLoader.OnPlayerReferenceSet();
    }

    [Command]
    private void CmdClientFinishedLoadingGame()
    {
        _gameLoadEventChannel.OnAllPlayerGameLoaded += CmdAllClientsGameLoadIsLoaded;
        _gameLoadEventChannel.NotifyPlayerGameLoaded();
    }

    [Command]
    private void CmdAllClientsGameLoadIsLoaded()
    {
        RpcAlllClientsGameIsLoaded();
    }

    [ClientRpc]
    private void RpcAlllClientsGameIsLoaded()
    {
        Debug.Log(PlayerName + " finished loading Game");
        _gameLoader.OnLoadFinish();
    }
}