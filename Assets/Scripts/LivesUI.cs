using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class LivesUI : MonoBehaviour
{
    TMP_Text livesText;

    void Awake()
    {
        livesText = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        LivesManager.OnLivesChanged += UpdateText;
    }

    void OnDisable()
    {
        LivesManager.OnLivesChanged -= UpdateText;
    }

    void UpdateText(int lives)
    {
        livesText.text = "lives : " + lives;
    }
}