using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text coinText;
    public TMP_Text comboText;

    void Update()
    {
        ScoreManager s = ScoreManager.Instance;
        if (s == null) return;

        scoreText.text = "score : " + s.Score;
        coinText.text = "coins : " + s.Coins;
        comboText.text = s.Combo > 0 ? "combo x" + s.Multiplier + " (" + s.Combo + ")" : "";
    }
}