using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Networking;

public delegate void RTSNetworkManagerEventHandler();

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private UnitFactionData[] playableFactions;
    [SerializeField] private string playSceneName;
    public UnitFactionData[] PlayableFactions => playableFactions;
    private List<NetworkPlayerConnection> connectedPlayers;

    public List<NetworkPlayerConnection> ConnectedPlayers => connectedPlayers;

    private int ReadyForNextStatePlayersCount = 0;

    private NetworkState _currentNetworkState;

    public NetworkState CurrentNetworkState
    {
        private set
        {
            _currentNetworkState = value;
            Debug.Log("NetworkState changed to:" + value);
            if (OnNetworkStateChange != null)
                OnNetworkStateChange();
        }
        get { return _currentNetworkState; }
    }


    public event RTSNetworkManagerEventHandler OnNetworkStateChange;


    public enum NetworkState
    {
        Disconected,
        PreGameLobby,
        LoadingGame,
        InGame,
        EndGameLobby
    }


    private void Start()
    {
        CurrentNetworkState = NetworkState.Disconected;
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        CurrentNetworkState = NetworkState.Disconected;
    }

    public void RegisterPlayer(NetworkPlayerConnection newNetworkPlayerConnection)
    {
        if (connectedPlayers == null)
            connectedPlayers = new List<NetworkPlayerConnection>();
        connectedPlayers.Add(newNetworkPlayerConnection);
    }

    public void UnRegisterPlayer(NetworkPlayerConnection networkPlayerConnectionToRemove)
    {
        if (connectedPlayers == null)
            connectedPlayers = new List<NetworkPlayerConnection>();
        connectedPlayers.Remove(networkPlayerConnectionToRemove);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        MainMenu.instance.ChangeToLobbyPanel();
        CurrentNetworkState = NetworkState.PreGameLobby;
        GetComponent<LobbyEventChannel>().OnLobbyPlayerSpawned += OnLobbyPlayerSpawn;
        base.OnClientConnect(conn);
    }

    private void OnLobbyPlayerSpawn(NetworkInstanceId lobbyPlayerID)
    {
        LobbyPlayerManager lobbyPlayer =
            ClientScene.FindLocalObject(lobbyPlayerID).GetComponent<LobbyPlayerManager>();
        lobbyPlayer.OnReadyChanged += AddReadyLobbyPlayer;
    }

    private void AddReadyLobbyPlayer()
    {
        ReadyForNextStatePlayersCount++;
        if (ReadyForNextStatePlayersCount == connectedPlayers.Count)
        {
            ReadyForNextStatePlayersCount = 0;
            StartCoroutine(StartGameCountDownRoutine(3));
        }
    }

    public IEnumerator StartGameCountDownRoutine(int seconds)
    {
        for (int i = seconds; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
        }

        ServerChangeScene(playSceneName);
        CurrentNetworkState = NetworkState.LoadingGame;
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        this.GetComponent<GameLoader>().OnSceneChanged();
        if (_currentNetworkState == NetworkState.LoadingGame)
        {
            foreach (var player in connectedPlayers)
            {
                player.OnEnterGameScene();
            }
        }
    }

    public void OnLoadingFinish()
    {
        CurrentNetworkState = NetworkState.InGame;
    }
}