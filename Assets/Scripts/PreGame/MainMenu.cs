using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private InputField IPAdresseInputField;
    public static MainMenu instance = null;

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            Destroy(gameObject);
    }


    private void Start()
    {
        IPAdresseInputField.text = "127.0.0.1";
    }

    public void OnClickFindMatch()
    {
       // NetworkManager.singleton.matchMaker.JoinMatch(NetworkManager.singleton.matches[0].networkId,"","","",0,1,waitFormConnectionRoutine());
        //https://github.com/Brackeys/MultiplayerFPS-Tutorial/blob/master/MultiplayerFPS/Assets/Scripts/JoinGame.cs
    }

    public void OnClickCreateMatch()
    {
        
    }
    public void OnClickHost()
    {
        ChangeToLobbyPanel();
        RTSNetworkManager.singleton.StartHost();
    }

    public void OnClickJoin()
    {
        RTSNetworkManager.singleton.networkAddress = IPAdresseInputField.text;
        Debug.Log("Joining:" + RTSNetworkManager.singleton.networkAddress + " port:" +
                  RTSNetworkManager.singleton.networkPort);
        RTSNetworkManager.singleton.StartClient();
        //todo:display connecting
    }

    public void ChangeToLobbyPanel()
    {
        mainMenuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }
    IEnumerator waitFormConnectionRoutine()
    {
        yield return null;
    }
}