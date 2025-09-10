using System;
using Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using View.Battle;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 8f;
    public float gravity = 20f;

    [Header("Camera Settings")]
    public Camera playerCamera;
    public float mouseSensitivity = 2f;
    public float verticalLookLimit = 80f;
    //public Vector3 thirdPersonOffset = new Vector3(0f, 2f, -4f);
    //public Vector3 aimOffset = new Vector3(0f, 1.7f, -1.5f);
    public float aimTransitionSpeed = 5f;
    public float cameraFollowSpeed = 10f;
    public float cameraRotationSpeed = 5f;
    public float cameraDistance = 4f;
    public float cameraHeight = 2f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 10f;
    public float maxRotationAngle = 80f;
    public float bodyRotationSpeed = 8f;

    [Header("Bone References")]
    public Transform spineBone;
    public Transform upperChestBone;
    
    [Header("Shooting")]
    public Transform weaponShootPlaceholder;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Animator animator;
    private Vector3 moveDirection = Vector3.zero;
    private Vector2 lookInput = Vector2.zero;
    private Vector2 moveInput = Vector2.zero;
    private float cameraRotationX = 0f;
    private float cameraRotationY = 0f;
    private float currentSpeed;
    private bool isRunning = false;
    private bool isAiming = false;
    private bool isShooting = false;
    private bool isRolling = false;
    private bool isGrounded = false;
    private bool canRoll = true;
    private float rollTimer = 0f;
    private Vector3 rollDirection = Vector3.zero;
    private float currentRotationY = 0f;
    private Vector3 targetCameraPosition;
    private Quaternion targetCameraRotation;
    private Vector3 cameraVelocity = Vector3.zero;
    private float currentCameraDistance;
    private Vector3 rollStartPosition;
    private float rollCurrentDistance = 0f;
    private PlayerData _playerData;
    
    private Func<bool> _startRollCallback;
    private Func<bool> _playerMainShotCallback;
    private Action _bulletDamageCallback;
    private Action<PlayerGunBulletView> _shootCallback;
    private Action<int, EnemyUnitView> _hitBulletCallback;
    private Action<int, int> _hitBulletWeaknessSpotCallback;
    
    private float walkSpeed = 5f;
    private float runSpeed = 8f;
    private float rollDistance = 4f;
    private float rollDuration = 0.6f;

    // Input actions
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction runAction;
    private InputAction fireAction;
    private InputAction aimAction;

    // Animation hashes
    private int speedHash;
    private int isRunningHash;
    private int isAimingHash;
    private int isRollingHash;
    private int isGroundedHash;
    private int jumpHash;
    private int fireHash;
    private int verticalInputHash;
    private int horizontalInputHash;

    // Roll types
    private enum RollType { Forward, Backward, Left, Right, Neutral }
    private RollType currentRollType = RollType.Neutral;

    public void Connect(PlayerData playerData, Func<bool> startRollCallback, Func<bool> playerMainShotCallback, 
        Action<PlayerGunBulletView> shootCallback, Action<int, EnemyUnitView> hitBulletCallback, 
        Action<int, int> hitBulletWeaknessSpotCallback)
    {
        _playerData = playerData;
        _shootCallback = shootCallback;
        _hitBulletCallback = hitBulletCallback;
        _hitBulletWeaknessSpotCallback = hitBulletWeaknessSpotCallback;
        _startRollCallback = startRollCallback;
        _playerMainShotCallback = playerMainShotCallback;
        walkSpeed = playerData.WalkSpeed;
        runSpeed = playerData.RunSpeed;
        rollDistance = playerData.RollDistance;
        rollDuration = playerData.RollDuration;
    }
    
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        // Исправляем поворот персонажа на 0 градусов по X
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        // Setup animation hashes
        speedHash = Animator.StringToHash("Speed");
        isRunningHash = Animator.StringToHash("Sprint");
        isAimingHash = Animator.StringToHash("Aiming");
        isRollingHash = Animator.StringToHash("Roll");
        isGroundedHash = Animator.StringToHash("OnGround");
        jumpHash = Animator.StringToHash("Jump");
        fireHash = Animator.StringToHash("Shoot");
        verticalInputHash = Animator.StringToHash("Y");
        horizontalInputHash = Animator.StringToHash("X");

        // Get input actions
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        runAction = playerInput.actions["Run"];
        fireAction = playerInput.actions["Fire"];
        aimAction = playerInput.actions["Aim"];

        // Initialize camera
        currentCameraDistance = cameraDistance;
        UpdateCameraPosition();
        
        playerCamera = Camera.main;
        //if (playerCamera != null)
        //{
            playerCamera.transform.position = targetCameraPosition;
            playerCamera.transform.LookAt(GetCameraLookTarget());
        //}

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
        // Дополнительная проверка поворота при старте
        EnsureCorrectRotation();
    }

    void Update()
    {
        EnsureCorrectRotation();
        CheckGrounded();
        
        if (isRolling)
        {
            HandleRoll();
            HandleCameraPosition();
            UpdateAnimations();
            return;
        }

        HandleInput();
        HandleMovement();
        HandleCameraRotation();
        HandleCameraPosition();
        HandleAiming();
        HandleShooting();
        HandleCharacterRotation();
        UpdateAnimations();
    }

    private void EnsureCorrectRotation()
    {
        // Всегда поддерживаем правильный поворот персонажа (0 по X)
        Vector3 currentEuler = transform.rotation.eulerAngles;
        if (Mathf.Abs(currentEuler.x) > 0.1f || Mathf.Abs(currentEuler.z) > 0.1f)
        {
            transform.rotation = Quaternion.Euler(0f, currentEuler.y, 0f);
        }
    }

    private void LateUpdate()
    {
        HandleCameraSmoothFollow();
        EnsureCorrectRotation(); // Дополнительная проверка после всех обновлений
    }

    private void CheckGrounded()
    {
        isGrounded = controller.isGrounded;
    }

    private void HandleInput()
    {
        // Get movement input
        moveInput = moveAction.ReadValue<Vector2>();
        
        // Convert 2D input to 3D movement relative to camera
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;
        
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x);
        
        // Get look input
        lookInput = lookAction.ReadValue<Vector2>();

        // Check running
        isRunning = runAction.IsPressed();

        // Check aiming
        isAiming = aimAction.IsPressed();

        // Handle jump/roll
        if (jumpAction.triggered && canRoll)
        {
            //if (/*runAction.IsPressed() ||
            //    */!moveAction.IsPressed())
            //{
            //    Jump();
            //}
            //else
            //{
                StartRoll();
            //}
        }
    }

    private void HandleMovement()
    {
        if (isAiming) return;
        if (isShooting) return;
        
        // Apply speed
        currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        // Apply movement
        if (isGrounded)
        {
            moveDirection.y = -0.5f;
            
            Vector3 horizontalMovement = moveDirection * currentSpeed;
            controller.Move(horizontalMovement * Time.deltaTime);
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }
    }

    private void StartRoll()
    {
        if (isGrounded && !isRolling && canRoll && _startRollCallback.Invoke())
        {
            isRolling = true;
            canRoll = false;
            rollTimer = 0f;
            rollCurrentDistance = 0f;
            rollStartPosition = transform.position;

            // Ensure correct rotation before roll
            EnsureCorrectRotation();

            // Determine roll direction based on input
            DetermineRollDirection();

            // Set roll animation parameters
            animator.SetTrigger(isRollingHash);

            // Calculate roll direction vector
            CalculateRollDirectionVector();
        }
    }

    private void DetermineRollDirection()
    {
        float deadZone = 0.1f;
        
        if (moveInput.y > deadZone)
        {
            currentRollType = RollType.Forward;
        }
        else if (moveInput.y < -deadZone)
        {
            currentRollType = RollType.Backward;
        }
        else if (moveInput.x > deadZone)
        {
            currentRollType = RollType.Right;
        }
        else if (moveInput.x < -deadZone)
        {
            currentRollType = RollType.Left;
        }
        else
        {
            // Default to forward roll if no input
            currentRollType = RollType.Forward;
        }
    }

    private void CalculateRollDirectionVector()
    {
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        switch (currentRollType)
        {
            case RollType.Forward:
                rollDirection = cameraForward;
                break;
            case RollType.Backward:
                rollDirection = -cameraForward;
                break;
            case RollType.Right:
                rollDirection = cameraRight;
                break;
            case RollType.Left:
                rollDirection = -cameraRight;
                break;
            case RollType.Neutral:
                rollDirection = cameraForward;
                break;
        }

        // Ensure roll direction is normalized
        rollDirection.Normalize();

        // Rotate character to face roll direction (only Y rotation)
        if (rollDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rollDirection);
            // Сохраняем только Y поворот
            transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
        }
    }

    private void HandleRoll()
    {
        rollTimer += Time.deltaTime;

        if (rollTimer < rollDuration)
        {
            // Calculate roll progress (0 to 1)
            float rollProgress = rollTimer / rollDuration;
            
            // Ease-out curve for smooth deceleration
            float easeOut = 1f - Mathf.Pow(1f - rollProgress, 3f);
            
            // Calculate current roll distance
            rollCurrentDistance = easeOut * rollDistance;
            
            // Calculate target position
            Vector3 targetPosition = rollStartPosition + rollDirection * rollCurrentDistance;
            
            // Move towards target position
            Vector3 moveVector = targetPosition - transform.position;
            controller.Move(moveVector);

            // Ensure correct rotation during roll
            EnsureCorrectRotation();
        }
        else
        {
            EndRoll();
        }
    }

    private void EndRoll()
    {
        isRolling = false;
        canRoll = true;
        
        // Ensure correct rotation after roll
        EnsureCorrectRotation();
        
        // Reset animation
        animator.SetBool(isRollingHash, false);
    }

    private void HandleCameraRotation()
    {
        if (playerCamera == null) return;

        // Camera rotation around character
        cameraRotationY += lookInput.x * mouseSensitivity * Time.deltaTime;
        cameraRotationX -= lookInput.y * mouseSensitivity * Time.deltaTime;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -verticalLookLimit, verticalLookLimit);
    }

    private void HandleCameraPosition()
    {
        if (playerCamera == null) return;

        float targetDistance = isAiming ? cameraDistance * 0.7f : cameraDistance;
        currentCameraDistance = Mathf.Lerp(currentCameraDistance, targetDistance, aimTransitionSpeed * Time.deltaTime);

        Vector3 cameraDirection = new Vector3(0, 0, -currentCameraDistance);
        Quaternion cameraRotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0);
        Vector3 rotatedOffset = cameraRotation * cameraDirection;

        targetCameraPosition = transform.position + 
                             new Vector3(0, cameraHeight, 0) + 
                             rotatedOffset;

        HandleCameraCollision();

        Vector3 lookTarget = GetCameraLookTarget();
        targetCameraRotation = Quaternion.LookRotation(lookTarget - targetCameraPosition);
    }

    private void HandleCameraCollision()
    {
        Vector3 cameraDirection = (targetCameraPosition - (transform.position + Vector3.up * cameraHeight)).normalized;
        float maxDistance = Vector3.Distance(transform.position + Vector3.up * cameraHeight, targetCameraPosition);

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * cameraHeight, cameraDirection, out hit, maxDistance))
        {
            targetCameraPosition = hit.point - cameraDirection * 0.3f;
        }
    }

    private Vector3 GetCameraLookTarget()
    {
        return transform.position + Vector3.up * (isAiming ? 2f : 1.8f);
    }

    private void HandleCameraSmoothFollow()
    {
        if (playerCamera == null) return;

        playerCamera.transform.position = Vector3.SmoothDamp(
            playerCamera.transform.position,
            targetCameraPosition,
            ref cameraVelocity,
            cameraFollowSpeed * Time.deltaTime
        );

        playerCamera.transform.rotation = Quaternion.Slerp(
            playerCamera.transform.rotation,
            targetCameraRotation,
            cameraRotationSpeed * Time.deltaTime
        );
    }

    private void UpdateCameraPosition()
    {
        Vector3 cameraDirection = new Vector3(0, 0, -currentCameraDistance);
        Quaternion cameraRotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0);
        Vector3 rotatedOffset = cameraRotation * cameraDirection;

        targetCameraPosition = transform.position + 
                             new Vector3(0, cameraHeight, 0) + 
                             rotatedOffset;
    }

    private void HandleCharacterRotation()
    {
        if (isRolling) return;

        // Ensure correct rotation before any rotation logic
        EnsureCorrectRotation();

        if (isAiming || isShooting)
        {
            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            if (cameraForward.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                // Сохраняем только Y поворот
                transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
            }

            float targetUpperBodyRotation = lookInput.x * mouseSensitivity * 2f;
            targetUpperBodyRotation = Mathf.Clamp(targetUpperBodyRotation, -maxRotationAngle, maxRotationAngle);
            
            currentRotationY = Mathf.Lerp(currentRotationY, targetUpperBodyRotation, rotationSpeed * Time.deltaTime);
            
            RotateUpperBody();
        }
        else
        {
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
                // Сохраняем только Y поворот
                transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
            }

            currentRotationY = Mathf.Lerp(currentRotationY, 0f, rotationSpeed * Time.deltaTime);
            
            ResetUpperBodyRotation();
        }
    }

    private void RotateUpperBody()
    {
        if (spineBone != null)
        {
            // Поворачиваем только по Y для костей
            spineBone.localRotation = Quaternion.Euler(0, currentRotationY * 0.7f, 0);
        }
        
        if (upperChestBone != null)
        {
            upperChestBone.localRotation = Quaternion.Euler(0, currentRotationY * 0.3f, 0);
        }
    }

    private void ResetUpperBodyRotation()
    {
        if (spineBone != null)
        {
            spineBone.localRotation = Quaternion.Slerp(spineBone.localRotation, Quaternion.identity, bodyRotationSpeed * Time.deltaTime);
        }
        
        if (upperChestBone != null)
        {
            upperChestBone.localRotation = Quaternion.Slerp(upperChestBone.localRotation, Quaternion.identity, bodyRotationSpeed * Time.deltaTime);
        }
    }

    private void HandleAiming()
    {
        if (playerCamera == null || isRolling) return;

        float targetFOV = isAiming ? 60f : 70f;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, aimTransitionSpeed * Time.deltaTime);
    }

    private void HandleShooting()
    {
        animator.SetBool(fireHash, fireAction.IsPressed());
        if (fireAction.IsPressed() && !isRolling)
        {
            isShooting = true;
            Shoot();
            
            return;
        }
        
        isShooting = false;
    }

    private void Shoot()
    {
        /*Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))*/
        //RaycastHit hit;
        //if (Physics.Raycast(weaponShootPlaceholder.position, Camera.main.transform.forward, out hit, 100f))
        //{
        //Debug.Log("Hit: " + hit.collider.name);
        //Debug.DrawLine(weaponShootPlaceholder.position, hit.point, Color.red, 2.5f);
        //}
        /*if (_playerMainShotCallback.Invoke())
        {
            var handle = Addressables.InstantiateAsync(_playerData.Weapon.BulletView, weaponShootPlaceholder.position, 
                transform.rotation, null);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var bulletViewObject = handle.Result;
                var bulletView = bulletViewObject.GetComponent<PlayerGunBulletView>();
                bulletView.transform.LookAt(playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.7f, 100))); 
                bulletView.Connect(0, _hitBulletCallback);
                _shootCallback?.Invoke(bulletView);
            }
        }*/
        _playerMainShotCallback.Invoke();
    }

    private void Jump()
    {
        if (isGrounded)
        {
            moveDirection.y = jumpForce;
            animator.SetFloat(jumpHash, jumpForce);
            controller.Move(moveDirection * Time.deltaTime);
            // Ensure correct rotation after jump
            EnsureCorrectRotation();
        }
    }

    private void UpdateAnimations()
    {
        float movementMagnitude = new Vector2(moveInput.x, moveInput.y).magnitude;
        float animSpeed = movementMagnitude * (isRunning ? 1.5f : 1f);
        
        animator.SetFloat(speedHash, animSpeed);
        animator.SetFloat(verticalInputHash, moveInput.y);
        animator.SetFloat(horizontalInputHash, moveInput.x);
        animator.SetBool(isRunningHash, isRunning);
        animator.SetBool(isAimingHash, (isAiming || isShooting) && !isRolling);
        animator.SetBool(isRollingHash, isRolling);
        animator.SetBool(isGroundedHash, isGrounded);
    }

    // Public methods
    public bool IsRolling() => isRolling;
    public bool IsAiming() => isAiming;
    public bool IsShooting() => isShooting;
    public bool IsRunning() => isRunning;
    public bool IsGrounded() => isGrounded;
    
    public async void SpawnBullet(int id, string bulletViewAsset)
    {
        var handle = Addressables.InstantiateAsync(bulletViewAsset, weaponShootPlaceholder.position, 
            transform.rotation, null);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var bulletViewObject = handle.Result;
            var bulletView = bulletViewObject.GetComponent<PlayerGunBulletView>();
            bulletView.transform.LookAt(playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.7f, 100))); 
            bulletView.Connect(id, _hitBulletCallback, _hitBulletWeaknessSpotCallback);
            _shootCallback?.Invoke(bulletView);
        }
    }
    
    // Animation event callbacks
    public void OnRollEnd()
    {
        EndRoll();
    }

    public void FootStep()
    {
        // Footstep sounds
    }

    public void RollSound()
    {
        // Roll sounds
    }

    public void CantRotate()
    {
        // CantRotate sounds
    }

    public void OnShoot()
    {
        // Shoot effects
    }

    void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetCameraPosition, 0.2f);
            Gizmos.DrawLine(transform.position + Vector3.up * cameraHeight, targetCameraPosition);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GetCameraLookTarget(), 0.1f);

            // Draw roll direction
            if (isRolling)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + rollDirection * 2f);
                Gizmos.DrawWireSphere(rollStartPosition + rollDirection * rollDistance, 0.3f);
            }
        }
    }
}