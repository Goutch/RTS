using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace AppLayer
{
    public class CustomLobbyPlayer : LobbyPlayer
    {
        [SerializeField] private Dropdown raceDropdown;
        [SyncVar(hook ="OnTeamChange")] public int TeamID;

        [SyncVar(hook = "OnRaceIndexChange")] public int RaceInfoIndex;

        private CustomNetworkLobbyManager customLobbyManager;

        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();
            customLobbyManager = GameObject.FindGameObjectWithTag("LobbyManager")
                .GetComponent<CustomNetworkLobbyManager>();
            //LobbyPlayerList._instance.AddPlayer(this);
            for (int i = 0; i < transform.root.GetComponent<CustomNetworkLobbyManager>().playableRaces.Count; i++)
            {
                raceDropdown.options.Add(new Dropdown.OptionData());
                raceDropdown.options[i].text =
                    transform.root.GetComponent<CustomNetworkLobbyManager>().playableRaces[i].name;
            }

            if (isLocalPlayer)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupOtherPlayer();
            }
            //setup the player data on UI. The value are SyncVar so the player
            //will be created with the right value currently on server
            OnRaceIndexChange(RaceInfoIndex);
            OnTeamChange(TeamID);
        }
        void  SetupOtherPlayer()
        {
            base.SetupOtherPlayer();
            raceDropdown.interactable = false;
        }

        void SetupLocalPlayer()
        {
          base.SetupLocalPlayer();
            raceDropdown.interactable = true;
        }


        private void OnTeamChange(int id)
        {
            if (isLocalPlayer)
            {
                CmdNameChanged(name);
            }

            else
            {
                TeamID = id;
            }
            

        }
        private void OnRaceIndexChange(int index)
        {
            raceDropdown.value = index;
            if (isLocalPlayer)
                CmdRaceIndexChanged(index);
        }

        //executed only ons server

        #region Commands

        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
        }

        [Command]
        public void CmdTeamChanged(int iD)
        {
            TeamID = iD;
        }
        [Command]
        public void CmdRaceIndexChanged(int index)
        {
            RaceInfoIndex = index;
        }

        #endregion
    }
}