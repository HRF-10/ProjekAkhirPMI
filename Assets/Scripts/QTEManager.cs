using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QTEManager : MonoBehaviour
{
    public static QTEManager Instance { get; private set; }
    public static event Action OnQTESuccess;
    public static event Action OnQTEFailed;

    [Header("UI")]
    public GameObject panel;
    public TMP_Text promptText;
    public Image timerBar;

    [Header("Pengaturan")]
    public KeyCode[] possibleKeys = { KeyCode.Q, KeyCode.E, KeyCode.R };
    public float slowMotionScale = 0.3f;

    bool isRunning;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void StartQTE(QTETrigger trigger, PlayerController player)
    {
        if (isRunning) return;
        StartCoroutine(Run(trigger, player));
    }

    IEnumerator Run(QTETrigger trigger, PlayerController player)
    {
        isRunning = true;

        KeyCode target = possibleKeys[UnityEngine.Random.Range(0, possibleKeys.Length)];
        float limit = trigger.timeLimit;
        float timer = limit;
        bool success = false;
        bool finished = false;

        panel.SetActive(true);
        promptText.text = target.ToString();
        Time.timeScale = slowMotionScale;

        while (timer > 0f && !finished)
        {
            timer -= Time.unscaledDeltaTime;
            timerBar.fillAmount = Mathf.Clamp01(timer / limit);

            foreach (KeyCode k in possibleKeys)
            {
                if (Input.GetKeyDown(k))
                {
                    success = (k == target);
                    finished = true;
                    break;
                }
            }

            yield return null;
        }

        Time.timeScale = 1f;
        panel.SetActive(false);

        trigger.Resolve(success, player);

        if (success)
        {
            Debug.Log("QTE berhasil");
            OnQTESuccess?.Invoke();
        }
        else
        {
            Debug.Log("QTE gagal");
            OnQTEFailed?.Invoke();
        }

        isRunning = false;
    }
}