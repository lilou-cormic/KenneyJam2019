using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ScoreText = null;

    [SerializeField]
    private TextMeshProUGUI BestText = null;

    private void Start()
    {
        SetScoreTexts();

        ScoreManager.ScoreChanged += ScoreManager_ScoreChanged;
    }

    private void ScoreManager_ScoreChanged()
    {
        SetScoreTexts();
    }

    private void OnDestroy()
    {
        ScoreManager.ScoreChanged -= ScoreManager_ScoreChanged;
    }

    private void SetScoreTexts()
    {
        if (ScoreText == null)
            return;

        ScoreText.text = "Score: " + ScoreManager.Score.ToString("00000");
        BestText.text = "Best: " + ScoreManager.HighScore.ToString("00000");
    }
}
