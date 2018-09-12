using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public delegate void CustomNetworkEventHandler();
public class CustomNetworkManager : NetworkManager
{
	public event CustomNetworkEventHandler OnAllPlayerLoaded;
	// Use this for initialization


	// Update is called once per frame
	void Update () {
		
	}
	public void NotifyPlayersReady()
	{
		if (OnAllPlayerLoaded!= null) OnAllPlayerLoaded();
	}


}
