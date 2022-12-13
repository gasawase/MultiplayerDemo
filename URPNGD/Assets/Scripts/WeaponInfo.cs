using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;
using Unity.Collections;

public class WeaponInfo : MonoBehaviour
{
    [SerializeField] private int damageNum;
    [SerializeField] private GameObject weaponModel;
}