using TMPro;
using UnityEngine;

public class Points : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI PointsText = null;

    public void SetPoints(int points)
    {
        PointsText.text = points.ToString();
    }
}
