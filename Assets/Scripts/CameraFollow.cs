using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Referensi")]
    public Transform target;
    public PlayerController player;

    [Header("Posisi")]
    public Vector3 offset = new Vector3(0f, 3f, -6f);
    public float lookHeight = 1.5f;
    public float lookAhead = 8f;

    [Header("Kehalusan")]
    [Range(0f, 1f)] public float xFollow = 0.6f;
    public float xSmooth = 6f;
    public float ySmooth = 8f;

    [Header("Efek Kecepatan (FOV)")]
    public float baseFov = 60f;
    public float maxFov = 75f;
    public float fovSmooth = 3f;

    Camera cam;
    float currentX;
    float currentY;
    float startSpeed;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        if (target == null) return;

        currentX = target.position.x * xFollow + offset.x;
        currentY = target.position.y + offset.y;
        cam.fieldOfView = baseFov;

        if (player != null) startSpeed = player.forwardSpeed;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float dt = Time.deltaTime;

        float targetX = target.position.x * xFollow + offset.x;
        float targetY = target.position.y + offset.y;

        currentX = Mathf.Lerp(currentX, targetX, 1f - Mathf.Exp(-xSmooth * dt));
        currentY = Mathf.Lerp(currentY, targetY, 1f - Mathf.Exp(-ySmooth * dt));

        transform.position = new Vector3(currentX, currentY, target.position.z + offset.z);
        transform.LookAt(new Vector3(currentX, target.position.y + lookHeight, target.position.z + lookAhead));

        if (player != null)
        {
            float t = Mathf.InverseLerp(startSpeed, player.maxForwardSpeed, player.forwardSpeed);
            float targetFov = Mathf.Lerp(baseFov, maxFov, t);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, 1f - Mathf.Exp(-fovSmooth * dt));
        }
    }
}