using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour
{
    [SyncVar(hook="OnTeamIdChange")] private int teamID = 0;
    [SyncVar(hook = "OnNameChange")] private string playerName = "";
    public int TeamId
    {
        get { return teamID; }
        set { teamID = value; }
    }

    private void OnNameChange(string name)
    {
        playerName = name;
        gameObject.name = playerName;
    }
    
    private void OnTeamIdChange(int id)
    {
        teamID=id;
    }
    public void ChangeTeamId(int id)
    {
        teamID = id;
    }

    public void ChangePlayerName(string newName)
    {
        playerName = newName;
    }
   
    
}