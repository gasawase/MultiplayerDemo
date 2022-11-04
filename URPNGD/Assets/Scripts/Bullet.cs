using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public NetworkVariable<int> damage = new NetworkVariable<int>(10);
}