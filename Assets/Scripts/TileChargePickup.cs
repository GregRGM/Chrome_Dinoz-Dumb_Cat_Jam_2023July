using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChargePickup : MonoBehaviour
{
    [SerializeField] AudioClip pickupSFX;
    [SerializeField] int charges = 10;
    
    bool wasCollected = false;

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player" && !wasCollected)
        {
            wasCollected = true;
            FindObjectOfType<TileSelector2D>().AddToCharges(charges);
            AudioSource.PlayClipAtPoint(pickupSFX, Camera.main.transform.position);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
