using System;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace AppLayer
{
    public delegate void LobbyPlayerEventHandler();

    public class LobbyPlayerManager : NetworkBehaviour
    {

        private RTSNetworkManager _networkManager;
        [SerializeField]private InputField nameInputField;
        [SerializeField] private Dropdown factionDropDown;
        [SerializeField] private InputField teamIdInputField;

        public event LobbyPlayerEventHandler OnNameChanged;
        public event LobbyPlayerEventHandler OnFactionChanged;
        public event LobbyPlayerEventHandler OnTeamChanged;

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            _networkManager = NetworkManager.singleton.GetComponent<RTSNetworkManager>();
            for (int i = 0; i < _networkManager.PlayableFactions.Length; i++)
            {
                factionDropDown.options.Add(new Dropdown.OptionData(_networkManager.PlayableFactions[i].Name));
            }

            factionDropDown.value = 0;
            LobbyPlayerObjectList.instance.Add(this.gameObject);
            if (hasAuthority)
                SetUpLocalPlayer();
            else
                SetUpOtherPlayer();
        }

        private void SetUpLocalPlayer()
        {
            nameInputField.interactable = true;
            factionDropDown.interactable = true;
            teamIdInputField.interactable = true;
            nameInputField.text = "Player "+LobbyPlayerObjectList.instance.PlayersList.Count;
            NotifyNameChanged();
            teamIdInputField.text = LobbyPlayerObjectList.instance.PlayersList.Count.ToString();
            NotifyTeamChanged();
        }

        private void SetUpOtherPlayer()
        {
            nameInputField.interactable = false;
            factionDropDown.interactable = false;
            teamIdInputField.interactable = false;
        }

        public override void OnNetworkDestroy()
        {
            LobbyPlayerObjectList.instance.Remove(this.gameObject);
        }

        public void SetPlayerName(String name)
        {
            nameInputField.text = name;
        }

        public void SetFactionIndex(int index)
        {
            factionDropDown.value = index;
        }

        public void SetTeamId(int ID)
        {
            teamIdInputField.text = ID.ToString();
        }

        public void NotifyNameChanged()
        {
            if (OnNameChanged != null)
            {
                OnNameChanged();
            }
        }

        public void NotifyFactionChanged()
        {
            if (OnFactionChanged != null)
            {
                OnFactionChanged();
            }
        }

        public void NotifyTeamChanged()
        {
            if (OnTeamChanged != null)
            {
                OnTeamChanged();
            }
        }
    }
}