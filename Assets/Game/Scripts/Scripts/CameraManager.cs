using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CameraManager : MonoBehaviour
{
    public static Camera Cam;
    public static CameraManager Instance;

    public PlayerController player;

    public Transform playerModel;
    public Transform cameraPosition;
    public Transform prepareCam;
    public Transform mainGameCam;
    public Transform finishCam;
    public Transform gameOverCam;

    [HideInInspector] public bool isSlideCamera;

    private Vector3 _mainGameCamTempPos;


    private void Awake()
    {
        Cam = Camera.main;
        Instance = this;
        isSlideCamera = true;
        _mainGameCamTempPos = mainGameCam.localPosition;
    }

    private void Update()
    {
        if (isSlideCamera)
        {
            CamFollow();
        }
        else
        {
            SlideCamera();
        }
    }

    public void SlideCamera()
    {
        StartCoroutine(SlideCameraBool());
    }

    private void CamFollow()
    {
        if (!player.finishCam && GameManager.Instance.CurrentGameState != GameState.GameOver &&
            GameManager.Instance.CurrentGameState != GameState.Prepare)
        {
            if (Cam.transform.parent != mainGameCam)
            {
                Cam.transform.SetParent(mainGameCam);
            }

            cameraPosition.position =
                Vector3.Lerp(cameraPosition.position, player.transform.position, Time.deltaTime * 2);
            Cam.transform.localRotation =
                Quaternion.Lerp(Cam.transform.localRotation, Quaternion.identity, Time.deltaTime * 2);
        }

        else if (!player.finishCam && GameManager.Instance.CurrentGameState == GameState.GameOver)
        {
            SmoothGameOver();
        }
        else if (player.finishCam && GameManager.Instance.CurrentGameState == GameState.FinishGame)
        {
            playerModel.transform.rotation = new Quaternion(0, 180, 0, 0);
            SmoothFinish();
        }

        else if (!player.finishCam && GameManager.Instance.CurrentGameState == GameState.Prepare)
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

        mainGameCam.transform.localRotation = Quaternion.Lerp(mainGameCam.localRotation, Quaternion.identity,
            0.8f * Time.deltaTime);

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

        mainGameCam.transform.localRotation = Quaternion.Lerp(mainGameCam.localRotation, Quaternion.identity,
            0.8f * Time.deltaTime);

        Cam.transform.localPosition =
            Vector3.Lerp(Cam.transform.localPosition, gameOverCam.transform.localPosition, Time.deltaTime * 0.8f);
    }

    private IEnumerator SlideCameraBool()
    {
        mainGameCam.localPosition = Vector3.Lerp(mainGameCam.localPosition,
            new Vector3(0f, 3f, 3f), Time.deltaTime * 2.5f);

        yield return new WaitForSeconds(1f);

        mainGameCam.localPosition = Vector3.Lerp(mainGameCam.localPosition,
            _mainGameCamTempPos, Time.deltaTime * 2.5f);

        isSlideCamera = true;
    }
}