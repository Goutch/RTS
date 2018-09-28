﻿
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

    [SyncVar(hook = "OnFactionIndexChange")]
    public int factionIndex = 0;

    private PlayerManager manager;

    private void Awake()
    {
        _networkManager = NetworkManager.singleton.GetComponent<RTSNetworkManager>();
    }

    public void SpawnStartingUnits()
    {
        _playerConnection = GetComponent<NetworkPlayerConnection>();
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

    //SynchVar Callback
    private void OnFactionIndexChange(int index)
    {
        factionIndex = index;
        FactionData = _networkManager.PlayableFactions[factionIndex];
    }

    public void ChangeFaction(int index)
    {
        factionIndex = index;
    }

    public void SetGamePlayer(Transform _gamePlayerTransform)
    {
        gamePlayerObjectTransform = _gamePlayerTransform;
    }

    [Command]
    public void CmdSpawnUnit(int spawnInfoIndex, Vector2 pos)
    {
        GameObject unit = Instantiate(unitContainerPrefab, pos, Quaternion.identity, gamePlayerObjectTransform);
        NetworkServer.SpawnWithClientAuthority(unit, connectionToClient);
        unit.GetComponent<UnitController>()
            .Init(_factionData.SpawnableUnits[spawnInfoIndex], _playerConnection.TeamId, gamePlayerObjectTransform);
        NetworkInstanceId netID = unit.GetComponent<NetworkIdentity>().netId;
        RpcInitUnit(netID, spawnInfoIndex, _playerConnection.TeamId);
    }

    [ClientRpc]
    public void RpcInitUnit(NetworkInstanceId id, int spawnInfoIndex, int teamID)
    {
        if (!isServer)
        {
            GameObject go = ClientScene.FindLocalObject(id);
            go.GetComponent<UnitController>()
                .Init(_factionData.SpawnableUnits[spawnInfoIndex], teamID, gamePlayerObjectTransform);
        }
    }
}