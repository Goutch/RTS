using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


    public delegate void LobbyPlayerNameEventHandler(string name);

    public delegate void LobbyPlayerFactionEventHandler(int index);

    public delegate void LobbyPlayerTeamEventHandler(int teamID);

    public delegate void LobbyPlayerReadyStateEventHandler();

    public class LobbyPlayerManager : NetworkBehaviour
    {
        private RTSNetworkManager _networkManager;
        [SerializeField] private InputField nameInputField;
        [SerializeField] private Dropdown factionDropDown;
        [SerializeField] private InputField teamIdInputField;
        [SerializeField] private Button lockNReadyButton;
        [SerializeField] private Text readyText;
        [SyncVar(hook = nameof(OnReady))] private bool isReady = false;
        public event LobbyPlayerNameEventHandler OnNameChanged;
        public event LobbyPlayerFactionEventHandler OnFactionChanged;
        public event LobbyPlayerTeamEventHandler OnTeamChanged;
        public event LobbyPlayerReadyStateEventHandler OnReadyChanged;
        public override void OnStartClient()
        {
            base.OnStartClient();
            _networkManager = NetworkManager.singleton.GetComponent<RTSNetworkManager>();
            for (int i = 0; i < _networkManager.PlayableFactions.Length; i++)
            {
                factionDropDown.options.Add(new Dropdown.OptionData(_networkManager.PlayableFactions[i].Name));
            }
            factionDropDown.value = 0;
            SetUpOtherPlayer();
            LobbyPlayerObjectList.instance.Add(this.gameObject);
            nameInputField.text = "Player " + LobbyPlayerObjectList.instance.PlayersList.Count;
            teamIdInputField.text = LobbyPlayerObjectList.instance.PlayersList.Count.ToString();
            _networkManager.GetComponent<LobbyEventChannel>().NotifyLobbyPlayerSpawned(this.netId);
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            SetUpLocalPlayer();
            NotifyNameChanged();
            NotifyTeamChanged();
            NotifyFactionChanged();
        }

        private void SetUpLocalPlayer()
        {
            nameInputField.interactable = true;
            factionDropDown.interactable = true;
            teamIdInputField.interactable = true;
            lockNReadyButton.gameObject.SetActive(true);
            teamIdInputField.text = LobbyPlayerObjectList.instance.PlayersList.Count.ToString();
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

        private void OnReady(bool ready)
        {
            readyText.gameObject.SetActive(ready);
            isReady = ready;
            if (OnReadyChanged != null) OnReadyChanged();
        }

        public void OnReadyButtonClick()
        {
            nameInputField.interactable = false;
            factionDropDown.interactable = false;
            teamIdInputField.interactable = false;
            lockNReadyButton.interactable = false;
            CmdSetClientReady();
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
                OnNameChanged(nameInputField.text);
            }
        }

        public void NotifyFactionChanged()
        {
            if (OnFactionChanged != null)
            {
                OnFactionChanged(factionDropDown.value);
            }
        }

        public void NotifyTeamChanged()
        {
            if (OnTeamChanged != null)
            {
                int teamID;
                Int32.TryParse(teamIdInputField.text, out teamID);
                OnTeamChanged(teamID);
            }
        }

        [Command]
        private void CmdSetClientReady()
        {
            isReady = true;
        }

    }
