using System.Collections;
using System.Collections.Generic;
using Ludiq.Peek;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D myRigidbody;
    Collider2D myBodyCollider;
    [SerializeField] bool flipWhenEdge = true;
    GameObject collidedObject = null;

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
        Boost();
        if(!isBoosting)
            myRigidbody.velocity = new Vector2 (moveSpeed, 0f);
        
        Die();
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (!flipWhenEdge || 
            other.gameObject.layer == LayerMask.GetMask("Effector") ||
            isBoosting)
        {
            return;
        }
        
        moveSpeed = -moveSpeed;
        FlipFacing();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        collidedObject = other.gameObject;
        if (collidedObject.GetComponent<PlayerMovement>() != null)
        {
            Debug.Log("Player Movement Found");
            if(other.gameObject.GetComponent<PlayerMovement>().GetIsBoosting())
                Destroy(gameObject);
        }
        else if(collidedObject.GetComponent<EnemyMovement>() != null)
        {
            Debug.Log("Enemy Movement Found");

            if(collidedObject.GetComponent<EnemyMovement>().GetIsBoosting())
                Destroy(gameObject);
        }
    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")) || 
            myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            isAlive = false;
            myRigidbody.gravityScale = 0f;
            myRigidbody.velocity = new Vector2 (0f, 0f);
            GetComponent<SpriteRenderer>().enabled = false;
            myBodyCollider.enabled = false;
            GetComponent<EnemyMovement>().enabled = false;
            
            //Destroy(gameObject);
            // myAnimator.SetTrigger("Dying");
            // myRigidbody.velocity = deathKick;
            // FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
    public void Boost()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Effector")) )
        {
            isBoosting = true;
            GetComponent<Rigidbody2D>().gravityScale = 0f;
            Vector2 playerVelocity = new Vector2 (moveSpeed * 5f, 0);
            myRigidbody.velocity = playerVelocity;    
        }
    }

    void FlipFacing()
    {
        transform.localScale = new Vector2 (-(Mathf.Sign(myRigidbody.velocity.x)), 1f);
    }
}
