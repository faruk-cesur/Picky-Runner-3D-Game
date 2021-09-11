using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathDrinkTrigger : MonoBehaviour
{
    public GameObject deathDrinkCanvas;
    
    public PlayerController player;
    private float _playerPositionZ;

    private void Update()
    {
        _playerPositionZ = player.transform.position.z;

        if (_playerPositionZ - 1 > transform.position.z)
        {
            deathDrinkCanvas.SetActive(false);
            Destroy(gameObject);
        }
    }
}
