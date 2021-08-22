using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using RayFire;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public bool finishCam;
    [SerializeField] private float runSpeed;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float maxSlideAmount;

    [SerializeField] private Transform playerModel;

    [SerializeField] private GameObject playerModelChild;
    [SerializeField] private GameObject baseballObject;
    [SerializeField] private GameObject axeObject;
    [SerializeField] private GameObject yellowCapObject;
    [SerializeField] private GameObject lifeBuoyObject;

    private GameObject _currentItem;

    private bool _isBreakable;
    private bool _isSelectedDoor;
    private bool _isJumped;

    private int _specialPower;

    private void Start()
    {
        DisappearObjects();
    }

    private void Update()
    {
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.Prepare:
                AnimationController.Instance.ActivateIdleAnim();
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
            mousePosX = CameraManager.Cam.ScreenToViewportPoint(Input.mousePosition).x;
        }

        if (Input.GetMouseButton(0))
        {
            float currentMousePosX = CameraManager.Cam.ScreenToViewportPoint(Input.mousePosition).x;
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


    private void OnTriggerEnter(Collider other)
    {
        RayfireRigid wall = other.GetComponentInParent<RayfireRigid>();
        if (wall)
        {
            if (!_isBreakable)
            {
                runSpeed = 0;
                GameManager.Instance.GameOver();
                AnimationController.Instance.ActivateDeathAnim();
            }
            else
            {
                Invoke(nameof(DisappearObjects), 2f);
                StartCoroutine(PlayerWallStrike());
                StartCoroutine(AnimationController.Instance.ActivateWallBreakAnim());
                _isSelectedDoor = false;
            }
        }

        BaseballDoor baseballDoor = other.GetComponentInParent<BaseballDoor>();
        if (baseballDoor)
        {
            if (_isSelectedDoor == false)
            {
                _isBreakable = true;
                baseballObject.SetActive(true);
                _isSelectedDoor = true;
            }
        }

        AxeDoor axeDoor = other.GetComponentInParent<AxeDoor>();
        if (axeDoor)
        {
            if (_isSelectedDoor == false)
            {
                _isBreakable = true;
                axeObject.SetActive(true);
                _isSelectedDoor = true;
            }
        }

        YellowCapDoor yellowCapDoor = other.GetComponentInParent<YellowCapDoor>();
        if (yellowCapDoor)
        {
            if (_isSelectedDoor == false)
            {
                _isBreakable = false;
                yellowCapObject.SetActive(true);
                _isSelectedDoor = true;
            }
        }

        LifeBuoyDoor lifeBuoyDoor = other.GetComponentInParent<LifeBuoyDoor>();
        if (lifeBuoyDoor)
        {
            if (_isSelectedDoor == false)
            {
                _isBreakable = false;
                lifeBuoyObject.SetActive(true);
                _isSelectedDoor = true;
            }
        }

        JumpPlatform jumpPlatform = other.GetComponentInParent<JumpPlatform>();
        if (jumpPlatform)
        {
            StartCoroutine(AnimationController.Instance.ActivateJumpAnim());
            StartCoroutine(JumpPosition());
        }

        SlidePlatform slidePlatform = other.GetComponentInParent<SlidePlatform>();
        if (slidePlatform)
        {
            StartCoroutine(AnimationController.Instance.ActivateSlideAnim());
        }


        Collectable collectable = other.GetComponentInParent<Collectable>();
        if (collectable)
        {
            _specialPower++;
            UIManager.Instance.energySlider.value = _specialPower;
            Destroy(other.gameObject);
        }
    }

    public void PlayerSpeedDown()
    {
        StartCoroutine(FinishGame());
    }

    private void DisappearObjects()
    {
        baseballObject.SetActive(false);
        yellowCapObject.SetActive(false);
        axeObject.SetActive(false);
        lifeBuoyObject.SetActive(false);
    }

    private IEnumerator FinishGame()
    {
        float timer = 0;
        float fixSpeed = runSpeed;
        while (true)
        {
            timer += Time.deltaTime;
            runSpeed = Mathf.Lerp(fixSpeed, 0, timer);

            if (timer >= 1f)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }


    private IEnumerator PlayerWallStrike()
    {
        runSpeed = 0;
        playerModelChild.transform.rotation = Quaternion.Euler(0, 110, 0);
        playerModelChild.GetComponent<Animator>().applyRootMotion = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds(2f);
        playerModelChild.GetComponent<Animator>().applyRootMotion = false;
        runSpeed = 10;
        playerModelChild.transform.rotation = Quaternion.identity;
        playerModelChild.transform.localPosition = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(2f);
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    private IEnumerator JumpPosition()
    {
        if (!_isJumped)
        {
            _isJumped = true;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.up * 13;
            yield return new WaitForSeconds(0.5f);
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.down * 10;
            _isJumped = false;
            yield return new WaitForSeconds(1f);
            gameObject.transform.position = new Vector3(0, 0, transform.position.z);
        }
    }
}