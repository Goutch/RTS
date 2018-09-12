using UnityEngine;
using UnityEngine.UI;

namespace AppLayer
{
    public class CreateGameMenu : MonoBehaviour
    {
        [SerializeField] private Dropdown MapDropDown;

        [SerializeField] private Button HostButton;

        [SerializeField] private Transform lobbyPanel;

        public void Start()
        {
            MapDropDown.interactable = true;
            HostButton.interactable = true;
        }


        public void OnHostButtonClick()
        {
            lobbyPanel.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
            transform.root.GetComponent<CustomNetworkLobbyManager>().StartHost();
        }
    }
}