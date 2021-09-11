using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDrinkTrigger : MonoBehaviour
{
    public GameObject deathDrinkCanvas;
    public GameObject energyDrinkCanvas;
    
    public PlayerController player;
    private float _playerPositionZ;

    private void Update()
    {
        _playerPositionZ = player.transform.position.z;

        if (_playerPositionZ - 1 > transform.position.z)
        {
            energyDrinkCanvas.SetActive(false);
            deathDrinkCanvas.SetActive(true);
            Destroy(gameObject);
        }
    }
}
