using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainLevel : NetworkBehaviour
{
    public Player playerPrefab;

    [SerializeField] public List<Transform> SpawnPoints;

    private int spawnIndex = 0;
    
    void Awake()
    {
        foreach (PlayerInfo tmpClient in GameManager.instance.playerList)
        {
            Transform currentPoint = SpawnPoints[spawnIndex];
            Player playerSpawn = Instantiate(playerPrefab, currentPoint.position, Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(tmpClient.clientId);
            spawnIndex++;
        }
    }
}
