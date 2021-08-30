using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidePlatform : MonoBehaviour
{
    public PlayerController player;
    private float _playerPositionZ;
    private void Update()
    {
        _playerPositionZ = player.transform.position.z;

        if (_playerPositionZ-5 > transform.position.z)
        {
            Destroy(gameObject);
        }
    }
}
