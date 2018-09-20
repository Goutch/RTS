using AppLayer;
using UnityEngine;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public class Spawner : NetworkBehaviour
    {
        [SerializeField] private UnitFactionInfo _factionInfo;
        [SerializeField] private GameObject unitContainerPrefab;
        [SerializeField] private CustomNetworkLobbyManager networkController;
        private int unitId;

        private UnitFactionInfo FactionInfo
        {
            get { return _factionInfo; }
            set { _factionInfo = Instantiate(value); }
        }

        [SyncVar(hook = "OnFactionIndexChange")] public int factionIndex=0;

        private PlayerManager manager;

        private void Awake()
        {
            networkController = GameObject.FindGameObjectWithTag("LobbyManager")
                .GetComponent<CustomNetworkLobbyManager>();
            networkController.OnAllPlayerLoaded += SpawnStartingUnits;
        }

        public void SpawnStartingUnits()
        {
            manager = GetComponent<PlayerManager>();
            FactionInfo = networkController.playableFactions[factionIndex];
            for (int i = 0; i < _factionInfo.StartUnits.Length; i++)
            {
                CmdSpawnUnit(_factionInfo.SpawnableUnits.IndexOf(_factionInfo.StartUnits[i]),
                    _factionInfo.StartUnitsOffSetPosition[i]);
            }
        }

        public void SpawnUnit(UnitInfo info, Vector2 pos)
        {
            CmdSpawnUnit(_factionInfo.SpawnableUnits.IndexOf(info), pos);
        }

        //SynchVar Callback
        private void OnFactionIndexChange(int index)
        {
            factionIndex = index;
            FactionInfo = networkController.playableFactions[factionIndex];
        }

        public void ChangeFaction(int index)
        {
            factionIndex = index;
        }


        [Command]
        public void CmdSpawnUnit(int spawnInfoIndex, Vector2 pos)
        {
            GameObject unit = Instantiate(unitContainerPrefab, pos, Quaternion.identity, this.transform);
            NetworkServer.SpawnWithClientAuthority(unit, connectionToClient);
            unit.GetComponent<UnitController>()
                .Init(_factionInfo.SpawnableUnits[spawnInfoIndex], manager.TeamId, this.transform);
            NetworkInstanceId netID = unit.GetComponent<NetworkIdentity>().netId;
            RpcInitUnit(netID, spawnInfoIndex, manager.TeamId);
            //need to find a way to init the unit on every clients 
        }

        [ClientRpc]
        public void RpcInitUnit(NetworkInstanceId id, int spawnInfoIndex, int teamID)
        {
            if (!isServer)
            {
                GameObject go = ClientScene.FindLocalObject(id);
                go.GetComponent<UnitController>()
                    .Init(_factionInfo.SpawnableUnits[spawnInfoIndex], teamID, this.transform);
            }
        }

    }
}