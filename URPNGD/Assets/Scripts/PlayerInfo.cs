using System;
using Unity.Netcode;

    // This script stores each players information ON THE SERVER
    public struct PlayerInfo : INetworkSerializable, System.IEquatable<PlayerInfo> 
    {
        public ulong clientId;

        public bool isReady;

        public PlayerInfo(ulong id, bool ready = false)
        {
            clientId = id;
            isReady = ready;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref clientId);
            serializer.SerializeValue(ref isReady);
        }

        public bool Equals(PlayerInfo other)
        {
            return other.clientId == clientId;
        }
        // public ulong networkClientID;
        // //private string networkPlayerName;
        // public bool networkPlayerReady;
        // public NetworkNameState.FixedPlayerName m_PlayerName;
        //
        //
        // public PlayerInfo(ulong clientId, string nwPName, bool playerReady)
        // {
        //     networkClientID = clientId;
        //     //networkPlayerName = nwPName;
        //     networkPlayerReady = playerReady;
        //     m_PlayerName = new NetworkNameState.FixedPlayerName();
        //     PlayerName = nwPName;
        // }
        //
        // public string PlayerName
        // {
        //     get => m_PlayerName;
        //     private set => m_PlayerName = value;
        // }
        // public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        // {
        //     serializer.SerializeValue(ref networkClientID);
        //     serializer.SerializeValue(ref m_PlayerName);
        //     serializer.SerializeValue(ref networkPlayerReady);
        // }
        //
        // public bool Equals(PlayerInfo other)
        // {
        //     return networkClientID == other.networkClientID &&
        //            networkPlayerReady == other.networkPlayerReady &&
        //            m_PlayerName.Equals(other.m_PlayerName);
        // }



    }    


