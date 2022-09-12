using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering.Universal;

public class PlayerSelector : MonoBehaviour
{
    public void SelectCharacter(int characterIndex)
    {
        //get the local client's id
        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        //try to get the local client object
        //return if unsuccessful
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
        {
            return;
        }
        
        //try to get the Player component from the player object
        // return if unsuccessful
        if (!networkClient.PlayerObject.TryGetComponent<Player>(out Player player))
        {
            return;
        }
        
        player.SetPlayerServerRpc((byte)characterIndex);
    }
}
