using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsMoveWithPlatform : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player" /*|| collision.gameObject.tag == "Grabbable"*/) {
            collision.collider.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Joint" /*|| collision.gameObject.tag == "Grabbable"*/) {
            collision.collider.transform.SetParent(null);
        }
    }

    /*
    public void OnGrapple(GameObject go) {
        if (go.tag == "Player") {
            collision.collider.transform.SetParent(transform);
        }
    }

    public void OnGrappleRemove(GameObject go) {
        if (go.tag == "Player") {
            collision.collider.transform.SetParent(null);
        }
    }
    */
}
