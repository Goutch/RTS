using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{

	[SerializeField] private GameObject createGamePanel;
	[SerializeField] private GameObject joinGamePanel;
	[SerializeField] private GameObject mainMenuPanel;
	private GameObject currentPannel;
	
	public void OnCreateGameButtonClick()
	{
		mainMenuPanel.SetActive(false);
		createGamePanel.SetActive(true);
		currentPannel = createGamePanel;
	}

	public void OnJoinGameButtonClick()
	{
		mainMenuPanel.SetActive(false);
		joinGamePanel.SetActive(true);
		currentPannel = joinGamePanel;
	}

	public void OnBackButtonClick()
	{
		currentPannel.SetActive(false);
		mainMenuPanel.SetActive(true);
	}
}
