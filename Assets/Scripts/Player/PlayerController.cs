using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;


public class PlayerController : MonoBehaviour
{

    KeyCode GetSavedKeyCode(string prefKey, KeyCode defaultKey)
    {
        string saved = PlayerPrefs.GetString(prefKey, defaultKey.ToString());
        if (System.Enum.TryParse<KeyCode>(saved, out KeyCode parsed))
            return parsed;
        else
            return defaultKey;
    }

    //발소리 사운드 클립
    private AudioSource footstepAudio;
    [SerializeField]
    private AudioClip walkClip;
    [SerializeField]
    private AudioClip runClip;
    [SerializeField]
    private AudioClip crouchClip;
    [SerializeField]
    private AudioClip sandClip;   // Sand에서의 걷는 소리
    [SerializeField]
    private AudioClip caveClip;  //동굴에서 걷는 소리

    private float footstepDelay;
    private float footstepTimer = 0f;

    private string currentSurface = "Grass1";  // 현재 표면 (기본값은 Grass)


    static public bool isActivated = true;

        // 스피드 조정 변수
        [SerializeField]
        private float walkSpeed;
        [SerializeField]
        private float runSpeed;
        [SerializeField]
        private float crouchSpeed;


        private float applySpeed;

        [SerializeField]
        private float jumpForce;


        // 상태 변수
        private bool isWalk = false;
        private bool isRun = false;
        private bool isCrouch = false;
        public bool IsCrouch => isCrouch;  //프로퍼티
        private bool isGround = true;


        // 움직임 체크 변수
        private Vector3 lastPos;


        // 앉았을 때 얼마나 앉을지 결정하는 변수.
        [SerializeField]
        private float crouchPosY;
        private float originPosY;
        private float applyCrouchPosY;

        // 땅 착지 여부
        private CapsuleCollider capsuleCollider;


        // 민감도
        [SerializeField]
        private float lookSensitivity;


        // 카메라 한계
        [SerializeField]
        private float cameraRotationLimit;
        private float currentCameraRotationX = 0;


        //필요한 컴포넌트
        [SerializeField]
        private Camera theCamera;
        private Rigidbody myRigid;
        private StatusController theStatusController;
        private Crosshair theCrosshair;
    void Awake()
    {
        

        theStatusController = FindObjectOfType<StatusController>();

        int isContinue = PlayerPrefs.GetInt("IsContinue", -1);
        int slotIndex = PlayerPrefs.GetInt("SelectedSlot", -1);

        Debug.Log($"[DEBUG] isContinue = {isContinue}, slotIndex = {slotIndex}");

        if (isContinue == 1)
        {
            SaveData data = SaveManager.instance.LoadFromSlot(slotIndex);
            ApplySaveData(data);
            GameManager.UpdateCursorState();
            GameManager.canPlayerMove = true;

        }
        else
        {
            Debug.Log("⚠️ 이어하기 아님 → 새 게임 상태임");
        }
    }


    void Start()
        {
            footstepAudio = GetComponent<AudioSource>();
        SoundManager.instance.audioSourceEffects[0] = footstepAudio;
            capsuleCollider = GetComponent<CapsuleCollider>();
            myRigid = GetComponent<Rigidbody>();
            theCrosshair = FindObjectOfType<Crosshair>();
            
            isActivated = FindObjectOfType<CraftManual>();

           
            myRigid.freezeRotation = true;
            myRigid.constraints = RigidbodyConstraints.FreezeRotation;

         
            applySpeed = walkSpeed;
            originPosY = theCamera.transform.localPosition.y;
            applyCrouchPosY = originPosY;

        
        int isContinue = PlayerPrefs.GetInt("IsContinue", 0);
        int slotIndex = PlayerPrefs.GetInt("SelectedSlot", 0);


        if (isContinue == 1)
        {
            SaveData data = SaveManager.instance.LoadFromSlot(slotIndex);
            ApplySaveData(data);
           
        }
        }

    private void ApplySaveData(SaveData data)
    {
     


        // 1. 위치 적용
        transform.position = data.playerPosition;

        // 2. 상태 적용
        theStatusController.SetStatus(data.hp, data.stamina, data.hunger, data.thirst);

        // 3. 무기 적용
        WeaponManager1 weaponManager = GetComponent<WeaponManager1>();
        if (weaponManager != null)
        {
            weaponManager.EquipWeaponByName(data.equippedWeaponName); // 수정된 함수 이름
        }


        // 4. 인벤토리 적용
        Inventory inventory = Inventory.instance;
        if (inventory != null)
        {
            inventory.LoadInventory(data.inventorySlots);
        }

        // 5. 시간 적용
        DayAndNight timeSystem = FindObjectOfType<DayAndNight>();
        if (timeSystem != null)
        {
            timeSystem.SetTime(data.currentTime);
        }

        // 6. 수리 진행도
        WreckShipRepair shipRepair = FindObjectOfType<WreckShipRepair>();
        if (shipRepair != null)
        {
            shipRepair.SetCurrentWood(data.currentWoodCount);

            // 7. 퀵슬롯 복원
            QuickSlotController quickSlot = FindObjectOfType<QuickSlotController>();
            if (quickSlot != null)
            {
                quickSlot.LoadQuickSlots(data.quickSlotDataList);
            }
        }

        //8. 동굴 위치 저장
        CaveRandomizer caveRandomizer = FindObjectOfType<CaveRandomizer>();
        if (caveRandomizer != null)
        {
            for (int i = 0; i < caveRandomizer.caveSpawns.Length; i++)
            {
                caveRandomizer.caveSpawns[i].SetActive(i == data.caveIndex);
            }
        }

        Debug.Log("불러온 저장 데이터 적용 완료!");
    }



    void Update()
        {
 
          PlayFootstepSound(); // 발소리 재생
        if (isActivated && GameManager.canPlayerMove)
            {
                IsGround();
                TryJump();
                TryRun();
                TryCrouch();            
                MoveCheck();
            if (!Inventory.inventoryActivated && GameManager.canPlayerRotate)
            {
                CameraRotation();
                CharacterRotation();
            }
        }
        }

     void FixedUpdate()
    {
        if (isActivated && GameManager.canPlayerMove)
        {
            Move();        
        }
    }

    // 앉기 시도
    private void TryCrouch()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Crouch();
            }
        }


        // 앉기 동작
        private void Crouch()
        {
            isCrouch = !isCrouch;
            theCrosshair.CrouchingAnimation(isCrouch);

            if (isCrouch)
            {
                applySpeed = crouchSpeed;
                applyCrouchPosY = crouchPosY;
            }
            else
            {
                applySpeed = walkSpeed;
                applyCrouchPosY = originPosY;
            }

            StartCoroutine(CrouchCoroutine());

        }


        // 부드러운 동작 실행.
        IEnumerator CrouchCoroutine()
        {

            float _posY = theCamera.transform.localPosition.y;
            int count = 0;

            while (_posY != applyCrouchPosY)
            {
                count++;
                _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
                theCamera.transform.localPosition = new Vector3(0, _posY, 0);
                if (count > 15)
                    break;
                yield return null;
            }
            theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
        }


    // 지면 체크.
    private void IsGround()
    {
        RaycastHit hit;
        float rayLength = capsuleCollider.bounds.extents.y + 0.3f;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

        Debug.DrawRay(rayOrigin, Vector3.down * rayLength, Color.red);
        isGround = Physics.Raycast(rayOrigin, Vector3.down, out hit, rayLength);

        theCrosshair.JumpingAnimation(!isGround);
    }


    // 점프 시도
    private void TryJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGround && theStatusController.GetCurrentSP() > 0)
            {
                Jump();
            }
        }



    // 점프
    private void Jump()
    {
        if (isCrouch)
            Crouch(); // 점프 시 앉기 해제

        theStatusController.DecreaseStamina(100);

        // 기존 속도의 Y 성분 제거
        Vector3 jumpVelocity = myRigid.velocity;
        jumpVelocity.y = 0;
        myRigid.velocity = jumpVelocity;

        // 점프 힘 적용
        myRigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }



    // 달리기 시도
    private void TryRun()
        {
            if (Input.GetKey(KeyCode.LeftShift) && theStatusController.GetCurrentSP() > 0)
            {
                Running();
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) || theStatusController.GetCurrentSP() <= 0)
            {
                RunningCancel();
            }
        }


        // 달리기 실행
        private void Running()
        {
            if (isCrouch)
                Crouch();



            isRun = true;
            theCrosshair.RunningAnimation(isRun);
            theStatusController.DecreaseStamina(10);
            applySpeed = runSpeed;
        }


        // 달리기 취소
        private void RunningCancel()
        {
            isRun = false;
            theCrosshair.RunningAnimation(isRun);
            applySpeed = walkSpeed;
        }


    // 움직임 실행
    void Move()
    {
        KeyCode upKey = GetSavedKeyCode("MoveUp", KeyCode.W);
        KeyCode downKey = GetSavedKeyCode("MoveDown", KeyCode.S);
        KeyCode leftKey = GetSavedKeyCode("MoveLeft", KeyCode.A);
        KeyCode rightKey = GetSavedKeyCode("MoveRight", KeyCode.D);

        float _moveDirX = 0f;
        float _moveDirZ = 0f;

        if (Input.GetKey(leftKey)) _moveDirX = -1f;
        else if (Input.GetKey(rightKey)) _moveDirX = 1f;

        if (Input.GetKey(downKey)) _moveDirZ = -1f;
        else if (Input.GetKey(upKey)) _moveDirZ = 1f;

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;
        Vector3 _moveDirection = (_moveHorizontal + _moveVertical).normalized;

        // 경사면 투영
        if (_moveDirection != Vector3.zero)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
            {
                _moveDirection = Vector3.ProjectOnPlane(_moveDirection, hit.normal).normalized;
            }
        }

        // 실제 이동 적용
        if (isGround)
        {
            Vector3 targetVelocity = _moveDirection * applySpeed;
            Vector3 currentVelocity = myRigid.velocity;
            Vector3 velocityChange = targetVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z);

            velocityChange.y = 0f;
            myRigid.AddForce(velocityChange, ForceMode.VelocityChange);

            if (!Input.GetKey(KeyCode.Space)) // 점프 안할 때만 중력 추가
            {
                myRigid.AddForce(Vector3.down * 250f, ForceMode.Force);
            }
        }
    }



    // 움직임 체크
    private void MoveCheck()
    {
        if (isGround)
        {
            if (Vector3.Distance(lastPos, transform.position) >= 0.01f)
            {
                if (isRun)
                {
                    isWalk = false;
                }
                else if (isCrouch)
                {
                    isWalk = false;
                }
                else
                {
                    isWalk = true;
                }
            }
            else
            {
                isWalk = false;
            }

            theCrosshair.WalkingAnimation(isWalk);
            lastPos = transform.position;
        }
    }


    // 좌우 캐릭터 회전
    private void CharacterRotation()
        {
            float _yRotation = Input.GetAxisRaw("Mouse X");
            Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
            myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        }



        // 상하 카메라 회전
        private void CameraRotation()
        {   
                float _xRotation = Input.GetAxisRaw("Mouse Y");
                float _cameraRotationX = _xRotation * lookSensitivity;
                currentCameraRotationX -= _cameraRotationX;
                currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

                theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
            
        }
      
        // 상태 변수 값 반환
        public bool GetRun()
        {
            return isRun;
        }
        public bool GetWalk()
        {
            return isWalk;
        }
        public bool GetCrouch()
        {
            return isCrouch;
        }
        public bool GetIsGround()
        {
            return isGround;
        }

    private void PlayFootstepSound()
    {
        if (!isGround)
        {
            footstepAudio.Stop();  // 땅에 없으면 발소리 강제 종료
            return;
        }
        Vector3 flatVelocity = new Vector3(myRigid.velocity.x, 0f, myRigid.velocity.z);
        if (flatVelocity.magnitude <= 0.1f)
        {
            footstepAudio.Stop();
            return;
        } // 안움직이면 소리 x
        

        footstepTimer += Time.deltaTime;

        string movementType = "";

        if (isRun)
            movementType = "Run";
        else if (isCrouch)
            movementType = "Crouch";
        else
            movementType = "Walk";

        // 표면 태그에 따라 발소리 다르게
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            if (hit.collider != null)
            {
                // 표면 태그를 검사하여 발소리 변경
                if (hit.collider.CompareTag("Grass"))
                {
                    currentSurface = "Grass";
                }
                else if (hit.collider.CompareTag("Sand"))
                {
                    currentSurface = "Sand";
                }
                else if (hit.collider.CompareTag("Cave"))
                {
                    currentSurface = "Cave";
                }
                else
                {
                    currentSurface = "Other";
                }
            }
        }

        // 움직임에 따라 footstep 딜레이 다르게
        if (movementType == "Walk")
            footstepDelay = 0.6f;
        else if (movementType == "Run")
            footstepDelay = 0.3f;
        else if (movementType == "Crouch")
            footstepDelay = 1f;

        if (footstepTimer >= footstepDelay)
        {
            footstepTimer = 0f;

            // 표면에 따라 발소리 클립 선택
            switch (movementType)
            {
                case "Walk":
                    if (currentSurface == "Sand")
                        footstepAudio.clip = sandClip;
                    else if (currentSurface == "Cave")
                        footstepAudio.clip = caveClip;
                    else
                        footstepAudio.clip = walkClip;
                    break;

                case "Run":
                    if (currentSurface == "Sand")
                        footstepAudio.clip = sandClip;
                    else if (currentSurface == "Cave")
                        footstepAudio.clip = caveClip;
                    else
                        footstepAudio.clip = runClip;
                    break;

                case "Crouch":
                    footstepAudio.clip = crouchClip;
                    break;
            }

            footstepAudio.Play();
        }
    }

    KeyCode GetSavedKey(string keyName, string defaultKey)
    {
        string saved = PlayerPrefs.GetString(keyName, defaultKey);

        try
        {
            KeyCode parsed = (KeyCode)System.Enum.Parse(typeof(KeyCode), saved, true);
            return parsed;
        }
        catch
        {
            Debug.LogWarning($"⚠️ PlayerPrefs의 {keyName} 값 '{saved}'이(가) 잘못되어 기본값 '{defaultKey}'으로 대체됨.");
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), defaultKey);
        }
    }

}




