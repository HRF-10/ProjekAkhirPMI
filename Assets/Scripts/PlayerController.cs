using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Referensi")]
    public Transform model;

    [Header("Gerak")]
    public float forwardSpeed = 10f;
    public float laneDistance = 3f;
    public float laneChangeSpeed = 15f;
    public float maxForwardSpeed = 25f;
    public float speedIncreasePerSecond = 0.1f;

    [Header("Lompat")]
    public float jumpHeight = 2f;
    public float gravity = -25f;

    [Header("Slide")]
    public float slideDuration = 0.8f;

    [Header("Vault")]
    public float vaultCheckDistance = 2f;
    public float vaultClearance = 0.4f;
    public float vaultSpeedBoost = 1.3f;
    public float vaultBoostDuration = 0.5f;

    [Header("Wall Run")]
    public float wallCheckDistance = 2f;
    public float wallRunDuration = 1.5f;

    [Header("Salto")]
    public float flipDuration = 0.5f;

    CharacterController controller;
    int currentLane = 1; // 0 = kiri, 1 = tengah, 2 = kanan
    float verticalVelocity;

    bool isSliding;
    float slideTimer;
    float originalHeight;
    Vector3 originalCenter;

    float boostTimer;

    bool isWallRunning;
    bool canWallRun = true;
    int wallSide; // -1 = dinding kiri, 1 = dinding kanan
    float wallRunTimer;

    bool isFlipping;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        HandleLaneInput();
        HandleVerticalInput();
        HandleSlideTimer();
        if (boostTimer > 0f) boostTimer -= Time.deltaTime;
        forwardSpeed = Mathf.Min(forwardSpeed + speedIncreasePerSecond * Time.deltaTime, maxForwardSpeed);
        Move();
    }

    // ---------- Input ----------

    bool LeftHeld() { return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow); }
    bool RightHeld() { return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow); }
    bool JumpPressed() { return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow); }
    bool SlidePressed() { return Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow); }
    bool FlipPressed() { return Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.LeftShift); }

    void HandleLaneInput()
    {
        if (isWallRunning) return;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            currentLane = Mathf.Max(0, currentLane - 1);

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            currentLane = Mathf.Min(2, currentLane + 1);
    }

    void HandleVerticalInput()
    {
        if (isWallRunning)
        {
            HandleWallRun();
            return;
        }

        if (controller.isGrounded)
        {
            canWallRun = true;
            if (verticalVelocity < 0f) verticalVelocity = -2f;

            if (JumpPressed() && !isSliding)
                JumpOrVault();

            if (SlidePressed() && !isSliding)
                StartSlide();
        }
        else
        {
            if (canWallRun && !isSliding)
            {
                if (LeftHeld() && CheckWall(-1)) StartWallRun(-1);
                else if (RightHeld() && CheckWall(1)) StartWallRun(1);
            }

            if (FlipPressed() && !isFlipping)
                StartCoroutine(Flip());
        }

        if (!isWallRunning)
            verticalVelocity += gravity * Time.deltaTime;
    }

    // ---------- Lompat dan Vault ----------

    Vector3 FeetPosition()
    {
        return transform.position + controller.center - Vector3.up * (controller.height * 0.5f);
    }

    void JumpOrVault()
    {
        AudioManager.PlayJump();
        Vector3 feet = FeetPosition();
        Vector3 origin = feet + Vector3.up * 0.3f;

        if (Physics.Raycast(origin, Vector3.forward, out RaycastHit hit, vaultCheckDistance)
            && hit.collider.CompareTag("Vault"))
        {
            float obstacleHeight = hit.collider.bounds.max.y - feet.y;
            float neededHeight = obstacleHeight + vaultClearance;
            verticalVelocity = Mathf.Sqrt(neededHeight * -2f * gravity);
            boostTimer = vaultBoostDuration;
            ScoreManager.Instance.RegisterAction();
        }
        else
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // ---------- Slide ----------

    void StartSlide()
    {
        isSliding = true;
        AudioManager.PlaySlide();
        slideTimer = slideDuration;
        controller.height = originalHeight * 0.5f;
        controller.center = originalCenter - new Vector3(0f, originalHeight * 0.25f, 0f);

        if (model != null)
        {
            model.localScale = new Vector3(1f, 0.5f, 1f);
            model.localPosition = new Vector3(0f, -originalHeight * 0.25f, 0f);
        }
    }

    void HandleSlideTimer()
    {
        if (!isSliding) return;

        slideTimer -= Time.deltaTime;
        if (slideTimer <= 0f)
        {
            isSliding = false;
            controller.height = originalHeight;
            controller.center = originalCenter;

            if (model != null)
            {
                model.localScale = Vector3.one;
                model.localPosition = Vector3.zero;
            }
        }
    }

    // ---------- Wall Run dan Wall Jump ----------

    bool CheckWall(int dir)
    {
        return Physics.Raycast(transform.position, Vector3.right * dir, out RaycastHit hit, wallCheckDistance)
            && hit.collider.CompareTag("Wall");
    }

    void StartWallRun(int side)
    {
        isWallRunning = true;
        canWallRun = false;
        wallSide = side;
        wallRunTimer = wallRunDuration;
        ScoreManager.Instance.RegisterAction();
        verticalVelocity = 0f;
    }

    void HandleWallRun()
    {
        wallRunTimer -= Time.deltaTime;
        verticalVelocity = 0f;

        if (JumpPressed())
        {
            WallJump();
            return;
        }

        bool holding = wallSide < 0 ? LeftHeld() : RightHeld();
        if (!holding || wallRunTimer <= 0f || !CheckWall(wallSide))
            isWallRunning = false;
    }

    void WallJump()
    {
        AudioManager.PlayJump();
        isWallRunning = false;
        currentLane = 1;
        ScoreManager.Instance.RegisterAction();
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    // ---------- Salto ----------

    IEnumerator Flip()
    {
        isFlipping = true;
        AudioManager.PlayFlip();
        ScoreManager.Instance.RegisterAction();
        float t = 0f;

        while (t < flipDuration)
        {
            t += Time.deltaTime;
            float angle = Mathf.Lerp(0f, 360f, t / flipDuration);
            if (model != null) model.localRotation = Quaternion.Euler(angle, 0f, 0f);
            yield return null;
        }

        if (model != null) model.localRotation = Quaternion.identity;
        isFlipping = false;
    }

    // ---------- Gerak ----------

    void Move()
    {
        float speed = forwardSpeed * (boostTimer > 0f ? vaultSpeedBoost : 1f);

        float targetX = (currentLane - 1) * laneDistance;
        float newX = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);
        float xDelta = newX - transform.position.x;

        Vector3 move = new Vector3(xDelta, verticalVelocity * Time.deltaTime, speed * Time.deltaTime);
        controller.Move(move);
    }
    
    public void PlayFlip() { if (!isFlipping) StartCoroutine(Flip()); }
}