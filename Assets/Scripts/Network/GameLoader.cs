using Game;
using UnityEngine;
using UnityEngine.Networking;

public delegate void GameLoaderEventHandler(GameLoader.GameLoadingState state);

public class GameLoader : NetworkBehaviour
{
    public event GameLoaderEventHandler OnGameLoadingStateChange;
    private int numberClientLoaded = 0;
    private GameEventChannel _gameEventChannel;

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
        _gameEventChannel.OnPlayersObjectSpawned += OnPlayersSpawned;
        CurrentGameLoadingState = GameLoadingState.SpawnPlayersObjects;
    }

    private void OnPlayersSpawned()
    {
        spawnedPlayersCount++;
        if (spawnedPlayersCount == NetworkServer.connections.Count)
        {
            _gameEventChannel.OnPlayersObjectSpawned -= OnPlayersSpawned;
            _gameEventChannel.OnPlayerReferenceSet += OnPlayerReferenceSet;
            CurrentGameLoadingState = GameLoadingState.SettingPlayersObjectsReferences;
        }
    }

    private void OnPlayerReferenceSet()
    {
        playersRefereneSetCount++;
        if (playersRefereneSetCount == NetworkServer.connections.Count)
        {
            _gameEventChannel.OnPlayerReferenceSet -= OnPlayerReferenceSet;
            _gameEventChannel.OnPlayerInitialized += OnPlayerInitialized;
            CurrentGameLoadingState = GameLoadingState.InitPlayersObjects;
        }
    }


    private void OnPlayerInitialized()
    {
        initializedPlayersCount++;
        if (initializedPlayersCount == NetworkServer.connections.Count)
        {
            _gameEventChannel.OnPlayerInitialized -= OnPlayerInitialized;
            CmdClientFinishedLoadingGame();
        }
    }

    [Command]
    private void CmdClientFinishedLoadingGame()
    {
        numberClientLoaded++;
        if (numberClientLoaded == NetworkServer.connections.Count)
        {
            RpcStartGame();
        }
    }

    [ClientRpc]
    private void RpcStartGame()
    {
        currentGameLoadingState = GameLoadingState.NotLoading;

        NetworkManager.singleton.GetComponent<RTSNetworkManager>().OnLoadingFinish();
    }
}