using System;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace AppLayer
{
    public class CustomLobbyPlayer : LobbyPlayer
    {
        [SerializeField] private Dropdown factionsDropdown;
        public int TeamID
        {
            get { return Array.IndexOf(LobbyPlayer.Colors, playerColor); }
        }

        [SyncVar(hook = "OnFactionIndexChange")]
        public int factionInfoIndex;

        private CustomNetworkLobbyManager customLobbyManager;

        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();
            customLobbyManager = GameObject.FindGameObjectWithTag("LobbyManager")
                .GetComponent<CustomNetworkLobbyManager>();
            //LobbyPlayerList._instance.AddPlayer(this);
            for (int i = 0; i < customLobbyManager.playableFactions.Count; i++)
            {
                factionsDropdown.options.Add(new Dropdown.OptionData());
                factionsDropdown.options[i].text =
                    customLobbyManager.playableFactions[i].name;
            }
        }

        private void Start()
        {
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
            OnFactionIndexChange(factionInfoIndex);
            OnMyName(playerName);
            OnMyColor(playerColor);
        }


        void SetupOtherPlayer()
        {
            base.SetupOtherPlayer();
            factionsDropdown.interactable = false;
        }

        void SetupLocalPlayer()
        {
            base.SetupLocalPlayer();
            factionsDropdown.interactable = true;
        }


        //callback from synchvar
        private void OnFactionIndexChange(int index)
        {
            factionInfoIndex = index;
            factionsDropdown.value = index;
        }

        //when drop down is interracted with by the client
        public void OnFactionDropDownValueChanged()
        {
            //ask server if modif is okay
            CmdFactionIndexChanged(factionsDropdown.value);
        }
        //executed only ons server

        #region Commands

        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
        }


        [Command]
        public void CmdFactionIndexChanged(int index)
        {
            factionInfoIndex = index;
        }

        #endregion
    }
}