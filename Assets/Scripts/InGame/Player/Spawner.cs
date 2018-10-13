using DefaultNamespace;
using UnityEngine;
using UnityEngine.Networking;


public class Spawner : NetworkBehaviour
{
    [SerializeField] private GameObject unitContainerPrefab;
    private NetworkPlayerConnection _playerConnection;
    private UnitFactionData _factionData;
    private RTSNetworkManager _networkManager;
    private Transform gamePlayerObjectTransform;

    private UnitFactionData FactionData
    {
        get { return _factionData; }
        set { _factionData = Instantiate(value); }
    }

    public int factionIndex = 0;

    public int FactionIndex
    {
        get { return factionIndex; }
        set
        {
            factionIndex = value;
            FactionData = _networkManager.PlayableFactions[factionIndex];
        }
    }

    private PlayerManager manager;

    public override void OnStartClient()
    {
        _networkManager = NetworkManager.singleton.GetComponent<RTSNetworkManager>();
        _playerConnection = GetComponent<NetworkPlayerConnection>();
        ChangeFaction(GetComponent<NetworkPlayerConnection>().FactionIndex);
    }

    public void SpawnStartingUnits()
    {
        FactionData = _networkManager.PlayableFactions[factionIndex];
        for (int i = 0; i < _factionData.StartUnits.Length; i++)
        {
            CmdSpawnUnit(_factionData.SpawnableUnits.IndexOf(_factionData.StartUnits[i]),
                _factionData.StartUnitsOffSetPosition[i]);
        }
    }

    public void SpawnUnit(UnitData data, Vector2 pos)
    {
        CmdSpawnUnit(_factionData.SpawnableUnits.IndexOf(data), pos);
    }


    public void ChangeFaction(int index)
    {
        FactionIndex = index;
    }

    public void SetGamePlayer(Transform _gamePlayerTransform)
    {
        gamePlayerObjectTransform = _gamePlayerTransform;
    }


    [Command]
    public void CmdSpawnUnit(int spawnDataIndex, Vector2 pos)
    {
        GameObject unit = Instantiate(unitContainerPrefab, pos, Quaternion.identity, gamePlayerObjectTransform);
        NetworkServer.SpawnWithClientAuthority(unit, connectionToClient);
        NetworkInstanceId netID = unit.GetComponent<NetworkIdentity>().netId;
        RpcInitUnit(netID, spawnDataIndex, _playerConnection.TeamId);
    }

    [ClientRpc]
    public void RpcInitUnit(NetworkInstanceId id, int spawnDataIndex, int teamID)
    {
        UnitController unit = ClientScene.FindLocalObject(id).GetComponent<UnitController>();
        unit.Init(_factionData.SpawnableUnits[spawnDataIndex], teamID, gamePlayerObjectTransform);
        if (hasAuthority)
            StartCoroutine(unit.WaitForHauthorityRoutine());
    }
}