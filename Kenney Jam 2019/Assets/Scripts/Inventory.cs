using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static Inventory _instance;

    public static Dictionary<ItemDef, int> Items = new Dictionary<ItemDef, int>();

    public static event Action<ItemDef> ItemAdded;

    public static event Action<ItemDef> ItemRemoved;

    public static event Action ItemsCleared;

    private static ItemDef[] _itemDefs;

    private void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _itemDefs = Resources.LoadAll<ItemDef>("Items");

        _instance = this;

        DontDestroyOnLoad(gameObject);

        Clear();
    }

    public static int GetItemNumber(ItemDef itemDef)
    {
        if (Items.TryGetValue(itemDef, out int number))
            return number;

        return 0;
    }

    public static void AddItem(ItemDef itemDef)
    {
        if (itemDef == null)
            return;

        if (Items.ContainsKey(itemDef))
            Items[itemDef]++;
        else
            Items.Add(itemDef, 1);

        ItemAdded?.Invoke(itemDef);
    }

    public static void RemoveItem(ItemDef itemDef)
    {
        if (Items.TryGetValue(itemDef, out int number))
        {
            if (number == 1)
                Items.Remove(itemDef);
            else
                Items[itemDef]--;

            ItemRemoved?.Invoke(itemDef);
        }
    }

    public static void SetItemNumber(string itemName, int number)
    {
        ItemDef itemDef = _itemDefs.FirstOrDefault(x => x.Name.Equals(itemName, StringComparison.InvariantCultureIgnoreCase));

        if (itemDef == null)
            return;

        if (number <= 0)
        {
            RemoveItem(itemDef);
            return;
        }

        Items[itemDef] = number;

        ItemAdded?.Invoke(itemDef);
    }

    public static void Clear()
    {
        Items.Clear();

        ItemsCleared?.Invoke();
    }

    public static ItemDef GetRandomItemDef()
    {
        return _itemDefs[UnityEngine.Random.Range(0, _itemDefs.Length)];
    }

    public static void FromXElement(XElement element)
    {
        if (element == null)
            return;

        Clear();

        var xItemList = element.Elements();

        if (xItemList != null)
        {
            foreach (var xItem in xItemList)
            {
                SetItemNumber(xItem.Name.LocalName, (int)xItem);
            }
        }
    }

    public static XElement ToXElement(string name)
    {
        return new XElement(name,
            Items.Select(x => new XElement(x.Key.Name, x.Value)));
    }
}
