using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : GrabPlatform
{
    public override void OnActivate()
    {
        DestroyObject();
    }

    // Code for destruction
    // Possible todo: animate this
    public void DestroyObject() {
        Destroy(this.gameObject);
    }
}
