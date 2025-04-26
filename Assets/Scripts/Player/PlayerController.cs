using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{

    private AudioSource footstepAudio;
    [SerializeField]
    private AudioClip walkClip;
    [SerializeField]
    private AudioClip runClip;
    [SerializeField]
    private AudioClip crouchClip;
    [SerializeField]
    private AudioClip sandClip;   // Sand에서의 걷는 소리

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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
            capsuleCollider = GetComponent<CapsuleCollider>();
            myRigid = GetComponent<Rigidbody>();
            theCrosshair = FindObjectOfType<Crosshair>();
            
            isActivated = FindObjectOfType<CraftManual>();

            // ✅ 회전 고정 설정 (충돌 시 회전 방지)
            myRigid.freezeRotation = true;
            myRigid.constraints = RigidbodyConstraints.FreezeRotation;

            // 초기화.
            applySpeed = walkSpeed;
            originPosY = theCamera.transform.localPosition.y;
            applyCrouchPosY = originPosY;

        // ✅ 이어하기 여부에 따라 저장 데이터 적용
        int isContinue = PlayerPrefs.GetInt("IsContinue", 0);
        int slotIndex = PlayerPrefs.GetInt("SelectedSlot", 0);


        if (isContinue == 1)
        {
            SaveData data = SaveManager.instance.LoadFromSlot(slotIndex);
            ApplySaveData(data);
            Debug.Log("📦 ApplySaveData 실행됨!");
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
                Move();
                MoveCheck();
                if (!Inventory.inventoryActivated)
                {
                    CameraRotation();
                    CharacterRotation();
                }



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
            float rayLength = capsuleCollider.bounds.extents.y + 0.2f; // 기존보다 길이를 더 증가

            // 디버깅을 위한 Ray 시각화
            Debug.DrawRay(transform.position, Vector3.down * rayLength, Color.red);

            isGround = Physics.Raycast(transform.position, Vector3.down, out hit, rayLength);
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

            // 앉은 상태에서 점프시 앉은 상태 해제.
            if (isCrouch)
                Crouch();

            theStatusController.DecreaseStamina(100);
            myRigid.velocity = transform.up * jumpForce;
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
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;
        Vector3 _moveDirection = (_moveHorizontal + _moveVertical).normalized;

        if (_moveDirection != Vector3.zero)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
            {
                // 이동 방향을 경사면에 투영 (평면화)
                _moveDirection = Vector3.ProjectOnPlane(_moveDirection, hit.normal).normalized;
            }
        }

        Vector3 _velocity = _moveDirection * applySpeed;
        myRigid.velocity = new Vector3(_velocity.x, myRigid.velocity.y, _velocity.z);
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
            if (!pauseCameraRotation)
            {
                float _xRotation = Input.GetAxisRaw("Mouse Y");
                float _cameraRotationX = _xRotation * lookSensitivity;
                currentCameraRotationX -= _cameraRotationX;
                currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

                theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
            }
        }



        private bool pauseCameraRotation = false;

        //트리 보는 코루틴
        public IEnumerator TreeLookCoroutine(Vector3 _target)
        {
            pauseCameraRotation = true;

            Quaternion direction = Quaternion.LookRotation(_target - theCamera.transform.position);
            Vector3 eulerValue = direction.eulerAngles;
            float destinationX = eulerValue.x;

            while (Mathf.Abs(destinationX - currentCameraRotationX) >= 0.5f)
            {
                eulerValue = Quaternion.Lerp(theCamera.transform.localRotation, direction, 0.3f).eulerAngles;
                theCamera.transform.localRotation = Quaternion.Euler(eulerValue.x, 0f, 0f);
                currentCameraRotationX = theCamera.transform.localEulerAngles.x;
                yield return null;
            }

            pauseCameraRotation = false;
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
        if (!isGround) return;
        if (myRigid.velocity.magnitude <= 0.1f) return; // 안 움직이면 소리 X

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
                else
                {
                    currentSurface = "Other";
                }
            }
        }

        // 움직임에 따라 footstep 딜레이 다르게
        if (movementType == "Walk")
            footstepDelay = 0.5f;
        else if (movementType == "Run")
            footstepDelay = 0.3f;
        else if (movementType == "Crouch")
            footstepDelay = 0.8f;

        if (footstepTimer >= footstepDelay)
        {
            footstepTimer = 0f;

            // 표면에 따라 발소리 클립 선택
            switch (movementType)
            {
                case "Walk":
                    if (currentSurface == "Sand")
                        footstepAudio.clip = sandClip;  // 모래에서는 sandClip
                    else
                        footstepAudio.clip = walkClip;  // 기본적으로는 walkClip
                    break;
                case "Run":
                    if (currentSurface == "Sand")
                        footstepAudio.clip = sandClip;  // 모래에서는 sandClip
                    else
                        footstepAudio.clip = runClip;   // 기본적으로는 runClip
                    break;
                case "Crouch":
                    footstepAudio.clip = crouchClip;  // Crouch는 항상 crouchClip
                    break;
            }

            footstepAudio.Play();
        }
    }
}




