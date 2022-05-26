using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject camera;
    private Vector3 startPostion = Vector3.up *3;

    private void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, startPostion, Quaternion.identity);
    }
}
