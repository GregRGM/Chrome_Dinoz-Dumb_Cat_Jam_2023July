using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D myRigidbody;
    Collider2D myBodyCollider;
    [SerializeField] bool flipWhenEdge = true;

    bool isAlive = true, isBoosting = false;

    public void ToggleBoosting(bool _b)
    {
        isBoosting = _b;
    }
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myBodyCollider = GetComponent<Collider2D>();
    }

    public bool GetIsBoosting()
    {
        return isBoosting;
    }

    void Update()
    {
        myRigidbody.velocity = new Vector2 (moveSpeed, 0f);
        Die();
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (!flipWhenEdge)
        {
            return;
        }
        moveSpeed = -moveSpeed;
        FlipFacing();
    }

    private void OnTriggerEnter2D(Collider other) {
        if (other.gameObject.GetComponent<PlayerMovement>() != null)
        {
            Debug.Log("Player Movement Found");
            if(other.gameObject.GetComponent<PlayerMovement>().GetIsBoosting())
                Destroy(gameObject, 0.1f);
        }
        else if(other.gameObject.GetComponent<Movement>() != null)
        {
            Debug.Log("Enemy Movement Found");

            if(other.gameObject.GetComponent<Movement>().GetIsBoosting())
                Destroy(gameObject, 0.1f);
        }
    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")))
        {
            Destroy(gameObject);
            // isAlive = false;
            // myAnimator.SetTrigger("Dying");
            // myRigidbody.velocity = deathKick;
            // FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }


    void FlipFacing()
    {
        transform.localScale = new Vector2 (-(Mathf.Sign(myRigidbody.velocity.x)), 1f);
    }
}
