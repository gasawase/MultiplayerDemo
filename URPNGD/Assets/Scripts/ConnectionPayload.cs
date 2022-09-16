using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[Serializable] 
public class ConnectionPayload
{
    public ulong networkClientID;
    public string networkPlayerName;
    public bool networkPlayerReady;
    
    
}
