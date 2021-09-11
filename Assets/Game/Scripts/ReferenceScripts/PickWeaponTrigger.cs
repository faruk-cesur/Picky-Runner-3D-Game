using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickWeaponTrigger : MonoBehaviour
{
    public GameObject pickWeaponCanvas;
    
    public PlayerController player;
    private float _playerPositionZ;

    private void Update()
    {
        _playerPositionZ = player.transform.position.z;

        if (_playerPositionZ - 1 > transform.position.z)
        {
            pickWeaponCanvas.SetActive(false);
            Destroy(gameObject);
        }
    }
}
