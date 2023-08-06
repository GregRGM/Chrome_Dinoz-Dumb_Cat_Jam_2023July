using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D myRigidbody;
    Collider2D myBodyCollider;
    [SerializeField] bool flipWhenEdge = true;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myBodyCollider = GetComponent<Collider2D>();
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
        FlipEnemyFacing();
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


    void FlipEnemyFacing()
    {
        transform.localScale = new Vector2 (-(Mathf.Sign(myRigidbody.velocity.x)), 1f);
    }
}
