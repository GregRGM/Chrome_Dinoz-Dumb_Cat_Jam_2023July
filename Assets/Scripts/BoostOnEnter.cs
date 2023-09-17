using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostOnEnter : MonoBehaviour
{
    PlayerMovement playerMovement;
    [SerializeField] private float stopBoostTimer = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Triggered");
        if(other.gameObject.tag == "Player")
        {    
            playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            playerMovement.ToggleBoosting(true);
            playerMovement.Boost();
        }
        
    }

    private void OnTriggerExit2D(Collider2D other) {
        Debug.Log("Trigger Exit");
        if(other.gameObject.tag == "Player" && playerMovement != null)
            Invoke("CallStopBoost", stopBoostTimer);
    }

    void CallStopBoost()
    {
        if(playerMovement != null)
            playerMovement.StopBoost();
    }
}
