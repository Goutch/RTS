using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace AppLayer
{
    public delegate void CustomNetworkEventHandler();

    public class CustomNetworkLobbyManager : LobbyManager
    {
        [SerializeField] public List<UnitFactionInfo> playableFactions;
        [NonSerialized] public GameObject gamePlayer;
        private NetworkConnection connectionToServer;
        public event CustomNetworkEventHandler OnAllPlayerLoaded;

        //called on server only
        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            //when it is called on client owned object only authority can update synchvar so the callbacks are not called cause its the server that called them
            this.gamePlayer = gamePlayer;
            CustomLobbyPlayer customLobbyPlayer = lobbyPlayer.GetComponent<CustomLobbyPlayer>();
            PlayerManager player = gamePlayer.GetComponent<PlayerManager>();
            player.ChangePlayerName(customLobbyPlayer.playerName);
            player.ChangeTeamId(customLobbyPlayer.TeamID);
            gamePlayer.GetComponent<Spawner>().ChangeFaction(customLobbyPlayer.factionInfoIndex);
            return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
        }
        //called on server when client is ready
        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
            bool allPlayersLoaded = true;
            foreach (var con in NetworkServer.connections)
            {
                if (!con.isReady)
                {
                    allPlayersLoaded = false;
                }
            }

            if (allPlayersLoaded)
                if (OnAllPlayerLoaded != null)
                    OnAllPlayerLoaded();
        }

        private IEnumerator StartGameIntreeSecRoutine()
        {
            yield return new WaitForSeconds(3);
            if (OnAllPlayerLoaded != null)
                OnAllPlayerLoaded();
        }
    }
   
}