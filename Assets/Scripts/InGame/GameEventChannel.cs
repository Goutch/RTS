using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public delegate void GameLoadEventHandler();
    public class GameEventChannel : MonoBehaviour
    {
        //server
        private int numberPlayerSceneLoaded = 0;
        public event GameLoadEventHandler OnALlPlayerSceneLoaded;
        
        //client
        private int numberPlayerObjectSpawned = 0;
        public event GameLoadEventHandler OnPlayersObjectSpawned;
        private int numberPlayerObjectReferenceSet = 0;
        public event GameLoadEventHandler OnPlayerReferenceSet;
        private int numberPlayerObjectInitialized = 0;
        public event GameLoadEventHandler OnPlayerInitialized;
        
        //server
        private int numberPlayerGameLoaded = 0;
        public event GameLoadEventHandler OnAllPlayerGameLoaded;


        private RTSNetworkManager _networkManager;
        private void Start()
        {
            _networkManager=NetworkManager.singleton.GetComponent<RTSNetworkManager>();
        }

        public void NotifyPlayerSceneLoaded()
        {
            numberPlayerSceneLoaded++;
            if (numberPlayerSceneLoaded == _networkManager.ConnectedPlayers.Count)
            {
                OnALlPlayerSceneLoaded?.Invoke();
            }
        }

        public void NotifyPlayersObjectSpawned()
        {
            numberPlayerObjectSpawned++;
            if (numberPlayerObjectSpawned == _networkManager.ConnectedPlayers.Count*_networkManager.ConnectedPlayers.Count)
            {
                OnPlayersObjectSpawned?.Invoke();
            }
        }

        public void NotifyPlayerReferenceSet()
        {
            numberPlayerObjectReferenceSet++;
            if (numberPlayerObjectReferenceSet == _networkManager.ConnectedPlayers.Count*_networkManager.ConnectedPlayers.Count)
            {
                OnPlayerReferenceSet?.Invoke();
            }
        }

        public void NotifyPlayersObjectInitialized()
        {
            numberPlayerObjectInitialized++;
            if (numberPlayerObjectInitialized == _networkManager.ConnectedPlayers.Count)
                OnPlayerInitialized?.Invoke();
        }

        public void NotifyPlayerGameLoaded()
        {
            numberPlayerGameLoaded++;
            if (numberPlayerGameLoaded == _networkManager.ConnectedPlayers.Count)
            {
                OnAllPlayerGameLoaded?.Invoke();
            }
        }
    }
}