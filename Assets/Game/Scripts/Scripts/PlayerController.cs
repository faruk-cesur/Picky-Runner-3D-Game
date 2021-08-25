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

    [SerializeField] private RayfireRigid rayfireRigid;

    private bool _isWallBreakable;
    private bool _isPlayerSelectedDoor;
    private bool _isPlayerJumped;
    private bool _isPlayerMoved;
    private bool _isTriggerAttack;


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
            if (!_isWallBreakable)
            {
                runSpeed = 0;
                GameManager.Instance.GameOver();
                AnimationController.Instance.ActivateDeathAnim();
            }
            else
            {
                Invoke(nameof(DisappearObjects), 1f);
                _isPlayerSelectedDoor = false;
                Destroy(other.gameObject, 6);
            }
        }

        TriggerAttack triggerAttack = other.GetComponentInParent<TriggerAttack>();
        if (triggerAttack)
        {
            if (_isTriggerAttack == false)
            {
                StartCoroutine(TriggerAttackBool());
                StartCoroutine(AnimationController.Instance.ActivateAttackAnim());
                StartCoroutine(PlayerAttackRootMotion());
            }
        }

        BaseballDoor baseballDoor = other.GetComponentInParent<BaseballDoor>();
        if (baseballDoor)
        {
            if (_isPlayerSelectedDoor == false)
            {
                _isWallBreakable = true;
                baseballObject.SetActive(true);
                _isPlayerSelectedDoor = true;
            }
        }

        AxeDoor axeDoor = other.GetComponentInParent<AxeDoor>();
        if (axeDoor)
        {
            if (_isPlayerSelectedDoor == false)
            {
                _isWallBreakable = true;
                axeObject.SetActive(true);
                _isPlayerSelectedDoor = true;
            }
        }

        YellowCapDoor yellowCapDoor = other.GetComponentInParent<YellowCapDoor>();
        if (yellowCapDoor)
        {
            if (_isPlayerSelectedDoor == false)
            {
                _isWallBreakable = false;
                yellowCapObject.SetActive(true);
                _isPlayerSelectedDoor = true;
            }
        }

        LifeBuoyDoor lifeBuoyDoor = other.GetComponentInParent<LifeBuoyDoor>();
        if (lifeBuoyDoor)
        {
            if (_isPlayerSelectedDoor == false)
            {
                _isWallBreakable = false;
                lifeBuoyObject.SetActive(true);
                _isPlayerSelectedDoor = true;
            }
        }

        JumpPlatform jumpPlatform = other.GetComponentInParent<JumpPlatform>();
        if (jumpPlatform)
        {
            if (!_isPlayerMoved)
            {
                UIManager.Instance.energySlider.value--;
                UIManager.Instance.EnergySliderStars();
                StartCoroutine(AnimationController.Instance.ActivateJumpAnim());
                StartCoroutine(JumpPosition());
                StartCoroutine(PlayerMovingBool());
            }
        }

        SlidePlatform slidePlatform = other.GetComponentInParent<SlidePlatform>();
        if (slidePlatform)
        {
            if (!_isPlayerMoved)
            {
                UIManager.Instance.energySlider.value--;
                UIManager.Instance.EnergySliderStars();
                StartCoroutine(AnimationController.Instance.ActivateSlideAnim());
                StartCoroutine(PlayerMovingBool());
            }
        }


        Collectable collectable = other.GetComponentInParent<Collectable>();
        if (collectable)
        {
            UIManager.Instance.energySlider.value++;
            UIManager.Instance.EnergySliderStars();
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

    private IEnumerator JumpPosition()
    {
        if (!_isPlayerJumped)
        {
            _isPlayerJumped = true;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.up * 13;
            yield return new WaitForSeconds(0.5f);
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.down * 10;
            _isPlayerJumped = false;
            yield return new WaitForSeconds(1f);
            gameObject.transform.position = new Vector3(0, 0, transform.position.z);
        }
    }

    private IEnumerator PlayerMovingBool()
    {
        _isPlayerMoved = true;
        yield return new WaitForSeconds(1f);
        _isPlayerMoved = false;
    }

    private IEnumerator TriggerAttackBool()
    {
        _isTriggerAttack = true;
        yield return new WaitForSeconds(1f);
        _isTriggerAttack = false;
    }

    private IEnumerator PlayerAttackRootMotion()
    {
        runSpeed = 0;
        playerModelChild.GetComponent<Animator>().applyRootMotion = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        yield return new WaitForSeconds(1.55f);

        rayfireRigid.Initialize();
        rayfireRigid.Fade();
        runSpeed = 10;
        playerModelChild.GetComponent<Animator>().applyRootMotion = false;
        playerModelChild.transform.rotation = Quaternion.identity;

        yield return new WaitForSeconds(1.25f);

        playerModelChild.transform.localPosition = new Vector3(0, 0, 0);

        yield return new WaitForSeconds(2f);

        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }
}