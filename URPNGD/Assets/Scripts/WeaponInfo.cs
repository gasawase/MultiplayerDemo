using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;
using Unity.Collections;
using Unity.VisualScripting;

public class WeaponInfo : NetworkBehaviour
{
    [SerializeField] private int damageNum;
    [SerializeField] private GameObject weaponModel;
    public NetworkVariable<int> damageNWVar;

    private void Start()
    {
        damageNWVar.Value = damageNum;
    }
}