using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInGame : Player
{
    // Start is called before the first frame update
    void Awake()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        int myIndex = GameData.Instance.FindPlayerIndex(NetworkManager.LocalClientId);
        PlayerInfo info = GameData.Instance.allPlayers[myIndex];
        skinnedMeshRenderer.sharedMesh = listOfMeshes[info.playMeshSelect];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
