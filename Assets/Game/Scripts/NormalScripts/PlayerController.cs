using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float slideSpeed = 5f;
    [SerializeField] private float maxSlideAmount = 4f;

    [SerializeField] private Transform playerModel;

    private void Start()
    {
    }

    private void Update()
    {
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.Prepare:
                AnimationController.instance.ActivateIdleAnim();
                break;
            case GameState.MainGame:
                VerticleMovement(runSpeed);
                HorizontalMovement(slideSpeed);
                break;
            case GameState.GameOver:
                break;
            case GameState.FinishGame:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #region PlayerMovement
    private void VerticleMovement(float runSpeed)
    {
        transform.position += Vector3.forward * runSpeed * Time.deltaTime;
    }

    private float mousePosX;
    private float playerVisualPosX;
    private void HorizontalMovement(float slideSpeed)
    {
        if (Input.GetMouseButtonDown(0))
        {
            playerVisualPosX = playerModel.localPosition.x;
            mousePosX = CameraManagerScript.Cam.ScreenToViewportPoint(Input.mousePosition).x;
        }
        if (Input.GetMouseButton(0))
        {
            float currentMousePosX = CameraManagerScript.Cam.ScreenToViewportPoint(Input.mousePosition).x;
            float distance = currentMousePosX - mousePosX;
            float posX = playerVisualPosX + (distance * slideSpeed);
            Vector3 pos = playerModel.localPosition;
            pos.x = Mathf.Clamp(posX, -maxSlideAmount, maxSlideAmount);
            playerModel.localPosition = pos;
        }
        else
        {
            Vector3 pos = playerModel.localPosition;
        }
    }

    #endregion
}