using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerPlus : NetworkManager
{
    [SerializeField]
    List<Transform> m_spawnPositions = new List<Transform>();

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = (GameObject)Instantiate(playerPrefab, GetSpawnPosition(numPlayers), Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        
        player.GetComponent<PlayerLogic>().SetTeam((numPlayers - 1) % 2 == 0 ? Team.Blue : Team.Red);
        
        Debug.Log("Player spawned with Index: " + (numPlayers - 1));
    }

    Vector3 GetSpawnPosition(int spawnIndex)
    {
        return m_spawnPositions[spawnIndex].position;
    }
}
