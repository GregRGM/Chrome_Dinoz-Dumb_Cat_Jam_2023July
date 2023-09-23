using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;

    //Since this uses Dynamic Rigidbody2D, it relies on a high negative value to simulate gravity and a low positive value to simulate jumping. Higher negative values will make the player fall faster AND allow the player to jump higher at positive values
    [SerializeField] Vector2 maxVertSpeed = new Vector2 (-20f, 10f);
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2 (10f, 10f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    [SerializeField] bool canDie = true;
    
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart;

    bool isAlive = true, isAirborne = false, isBoosting = false;

    public void ToggleBoosting(bool _b)
    {
        isBoosting = _b;
    }

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    public bool GetIsAlive()
    {
        return isAlive;
    }

    public bool GetIsAirborne()
    {
        return isAirborne;
    }
public bool GetIsBoosting()
    {
        return isBoosting;
    }
    void FixedUpdate()
    {
        if (!isAlive) { return; }
        if(myRigidbody.velocity.y != 0)
        {    
            isAirborne = true;
            ClampVerticalVelocity();
        }
        else
        {
            isAirborne = false;
        }
        Run();
        if(isBoosting)
            Boost();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnFire(InputValue value)
    {
        // if (!isAlive) { return; }
        // Instantiate(bullet, gun.position, transform.rotation);
    }
    
    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void ClampVerticalVelocity()
    {
        Vector2 velocity = myRigidbody.velocity;
        velocity.y = Mathf.Clamp(velocity.y, maxVertSpeed.x, maxVertSpeed.y);
        myRigidbody.velocity = velocity;
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return;}
        
        if(value.isPressed)
        {
            // do stuff
            myRigidbody.velocity += new Vector2 (0f, jumpSpeed);
        }
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2 (moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;        
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
        // myRigidbody.velocity.y = 
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) 
        { 
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            return;
        }
        
        Vector2 climbVelocity = new Vector2 (myRigidbody.velocity.x, moveInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards"))  )
        {
            if((isBoosting && myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies"))) || !canDie)
            {
                return;
            }

            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    public void Boost()
    {
        //isBoosting = true;
        GetComponent<Rigidbody2D>().gravityScale = 0f;
        myAnimator.SetBool("isBoosting", true);
        Vector2 playerVelocity = new Vector2 (moveInput.x * runSpeed * 5f, 0);
        myRigidbody.velocity = playerVelocity;    
        //GetComponent<Rigidbody2D>().velocity += new Vector2 ((jumpSpeed * 2), 0);
    }

    public void StopBoost()
    {
        isBoosting = false;
        // myAnimator.SetBool("isBoosting", false);
        GetComponent<Rigidbody2D>().gravityScale = gravityScaleAtStart;
    }    
}
