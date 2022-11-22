using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;
using Unity.Collections;

public struct PlayerInfo : INetworkSerializable, System.IEquatable<PlayerInfo> {
    public ulong clientId;
    public Color color;
    public bool isReady;
    public FixedPlayerName m_PlayerName;

    public PlayerInfo(ulong id, string name, Color c, bool ready=false) {
        clientId = id;
        m_PlayerName = new FixedPlayerName();
        color = c;
        isReady = ready;

        PlayerName = name;
    }

    public string PlayerName
    {
        get => m_PlayerName;
        private set => m_PlayerName = value;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref m_PlayerName);
        serializer.SerializeValue(ref color);
        serializer.SerializeValue(ref isReady);
    }

    public bool Equals(PlayerInfo other) {
        return other.clientId == clientId &&
               m_PlayerName.Equals(other.m_PlayerName);
    }
}