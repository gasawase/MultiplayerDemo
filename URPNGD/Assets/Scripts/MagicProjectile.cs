using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MagicProjectile : NetworkBehaviour
{
    public NetworkVariable<int> damageNWVar = new NetworkVariable<int>(10);
}