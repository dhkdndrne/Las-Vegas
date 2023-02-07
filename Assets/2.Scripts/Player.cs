using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
}
