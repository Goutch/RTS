
using System.Configuration;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour
{
    [SyncVar] private int teamID = 0;
    
    // Use this for initialization
    private void Start()
    {
        if(isServer)
        GetComponent<Spawner>().SpawnStartingUnits();
    }

    public int TeamId
    {
        get { return teamID; }
        set { teamID = value; }
    }



}