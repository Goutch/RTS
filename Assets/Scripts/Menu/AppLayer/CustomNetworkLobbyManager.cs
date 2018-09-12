using System;
using System.Collections.Generic;
using DefaultNamespace;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace AppLayer
{
    public class CustomNetworkLobbyManager : LobbyManager
    {
        [SerializeField] public List<UnitRaceInfo> playableRaces;
        [NonSerialized]public GameObject gamePlayer;

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
           this.gamePlayer = gamePlayer;
            CustomLobbyPlayer customLobbyPlayer = lobbyPlayer.GetComponent<CustomLobbyPlayer>();
            PlayerManager player = gamePlayer.GetComponent<PlayerManager>();
            player.name = customLobbyPlayer.playerName;
            player.TeamId = customLobbyPlayer.TeamID;
            player.GetComponent<Spawner>().RaceInfo = playableRaces[customLobbyPlayer.RaceInfoIndex];
            ClientScene.Ready(client.connection);
           return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer,gamePlayer);
        }

    }
}