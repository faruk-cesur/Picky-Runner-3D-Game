using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CameraManager : MonoBehaviour
{
    public static Camera Cam;
    public static CameraManager instance;
    public PlayerController player;
    public Transform playerModel;
    public Transform cameraPosition;
    public Transform prepareCam;
    public Transform mainGameCam;
    public Transform finishCam;
    public Transform gameOverCam;


    private void Awake()
    {
        Cam = Camera.main;
    }

    private void Update()
    {
        CamFollow();
    }

    private void CamFollow()
    {
        if (!player.finishCam && GameManager.Instance.CurrentGameState != GameState.GameOver && GameManager.Instance.CurrentGameState != GameState.Prepare)
        {
            if (Cam.transform.parent != mainGameCam)
            {
                Cam.transform.SetParent(mainGameCam);
            }
            cameraPosition.position =
                Vector3.Lerp(cameraPosition.position, player.transform.position, Time.deltaTime * 3);
            Cam.transform.localRotation =
                Quaternion.Lerp(Cam.transform.localRotation, Quaternion.identity, Time.deltaTime * 2);


        }

        else if (!player.finishCam && GameManager.Instance.CurrentGameState == GameState.GameOver)
        {
            SmoothGameOver();
        }
        else if (!player.finishCam && GameManager.Instance.CurrentGameState == GameState.FinishGame)
        {
            playerModel.transform.rotation = new Quaternion(0, 180, 0, 0);
            SmoothFinish();
        }

        else if (player.finishCam && GameManager.Instance.CurrentGameState == GameState.FinishGame)
        {
            if (Cam.transform.parent != prepareCam)
            {
                Cam.transform.SetParent(prepareCam);
            }
        }

        if (GameManager.Instance.CurrentGameState == GameState.Prepare ||
            GameManager.Instance.CurrentGameState == GameState.MainGame)
        {
            Cam.transform.localPosition =
                Vector3.Lerp(Cam.transform.localPosition, Vector3.zero, Time.deltaTime * 0.75f);
        }
    }

    private void SmoothFinish()
    {
        Quaternion calculateRotation = Quaternion.LookRotation(playerModel.position - finishCam.transform.position);
        Cam.transform.localRotation =
            Quaternion.Lerp(Cam.transform.localRotation, calculateRotation, 0.8f * Time.deltaTime);
        mainGameCam.transform.localPosition =
            Vector3.Lerp(mainGameCam.transform.localPosition, Vector3.zero, 0.8f * Time.deltaTime);
        mainGameCam.transform.localRotation = Quaternion.Lerp(mainGameCam.transform.localRotation, Quaternion.identity, 0.8f * Time.deltaTime);
        Cam.transform.localPosition =
            Vector3.Lerp(Cam.transform.localPosition, finishCam.transform.localPosition, Time.deltaTime * 0.8f);
    }

    private void SmoothGameOver()
    {

        Quaternion calculateRotation = Quaternion.LookRotation(playerModel.position - gameOverCam.transform.position);
        Cam.transform.localRotation =
            Quaternion.Lerp(Cam.transform.localRotation, calculateRotation, 0.8f * Time.deltaTime);
        mainGameCam.transform.localPosition =
            Vector3.Lerp(mainGameCam.localPosition, Vector3.zero, 0.8f * Time.deltaTime);
        mainGameCam.transform.localRotation = Quaternion.Lerp(mainGameCam.transform.localRotation, Quaternion.identity, 0.8f * Time.deltaTime);
        Cam.transform.localPosition =
            Vector3.Lerp(Cam.transform.localPosition, gameOverCam.transform.localPosition, Time.deltaTime * 0.8f);

    }
}