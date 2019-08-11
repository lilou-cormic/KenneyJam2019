using UnityEngine;

public class Die : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer SpriteRenderer = null;

    [SerializeField]
    private Sprite[] DieSprites = null;

    private int _Value = 0;
    public int Value
    {
        get => _Value;

        set { _Value = value; SetSprite(); }
    }

    private bool _Keep = false;
    public bool Keep
    {
        get => _Keep;

        set { _Keep = value; SetSprite(); }
    }

    public void Roll()
    {
        Value = Random.Range(0, 7);
    }

    public void Clear()
    {
        Value = 0;
    }

    private void SetSprite()
    {
        if (Keep)
            SpriteRenderer.sprite = DieSprites[Value];
        else
            SpriteRenderer.sprite = DieSprites[0];
    }
}
