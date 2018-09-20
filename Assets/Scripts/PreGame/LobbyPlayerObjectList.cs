using System.ComponentModel;
using Boo.Lang;
using UnityEngine;

namespace AppLayer
{
    public class LobbyPlayerObjectList:MonoBehaviour
    {
        public static LobbyPlayerObjectList instance=null;

        private List<GameObject> playersList;

        public List<GameObject> PlayersList => playersList;

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
}