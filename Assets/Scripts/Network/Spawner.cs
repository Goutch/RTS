using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Abilities;
using DefaultNamespace;
using UnitComponent;
using UnityEngine;
using UnityEngine.Networking;

namespace Player
{
    public class Spawner : NetworkBehaviour
    {
        [SerializeField] private GameObject unitContainerPrefab;
        [SerializeField] private GameObject projectileContainerPrefab;
        private readonly List<Projectile> spawnableProjectiles=new List<Projectile>();
        private NetworkPlayerConnection _playerConnection;
        private UnitFactionData _factionData;
        private RTSNetworkManager _networkManager;
        private Transform gamePlayerObjectTransform;

        private UnitFactionData FactionData
        {
            get { return _factionData; }
            set
            {
                Destroy(_factionData);
                _factionData = Instantiate(value);
                foreach (var unit in _factionData.SpawnableUnits)
                {
                    foreach (var ability in unit.Abilities)
                    {
                        if (ability is Projectile)
                        {
                            spawnableProjectiles.Add((Projectile) ability);
                        }
                    }
                }
            }
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

        public void SpawnProjectile(Projectile proj, Vector2 pos, Vector2 dir, int teamId)
        {
            CmdSpawnProjectile(spawnableProjectiles.IndexOf(proj), pos, dir, teamId);
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

        [Command]
        public void CmdSpawnProjectile(int dataPrefabIndex, Vector2 pos, Vector2 dir, int teamId)
        {
            GameObject proj = Instantiate(projectileContainerPrefab, pos, Quaternion.identity);
            NetworkServer.Spawn(proj);
            RpcSpawnProjectile(proj.GetComponent<NetworkIdentity>().netId, dir, dataPrefabIndex, teamId);
        }

        [ClientRpc]
        public void RpcSpawnProjectile(NetworkInstanceId netId, Vector2 dir, int dataPrefabIndex, int teamId)
        {
            ProjectileController proj = ClientScene.FindLocalObject(netId).GetComponent<ProjectileController>();
            proj.Init(spawnableProjectiles[dataPrefabIndex], dir, teamId);
        }
    }
}