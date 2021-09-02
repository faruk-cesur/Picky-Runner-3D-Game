using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using RayFire;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    #region Fields

    [HideInInspector] public bool finishCam;

    [SerializeField] private float runSpeed, slideSpeed, maxSlideAmount;

    [SerializeField] private Transform playerModel;

    [SerializeField] private GameObject playerModelChild;

    [SerializeField] private GameObject baseballObject,
        axeObject,
        pickaxeObject,
        spearObject,
        swordObject,
        thorHammerObject,
        yellowCapObject,
        lifeBuoyObject,
        backpackObject,
        hairDryerObject,
        pillowObject,
        umbrellaObject;

    [SerializeField] private GameObject particleCollectable, particleFiveStars, particleBuffDoor;

    [SerializeField] private RayfireRigid rayfireRigid;

    private bool _isWallBreakable,
        _isPlayerSelectedDoor,
        _isPlayerJumped,
        _isPlayerMoved,
        _isPlayerSlideJump,
        _isPlayerUsedEnergy,
        _isTriggerAttack,
        _isParticleStarTrail,
        _isPlayerDead,
        _isPlayerInteract;

    private GameObject _particleStarTrailTemp;

    #endregion


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
                FiveStarParticle();
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

    private float _mousePosX;
    private float _playerVisualPosX;


    private void HorizontalMovement(float slideSpeed)
    {
        if (!_isPlayerInteract)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _playerVisualPosX = playerModel.localPosition.x;
                _mousePosX = CameraManager.Cam.ScreenToViewportPoint(Input.mousePosition).x;
            }

            if (Input.GetMouseButton(0))
            {
                float currentMousePosX = CameraManager.Cam.ScreenToViewportPoint(Input.mousePosition).x;
                float distance = currentMousePosX - _mousePosX;
                float posX = _playerVisualPosX + (distance * slideSpeed);
                Vector3 pos = playerModel.localPosition;
                pos.x = Mathf.Clamp(posX, -maxSlideAmount, maxSlideAmount);
                playerModel.localPosition = pos;
            }
            else
            {
                Vector3 pos = playerModel.localPosition;
            }
        }
    }

    #endregion

    #region PlayerTriggerEvents

    private void OnTriggerEnter(Collider other)
    {
        RayfireRigid wall = other.GetComponentInParent<RayfireRigid>();
        if (wall)
        {
            if (!_isWallBreakable)
            {
                PlayerDeath();
                UIManager.Instance.wrongItem.SetActive(true);
            }
            else
            {
                StartCoroutine(PlayerInteractBool());
                Invoke(nameof(DisappearObjects), 1f);
                _isPlayerSelectedDoor = false;
                Destroy(other.gameObject, 6);
            }
        }

        TriggerAttack triggerAttack = other.GetComponentInParent<TriggerAttack>();
        if (triggerAttack)
        {
            if (!_isTriggerAttack && _isWallBreakable)
            {
                StartCoroutine(PlayerInteractBool());
                StartCoroutine(TriggerAttackBool());
                StartCoroutine(AnimationController.Instance.ActivateAttackAnim());
                StartCoroutine(PlayerAttackBreakWall());
            }
        }

        BaseballDoor baseballDoor = other.GetComponentInParent<BaseballDoor>();
        if (baseballDoor)
        {
            if (!_isPlayerSelectedDoor)
            {
                _isWallBreakable = true;
                baseballObject.SetActive(true);
                _isPlayerSelectedDoor = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.pickItemSound, 0.7f);
            }
        }


        AxeDoor axeDoor = other.GetComponentInParent<AxeDoor>();
        if (axeDoor)
        {
            if (!_isPlayerSelectedDoor)
            {
                _isWallBreakable = true;
                axeObject.SetActive(true);
                _isPlayerSelectedDoor = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.pickItemSound, 0.7f);
            }
        }

        PickaxeDoor pickaxeDoor = other.GetComponentInParent<PickaxeDoor>();
        if (pickaxeDoor)
        {
            if (!_isPlayerSelectedDoor)
            {
                _isWallBreakable = true;
                pickaxeObject.SetActive(true);
                _isPlayerSelectedDoor = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.pickItemSound, 0.7f);
            }
        }

        SpearDoor spearDoor = other.GetComponentInParent<SpearDoor>();
        if (spearDoor)
        {
            if (!_isPlayerSelectedDoor)
            {
                _isWallBreakable = true;
                spearObject.SetActive(true);
                _isPlayerSelectedDoor = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.pickItemSound, 0.7f);
            }
        }

        SwordDoor swordDoor = other.GetComponentInParent<SwordDoor>();
        if (swordDoor)
        {
            if (!_isPlayerSelectedDoor)
            {
                _isWallBreakable = true;
                swordObject.SetActive(true);
                _isPlayerSelectedDoor = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.pickItemSound, 0.7f);
            }
        }

        ThorHammerDoor thorHammerDoor = other.GetComponentInParent<ThorHammerDoor>();
        if (thorHammerDoor)
        {
            if (!_isPlayerSelectedDoor)
            {
                _isWallBreakable = true;
                thorHammerObject.SetActive(true);
                _isPlayerSelectedDoor = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.pickItemSound, 0.7f);
            }
        }


        YellowCapDoor yellowCapDoor = other.GetComponentInParent<YellowCapDoor>();
        if (yellowCapDoor)
        {
            if (!_isPlayerSelectedDoor)
            {
                _isWallBreakable = false;
                yellowCapObject.SetActive(true);
                _isPlayerSelectedDoor = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.pickItemSound, 0.7f);
            }
        }

        LifeBuoyDoor lifeBuoyDoor = other.GetComponentInParent<LifeBuoyDoor>();
        if (lifeBuoyDoor)
        {
            if (!_isPlayerSelectedDoor)
            {
                _isWallBreakable = false;
                lifeBuoyObject.SetActive(true);
                _isPlayerSelectedDoor = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.pickItemSound, 0.7f);
            }
        }

        BackpackDoor backpackDoor = other.GetComponentInParent<BackpackDoor>();
        if (backpackDoor)
        {
            if (!_isPlayerSelectedDoor)
            {
                _isWallBreakable = false;
                backpackObject.SetActive(true);
                _isPlayerSelectedDoor = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.pickItemSound, 0.7f);
            }
        }

        HairDryerDoor hairDryerDoor = other.GetComponentInParent<HairDryerDoor>();
        if (hairDryerDoor)
        {
            if (!_isPlayerSelectedDoor)
            {
                _isWallBreakable = false;
                hairDryerObject.SetActive(true);
                _isPlayerSelectedDoor = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.pickItemSound, 0.7f);
            }
        }

        PillowDoor pillowDoor = other.GetComponentInParent<PillowDoor>();
        if (pillowDoor)
        {
            if (!_isPlayerSelectedDoor)
            {
                _isWallBreakable = false;
                pillowObject.SetActive(true);
                _isPlayerSelectedDoor = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.pickItemSound, 0.7f);
            }
        }

        UmbrellaDoor umbrellaDoor = other.GetComponentInParent<UmbrellaDoor>();
        if (umbrellaDoor)
        {
            if (!_isPlayerSelectedDoor)
            {
                _isWallBreakable = false;
                umbrellaObject.SetActive(true);
                _isPlayerSelectedDoor = true;
                SoundManager.Instance.PlaySound(SoundManager.Instance.pickItemSound, 0.7f);
            }
        }

        PlatformOneStar platformOneStar = other.GetComponentInParent<PlatformOneStar>();
        if (platformOneStar)
        {
            if (!_isPlayerSlideJump)
            {
                UIManager.Instance.gold += 1;

                if (UIManager.Instance.energySlider.value < 1)
                {
                    PlayerDeath();
                    UIManager.Instance.energyWasNotEnough.SetActive(true);
                }
                else if (!_isPlayerUsedEnergy)
                {
                    UIManager.Instance.energySlider.value--;
                    UpdateEnergyStars();
                }

                StartCoroutine(PlayerSlideJumpBool());
            }
        }

        PlatformTwoStars platformTwoStars = other.GetComponentInParent<PlatformTwoStars>();
        if (platformTwoStars)
        {
            if (!_isPlayerSlideJump)
            {
                UIManager.Instance.gold += 2;

                if (UIManager.Instance.energySlider.value < 2)
                {
                    PlayerDeath();
                    UIManager.Instance.energyWasNotEnough.SetActive(true);
                }
                else if (!_isPlayerUsedEnergy)
                {
                    UIManager.Instance.energySlider.value -= 2;
                    UpdateEnergyStars();
                }

                StartCoroutine(PlayerSlideJumpBool());
            }
        }

        PlatformThreeStars platformThreeStars = other.GetComponentInParent<PlatformThreeStars>();
        if (platformThreeStars)
        {
            if (!_isPlayerSlideJump)
            {
                UIManager.Instance.gold += 3;

                if (UIManager.Instance.energySlider.value < 3)
                {
                    PlayerDeath();
                    UIManager.Instance.energyWasNotEnough.SetActive(true);
                }
                else if (!_isPlayerUsedEnergy)
                {
                    UIManager.Instance.energySlider.value -= 3;
                    UpdateEnergyStars();
                }

                StartCoroutine(PlayerSlideJumpBool());
            }
        }

        JumpPlatform jumpPlatform = other.GetComponentInParent<JumpPlatform>();
        if (jumpPlatform)
        {
            if (!_isPlayerMoved && !_isPlayerDead)
            {
                StartCoroutine(PlayerInteractBool());
                StartCoroutine(AnimationController.Instance.ActivateJumpAnim());
                StartCoroutine(JumpPosition());
                StartCoroutine(PlayerMovingBool());
                SoundManager.Instance.PlaySound(SoundManager.Instance.jumpSound, 0.4f);
            }
        }

        SlidePlatform slidePlatform = other.GetComponentInParent<SlidePlatform>();
        if (slidePlatform)
        {
            if (!_isPlayerMoved && !_isPlayerDead)
            {
                StartCoroutine(PlayerInteractBool());
                StartCoroutine(AnimationController.Instance.ActivateSlideAnim());
                StartCoroutine(PlayerMovingBool());
                StartCoroutine(PlayerSlidePositionY());
                CameraManager.Instance.isSlideCamera = false;
                SoundManager.Instance.PlaySound(SoundManager.Instance.slideSound, 0.5f);
            }
        }


        CompleteStarBuff completeStarBuff = other.GetComponentInParent<CompleteStarBuff>();
        if (completeStarBuff)
        {
            Instantiate(particleBuffDoor, playerModel.transform.position, Quaternion.identity);
            SoundManager.Instance.PlaySound(SoundManager.Instance.starBuffSound, 0.7f);
            UIManager.Instance.energySlider.value += 5;
            UIManager.Instance.EnergySliderStars();
            Destroy(other.gameObject);
        }

        Collectable collectable = other.GetComponentInParent<Collectable>();
        if (collectable)
        {
            Instantiate(particleCollectable, playerModel.transform.position + new Vector3(0, 2, 0),
                Quaternion.identity);
            SoundManager.Instance.PlaySound(SoundManager.Instance.collectableSound, 0.6f);
            UIManager.Instance.energySlider.value++;
            UIManager.Instance.EnergySliderStars();
            Destroy(other.gameObject);
        }
    }

    #endregion

    #region Methods

    public void PlayerSpeedDown()
    {
        StartCoroutine(FinishGame());
    }

    private void DisappearObjects()
    {
        baseballObject.SetActive(false);
        axeObject.SetActive(false);
        pickaxeObject.SetActive(false);
        spearObject.SetActive(false);
        swordObject.SetActive(false);
        thorHammerObject.SetActive(false);
        yellowCapObject.SetActive(false);
        lifeBuoyObject.SetActive(false);
        backpackObject.SetActive(false);
        hairDryerObject.SetActive(false);
        pillowObject.SetActive(false);
        umbrellaObject.SetActive(false);
    }

    private void PlayerDeath()
    {
        runSpeed = 0;
        GameManager.Instance.GameOver();
        AnimationController.Instance.ActivateDeathAnim();
        StartCoroutine(SoundManager.Instance.GameOverSound());
        GameManager.Instance.CurrentGameState = GameState.GameOver;
        _isPlayerDead = true;
        _isPlayerInteract = true;
    }

    private void UpdateEnergyStars()
    {
        UIManager.Instance.EnergySliderStars();
        StartCoroutine(PlayerUsedEnergyBool());
    }

    private void FiveStarParticle()
    {
        if (UIManager.Instance.energySlider.value >= 5f)
        {
            if (!_isParticleStarTrail)
            {
                _particleStarTrailTemp = Instantiate(particleFiveStars, playerModel.transform.position,
                    new Quaternion(-90, 0, 0, 90));
                _particleStarTrailTemp.transform.SetParent(playerModel);
                _isParticleStarTrail = true;
            }
        }
        else
        {
            Destroy(_particleStarTrailTemp);
            _isParticleStarTrail = false;
        }
    }

    #endregion

    #region Coroutines

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

    private IEnumerator PlayerSlideJumpBool()
    {
        _isPlayerSlideJump = true;
        yield return new WaitForSeconds(1f);
        _isPlayerSlideJump = false;
    }

    private IEnumerator TriggerAttackBool()
    {
        _isTriggerAttack = true;
        yield return new WaitForSeconds(1f);
        _isTriggerAttack = false;
    }

    private IEnumerator PlayerUsedEnergyBool()
    {
        _isPlayerUsedEnergy = true;
        yield return new WaitForSeconds(1f);
        _isPlayerUsedEnergy = false;
    }
    private IEnumerator PlayerInteractBool()
    {
        _isPlayerInteract = true;
        yield return new WaitForSeconds(1f);
        _isPlayerInteract = false;
    }

    private IEnumerator PlayerSlidePositionY()
    {
        yield return new WaitForSeconds(0.20f);
        playerModelChild.transform.localPosition = new Vector3(0, -0.5f, 0);
        yield return new WaitForSeconds(0.8f);
        playerModelChild.transform.localPosition = new Vector3(0, 0, 0);
    }

    private IEnumerator PlayerAttackBreakWall()
    {
        runSpeed = 0;
        playerModelChild.GetComponent<Animator>().applyRootMotion = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        SoundManager.Instance.PlaySound(SoundManager.Instance.beforeAttackSound,1f);

        yield return new WaitForSeconds(1.55f);

        rayfireRigid.Initialize();
        rayfireRigid.Fade();
        runSpeed = 12;
        playerModelChild.GetComponent<Animator>().applyRootMotion = false;
        playerModelChild.transform.rotation = Quaternion.identity;
        UIManager.Instance.gold += 10;
        SoundManager.Instance.PlaySound(SoundManager.Instance.afterAttackSound,1f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.smashWallSound, 1);


        yield return new WaitForSeconds(1.25f);

        playerModelChild.transform.localPosition = new Vector3(0, 0, 0);

        yield return new WaitForSeconds(2f);

        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    #endregion
}