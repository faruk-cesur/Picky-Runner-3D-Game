using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    public PlayerController player;
    private float _playerPositionZ;

    private void Update()
    {
        _playerPositionZ = player.transform.position.z;

        if (_playerPositionZ - 5 > transform.position.z)
        {
            Destroy(gameObject);
        }
    }
}