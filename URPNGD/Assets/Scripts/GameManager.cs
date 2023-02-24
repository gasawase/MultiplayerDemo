using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Manages the first level of the game
/// </summary>

public class GameManager : NetworkBehaviour
{
    [SerializeField] public Player playerPrefab;
    [SerializeField] public PlayerHUDManager hudPrefab;
    [SerializeField] public GameObject spawnPoints;

    //public Dictionary<ulong, GameObject> allPlayersSpawned = new Dictionary<ulong, GameObject>();

    private int spawnIndex = 0;
    //private Camera _camera;
    private List<Vector3> availSpawnPos = new List<Vector3>();
    private PlayerHUDManager hudSpawn;

    private void Awake()
    {
        RefreshSpawnPoints();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SpawnPlayers();
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
    
    // Spawns players for the scene 
    private void SpawnPlayers()
    {
        foreach (PlayerInfo pi in GameData.Instance.allPlayers)
        {
            Player playerSpawn = Instantiate(playerPrefab, GetNextSpawnLocation(), Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(pi.clientId);
            Debug.Log($"PLAYER INFO || clientId = {pi.clientId} ; PlayerName = {pi.PlayerName} ; CurrentMesh = {pi.playMeshSelect}");
            playerSpawn.PlayerMeshInt.Value = pi.playMeshSelect;
           
            // sends this to only one target
            // 2_22_23, I don't think this portion does anything
            ulong[] singleTarget = new ulong[1];
            singleTarget[0] = pi.clientId;
            ClientRpcParams rpcParams = default;
            rpcParams.Send.TargetClientIds = singleTarget;
            //

            GameData.Instance.allPlayersSpawned.Add(pi.clientId, playerSpawn.gameObject); //TODO need to handle client disconnecting
        }

        // create HUDs here 
        // TODO: create a checking system to make sure that all players are actually in the scene

        // foreach (PlayerInfo pi in GameData.Instance.allPlayers)
        // {
        //     // if the current playerInfo data matches this local player do this 
        //     if (pi.clientId == GameData.Instance.allPlayersSpawned[pi.clientId].GetComponent<NetworkObject>().OwnerClientId)
        //     {
        //         if (hudSpawn != null)
        //         {
        //             Destroy(hudSpawn);
        //         }
        //         hudSpawn = Instantiate(hudPrefab);
        //         hudSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(pi.clientId);
        //         Debug.Log($"{pi.clientId}");
        //         //hudSpawn.SetUpPlayerHUD();
        //     }
        // }

        SendHuDsActivationClientRpc();
        //SendAllPlayersSpawnedClientRpc();

    }

    [ClientRpc]
    public void SendHuDsActivationClientRpc()
    {
        hudSpawn = Instantiate(hudPrefab);
        hudSpawn.SetUpPlayerHUD();

    }

    // [ClientRpc]
    // public void SendAllPlayersSpawnedClientRpc()
    // {
    //     PlayerAttributes[] allPlayerAttributes = FindObjectsOfType<PlayerAttributes>();
    //     foreach()
    // }
    
}
