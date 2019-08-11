using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = @"Item")]
public class ItemDef : ScriptableObject, IEqualityComparer<ItemDef>
{
    public string Name;

    public string DisplayName;

    public Sprite DisplayImage;

    public override string ToString()
    {
        return Name;
    }

    public override int GetHashCode()
    {
        return Name?.GetHashCode() ?? base.GetHashCode();
    }

    public override bool Equals(object other)
    {
        return Equals(this, other as ItemDef);
    }

    public bool Equals(ItemDef x, ItemDef y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (ReferenceEquals(null, x))
            return false;

        if (ReferenceEquals(null, y))
            return false;

        return x.Name == y.Name;
    }

    public int GetHashCode(ItemDef obj)
    {
        return obj.GetHashCode();
    }
}
