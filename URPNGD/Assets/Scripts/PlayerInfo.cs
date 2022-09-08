using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerInfo : INetworkSerializable
{
    public ulong networkClientID;
    public string networkPlayerName;
    public bool networkPlayerReady;

    public PlayerInfo(ulong clientId, string nwPName, bool playerReady)
    {
        networkClientID = clientId;
        networkPlayerName = nwPName;
        networkPlayerReady = playerReady;
    }
    

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref networkClientID);
        serializer.SerializeValue(ref networkPlayerName);
        serializer.SerializeValue(ref networkPlayerReady);
    }
}