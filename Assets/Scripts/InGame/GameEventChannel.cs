using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public delegate void GameLoadEventHandler();

    public delegate void GameInitEventHandler();

    public class GameEventChannel : MonoBehaviour
    {
        private int numberPlayerObjectSpawned = 0;
        public event GameLoadEventHandler OnPlayersObjectSpawned;
        private int numberPlayerObjectReferenceSet = 0;
        public event GameLoadEventHandler OnPlayerReferenceSet;
        private int numberPlayerObjectInitialized = 0;
        public event GameInitEventHandler OnPlayerInitialized;


        public void NotifyPlayersObjectSpawned()
        {
            numberPlayerObjectSpawned++;
            if (numberPlayerObjectSpawned == NetworkServer.connections.Count)
            {
                OnPlayersObjectSpawned?.Invoke();
            }
        }

        public void NotifyPlayerReferenceSet()
        {
            numberPlayerObjectReferenceSet++;
            if (numberPlayerObjectReferenceSet == NetworkServer.connections.Count)
            {
                OnPlayerReferenceSet?.Invoke();
            }
        }

        public void NotifyPlayersObjectInitialized()
        {
            numberPlayerObjectInitialized++;
            if (numberPlayerObjectInitialized == NetworkServer.connections.Count)
                OnPlayerInitialized?.Invoke();
        }
    }
}