using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] public Player playerPrefab;
    [SerializeField] public GameObject spawnPoints;

    //public Dictionary<ulong, GameObject> allPlayersSpawned = new Dictionary<ulong, GameObject>();

    private int spawnIndex = 0;
    private Camera _camera;
    private List<Vector3> availSpawnPos = new List<Vector3>();

    private void Awake()
    {
        RefreshSpawnPoints();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            //SpawnPlayerServerRpc();
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
    
    private void SpawnPlayers()
    {
        foreach (PlayerInfo pi in GameData.Instance.allPlayers)
        {
            Player playerSpawn = Instantiate(playerPrefab, GetNextSpawnLocation(), Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(pi.clientId);
            //playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(pi.clientId);
            Debug.Log($"PLAYER INFO || clientId = {pi.clientId} ; PlayerName = {pi.PlayerName} ; CurrentMesh = {pi.playMeshSelect}");
            playerSpawn.PlayerMeshInt.Value = pi.playMeshSelect;
           
            ulong[] singleTarget = new ulong[1];
            singleTarget[0] = pi.clientId;
            ClientRpcParams rpcParams = default;
            rpcParams.Send.TargetClientIds = singleTarget;
            RecievePlayerNameClientRpc(pi.m_PlayerName, rpcParams);
            GameData.Instance.allPlayersSpawned.Add(pi.clientId, playerSpawn.gameObject); //TODO need to handle client disconnecting
            //playerSpawn.PlayerColor.Value = pi.color;
        }
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ServerRpcParams serverRpcParams = default) //ask server to send info from this script to the client?
    {
        SpawnPlayers();
    }

    [ClientRpc]
    public void RecievePlayerNameClientRpc(string pName, ClientRpcParams clientRpcParams = default)
    {
        GameObject.Find("PlayerName").GetComponent<TMP_Text>().text = pName;
    }
}
