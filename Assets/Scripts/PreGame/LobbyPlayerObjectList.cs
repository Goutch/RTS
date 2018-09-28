
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



    public class LobbyPlayerObjectList : MonoBehaviour
    {
        public static LobbyPlayerObjectList instance = null;

        private List<GameObject> playersList;

        public List<GameObject> PlayersList => playersList;

        private RTSNetworkManager _networkManager;
        private void Awake()
        {
            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)
                Destroy(gameObject);
            playersList=new List<GameObject>();
            _networkManager = NetworkManager.singleton.GetComponent<RTSNetworkManager>();
        }

       

        public void Add(GameObject LobbyPlayerObject)
        {
            playersList.Add(LobbyPlayerObject);
            LobbyPlayerObject.transform.parent = this.transform;

        }

        public void Remove(GameObject lobbyPlayerObject)
        {
            playersList.Remove(lobbyPlayerObject);

        }
    }
