using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Referensi")]
    public Transform player;

    [Header("Poin")]
    public float pointsPerMeter = 1f;
    public int coinPoints = 10;
    public int actionPoints = 50;

    [Header("Combo")]
    public float comboTimeout = 4f;
    public int actionsPerLevel = 2;
    public int maxMultiplier = 5;

    public int Score { get { return Mathf.FloorToInt(distanceScore + bonusScore); } }
    public int Coins { get; private set; }
    public int Combo { get; private set; }
    public int Multiplier { get { return Mathf.Min(1 + Combo / actionsPerLevel, maxMultiplier); } }

    float distanceScore;
    float bonusScore;
    float lastZ;
    float comboTimer;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        lastZ = player.position.z;
    }

    void OnEnable()
    {
        QTEManager.OnQTESuccess += RegisterAction;
        LivesManager.OnLivesChanged += OnLivesChanged;
    }

    void OnDisable()
    {
        QTEManager.OnQTESuccess -= RegisterAction;
        LivesManager.OnLivesChanged -= OnLivesChanged;
    }

    void Update()
    {
        float z = player.position.z;
        if (z > lastZ) distanceScore += (z - lastZ) * pointsPerMeter;
        lastZ = z;

        if (Combo > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f) Combo = 0;
        }
    }

    public void RegisterAction()
    {
        Combo++;
        comboTimer = comboTimeout;
        bonusScore += actionPoints * Multiplier;
    }

    public void AddCoin()
    {
        Coins++;
        bonusScore += coinPoints * Multiplier;
    }

    void OnLivesChanged(int lives)
    {
        Combo = 0;
    }
}