using UnityEngine;
using UnityEngine.Networking;


public delegate void LobbyEventHandler(NetworkInstanceId networkInstanceId);

public class LobbyEventChannel : MonoBehaviour
{
    public event LobbyEventHandler OnLobbyPlayerSpawned;

    public void NotifyLobbyPlayerSpawned(NetworkInstanceId networkInstanceId)
    {
        OnLobbyPlayerSpawned?.Invoke(networkInstanceId);
    }
}