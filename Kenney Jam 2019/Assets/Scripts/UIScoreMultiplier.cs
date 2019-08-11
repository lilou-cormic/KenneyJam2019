using TMPro;
using UnityEngine;

public class UIScoreMultiplier : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ScoreMultiplierText = null;

    private void Awake()
    {
        ScoreMultiplierText.text = "× 01";
    }

    private void Start()
    {
        ScoreMultiplierText.text = "× 01";

        ScoreManager.ScoreChanged += ScoreManager_ScoreChanged;
    }

    private void ScoreManager_ScoreChanged()
    {
        SetScoreMultiplierText();
    }

    private void OnDestroy()
    {
        ScoreManager.ScoreChanged -= ScoreManager_ScoreChanged;
    }

    private void SetScoreMultiplierText()
    {
        ScoreMultiplierText.text = $"× {ScoreManager.ScoreMultiplier:00}";
    }
}
