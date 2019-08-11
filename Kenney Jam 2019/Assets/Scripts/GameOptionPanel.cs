using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOptionPanel : MonoBehaviour
{
    [SerializeField]
    private Image SelectorImage = null;

    [SerializeField]
    private TextMeshProUGUI OptionText = null;

    [SerializeField]
    private Color SelectedColor;

    [SerializeField]
    private Color NotSelectedColor;

    [SerializeField]
    private string SceneName;

    [SerializeField]
    private bool IsSelected = false;

    public void SetIsSelected(bool isSelected)
    {
        IsSelected = isSelected;

        if (SelectorImage != null)
            SelectorImage.enabled = IsSelected;

        if (OptionText != null)
            OptionText.color = (IsSelected ? SelectedColor : NotSelectedColor);
    }

    private void OnValidate()
    {
        SetIsSelected(IsSelected);
    }

    private void Update()
    {
        if (!IsSelected)
            return;

        if (Input.GetButtonDown("Submit"))
        {
            if (SceneName == "Quit")
            {
                GameManager.QuitGame();
                return;
            }

            if (SceneName == "Main")
                ScoreManager.ResetScore();

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
        }
    }
}
