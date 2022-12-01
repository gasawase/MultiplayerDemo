using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] public Player playerPrefab;
    [SerializeField] public GameObject spawnPoints;

    private int spawnIndex = 0;
    private List<Vector3> availSpawnPos = new List<Vector3>();

    private void Awake()
    {
        RefreshSpawnPoints();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SpawnPlayerServerRpc();
        }
    }
    
    private void RefreshSpawnPoints()
    {
        Transform[] allPoints = spawnPoints.GetComponentsInChildren<Transform>();
        availSpawnPos.Clear();
        foreach (Transform point in allPoints)
        {
            if (point != spawnPoints.transform)
            {
                availSpawnPos.Add(point.localPosition);
            }
        }
    }

    public Vector3 GetNextSpawnLocation()
    {
        var newPosition = availSpawnPos[spawnIndex];
        newPosition.y = 1.5f;
        spawnIndex++;
        
        if (spawnIndex > availSpawnPos.Count - 1) {
            spawnIndex = 0;
        }
        
        return newPosition;
    }
    
    private void SpawnPlayers()
    {
        foreach (PlayerInfo pi in GameData.Instance.allPlayers)
        {
            Player playerSpawn = Instantiate(playerPrefab, GetNextSpawnLocation(), Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(pi.clientId);
            Debug.Log($"PLAYER INFO || clientId = {pi.clientId} ; PlayerName = {pi.PlayerName} ; CurrentMesh = {pi.playMeshSelect}");
            playerSpawn.PlayerMeshInt.Value = pi.playMeshSelect;
            //playerSpawn.PlayerColor.Value = pi.color;
        }
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ServerRpcParams serverRpcParams = default) //ask server to send info from this script to the client?
    {
        SpawnPlayers();
    }

    // [ClientRpc]
    // public void SpawnPlayersClientRpc(ClientRpcParams clientRpcParams = default)
    // {
    //     SpawnPlayers();
    // }
}
