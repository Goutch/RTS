using UnityEngine;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public class Spawner : NetworkBehaviour
    {
        [SerializeField] private UnitRaceInfo raceInfo;
        [SerializeField] private GameObject unitContainerPrefab;
        [SerializeField] private CustomNetworkManager networkController;
        private int unitId;

        public UnitRaceInfo RaceInfo
        {
            get;
            set;
        }

        private PlayerManager manager;


        public void SpawnStartingUnits()
        {
            raceInfo = Instantiate(raceInfo);
            manager = GetComponent<PlayerManager>();
            networkController = GameObject.FindGameObjectWithTag("GameController").GetComponent<CustomNetworkManager>();

            for (int i = 0; i < raceInfo.StartUnits.Length; i++)
            {
                CmdSpawnUnit(raceInfo.SpawnableUnits.IndexOf(raceInfo.StartUnits[i]),
                    raceInfo.StartUnitsOffSetPosition[i]);
            }
        }
        public void SpawnUnit(UnitInfo info, Vector2 pos)
        {
            CmdSpawnUnit(raceInfo.SpawnableUnits.IndexOf(info), pos);
        }

        [Command]
        public void CmdSpawnUnit(int spawnInfoIndex, Vector2 pos)
        {
            GameObject unit = Instantiate(unitContainerPrefab, pos, Quaternion.identity, this.transform);
            NetworkServer.SpawnWithClientAuthority(unit, connectionToClient);
            unit.GetComponent<UnitController>().Init(raceInfo.SpawnableUnits[spawnInfoIndex],manager.TeamId,this.transform);
            NetworkInstanceId netID= unit.GetComponent<NetworkIdentity>().netId;
            RpcInitUnit(netID,spawnInfoIndex,manager.TeamId);
            //need to find a way to init the unit on every clients 
        }

        [ClientRpc]
        public void RpcInitUnit(NetworkInstanceId id,int spawnInfoIndex,int teamID)
        {
            if(!isServer)
                ClientScene.FindLocalObject(id).GetComponent<UnitController>().Init(raceInfo.SpawnableUnits[spawnInfoIndex],teamID,this.transform);
        }
    }
}