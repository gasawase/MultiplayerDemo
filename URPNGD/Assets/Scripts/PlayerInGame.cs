using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInGame : Player
{
    private void Start() {
        ApplyPlayerMesh();
        PlayerMeshInt.OnValueChanged += OnPlayerMeshChanged;
    }
    
    public void ApplyPlayerMesh()
    {
        meshHolder.GetComponent<SkinnedMeshRenderer>().sharedMesh = listOfMeshes[PlayerMeshInt.Value];
    }
    
    public void OnPlayerMeshChanged(int previous, int current)
    {
        ApplyPlayerMesh();
    }

}
