using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField]
    private AudioClip PickupSound = null;

    private bool _isTaken = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isTaken)
            return;

        if (!CanPickup(collision))
            return;

        _isTaken = true;

        SoundPlayer.Play(PickupSound);

        OnPickup(collision);

        Destroy(gameObject);
    }

    protected virtual bool CanPickup(Collider2D collision)
    {
        return true;
    }

    protected abstract void OnPickup(Collider2D collision);
}
