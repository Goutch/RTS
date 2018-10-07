using Game;
using UnityEngine;
using UnityEngine.Networking;

public delegate void GameLoaderEventHandler(GameLoader.GameLoadingState state);


public class GameLoader : MonoBehaviour
{
    public event GameLoaderEventHandler OnGameLoadingStateChange;
    private static int numberClientLoaded = 0;


    private GameEventChannel _gameEventChannel;
    private RTSNetworkManager _networkManager;

    private void Awake()
    {
        this.GetComponent<GameEventChannel>();
    }

    public enum GameLoadingState
    {
        NotLoading,
        SpawnPlayersObjects,
        SettingPlayersObjectsReferences,
        InitPlayersObjects,
        LoadFinish,
    }

    private GameLoadingState currentGameLoadingState = GameLoadingState.NotLoading;

    public GameLoadingState CurrentGameLoadingState
    {
        private set
        {
            currentGameLoadingState = value;
            Debug.Log("GameLoadingState changed to:" + value);
            if (OnGameLoadingStateChange != null)
                OnGameLoadingStateChange(value);
        }
        get { return currentGameLoadingState; }
    }

    private int spawnedPlayersCount = 0;
    private int playersRefereneSetCount = 0;
    private int initializedPlayersCount = 0;

    public void OnSceneChanged()
    {
        _gameEventChannel = NetworkManager.singleton.GetComponent<GameEventChannel>();
        _networkManager = NetworkManager.singleton.GetComponent<RTSNetworkManager>();
    }

    public void OnClientLoadedScene()
    {
        numberClientLoaded++;
        if (numberClientLoaded == _networkManager.ConnectedPlayers.Count)
        {
            _gameEventChannel.OnPlayersObjectSpawned += OnPlayersSpawned;
            CurrentGameLoadingState = GameLoadingState.SpawnPlayersObjects;
            numberClientLoaded = 0;
        }
    }

    public void OnPlayersSpawned()
    {
        spawnedPlayersCount++;
        if (spawnedPlayersCount == _networkManager.ConnectedPlayers.Count)
        {
            _gameEventChannel.OnPlayersObjectSpawned -= OnPlayersSpawned;
            CurrentGameLoadingState = GameLoadingState.SettingPlayersObjectsReferences;
        }
    }

    public void OnPlayerReferenceSet()
    {
        playersRefereneSetCount++;
        if (playersRefereneSetCount == _networkManager.ConnectedPlayers.Count)
        {
            _gameEventChannel.OnPlayerReferenceSet -= OnPlayerReferenceSet;
            CurrentGameLoadingState = GameLoadingState.InitPlayersObjects;
        }
    }


   public void OnPlayerInitialized()
    {
        initializedPlayersCount++;
        if (initializedPlayersCount == _networkManager.ConnectedPlayers.Count)
        {
            CurrentGameLoadingState = GameLoadingState.LoadFinish;
        }
    }

    public void OnLoadFinish()
    {
        numberClientLoaded++;
        if (numberClientLoaded == _networkManager.ConnectedPlayers.Count)
        {
            GetComponent<RTSNetworkManager>().OnLoadingFinish();
        }
    }
    
}