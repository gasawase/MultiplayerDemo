using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Summer.Multiplayer
{
    // This script stores each players information ON THE SERVER
    public struct PlayerInfo : INetworkSerializable, IEquatable<PlayerInfo>
    {
        public ulong networkClientID;
        //public string networkPlayerName;
        public bool networkPlayerReady;
        private NetworkNameState.FixedPlayerName m_PlayerName;
        

        public PlayerInfo(ulong clientId, string nwPName, bool playerReady)
        {
            networkClientID = clientId;
            //networkPlayerName = nwPName;
            networkPlayerReady = playerReady;
            m_PlayerName = new NetworkNameState.FixedPlayerName();
            PlayerName = nwPName;
        }
        
        public string PlayerName
        {
            get => m_PlayerName;
            private set => m_PlayerName = value;
        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref networkClientID);
            serializer.SerializeValue(ref m_PlayerName);
            serializer.SerializeValue(ref networkPlayerReady);
        }

        public bool Equals(PlayerInfo other)
        {
            return networkClientID == other.networkClientID && networkPlayerReady == other.networkPlayerReady && m_PlayerName.Equals(other.m_PlayerName);
        }
        
    }    
}


