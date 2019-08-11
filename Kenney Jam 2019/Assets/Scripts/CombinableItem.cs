using UnityEngine;

public class CombinableItem : Item
{
    [SerializeField]
    private ItemDef _ItemDef = null;
    public ItemDef ItemDef { get => _ItemDef; set { _ItemDef = value; SetForItemDef(); } }

    [SerializeField]
    private Points PointsPrefab = null;

        private void OnValidate()
    {
        SetForItemDef();
    }

    private void SetForItemDef()
    {
        if (ItemDef != null)
        {
            name = "Item_" + ItemDef.Name;
            GetComponent<SpriteRenderer>().sprite = ItemDef.DisplayImage;
        }
    }

    protected override void OnPickup(Collider2D collision)
    {
        Points points = Instantiate(PointsPrefab, transform.position, Quaternion.identity);
        points.SetPoints(ScoreManager.AddPoints(10));
        Destroy(points.gameObject, 1f);

        Inventory.AddItem(ItemDef);
        GameManager.SaveInventory();
    }
}
