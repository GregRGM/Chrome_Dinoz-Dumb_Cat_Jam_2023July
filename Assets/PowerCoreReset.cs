using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCoreReset : MonoBehaviour
{
    [SerializeField] Collider2D colliderToReset;

    Vector3 resetPos;
    // Start is called before the first frame update
    void Start()
    {
        resetPos = colliderToReset.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision == colliderToReset)
        {
            Debug.Log("TODO: Finish");
        }
    }
}
