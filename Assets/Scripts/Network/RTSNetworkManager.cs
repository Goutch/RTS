using Boo.Lang;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace AppLayer
{
    public delegate void RTSNetworkManagerEventHandler();
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField]private UnitFactionInfo[] playableFactions;

        public UnitFactionInfo[] PlayableFactions => playableFactions;
        private List<NetworkPlayer> connectedPlayers;
        
        private NetworkState _currentNetworkState;
        public NetworkState CurrentNetworkState
        {
            private set
            {
                _currentNetworkState = value;
                Debug.Log("NetworkState changed to:"+value);
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
         
        public void RegisterPlayer(NetworkPlayer newNetworkPlayer)
        {
            if(connectedPlayers==null)
                connectedPlayers=new List<NetworkPlayer>();
            connectedPlayers.Add(newNetworkPlayer);
        }

        public void UnRegisterPlayer(NetworkPlayer networkPlayerToRemove)
        {
            if(connectedPlayers==null)
                connectedPlayers=new List<NetworkPlayer>();
            connectedPlayers.Remove(networkPlayerToRemove);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            MainMenu.instance.ChangeToLobbyPanel();
            CurrentNetworkState = NetworkState.PreGameLobby;
            
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            CurrentNetworkState = NetworkState.Disconected;
        }
    }
}