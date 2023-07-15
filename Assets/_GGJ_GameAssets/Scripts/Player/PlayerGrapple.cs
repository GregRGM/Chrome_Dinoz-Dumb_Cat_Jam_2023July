using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Aimer2D))]
public class PlayerGrapple : MonoBehaviour
{
    [SerializeField] DistanceJoint2D myGrappleJoint;
    [SerializeField] Transform grappleHandsTransform;
    [SerializeField] Transform grappleHookTransform;
    [SerializeField] float hookOffsetDistance;
    [SerializeField] float attachDistance = .7f;
    [SerializeField] float minDistance = .2f;
    [SerializeField] float grappleSpeed = 2.5f;
    [SerializeField] float grappleReleaseSpeed = 2.5f;
    [SerializeField] float shotSpeed = .7f;
    [SerializeField] float shotDistanceCheck = .15f;
    [SerializeField] float lineEndOffset = .15f;
    [SerializeField] LayerMask collisionMask;
    [SerializeField] LineRenderer lineRend;
    [SerializeField] LineRenderer preCastLineRend;
    [SerializeField] Animator myAnim;
    [SerializeField] AudioClip shootClip;
    [SerializeField] AudioClip attachClip;
    [SerializeField] AudioClip pullInClip;
    [SerializeField] AudioClip detachClip;
    [SerializeField] AudioClip wooshClip;


    Aimer2D aimer;
    AudioSource audioSource;
    Rigidbody2D myBody;
    CapsuleCollider2D myCollider;
    Vector2 currentHitPos;
    RaycastHit2D hit;
    PlayerMovement movement;
    MovablePlatform movablePlatform;
    bool attached = false;
    bool justShot = false;
    bool hanging = false;

    const float GRAPPLE_CD = .15f;
    float currentTimer;
    bool onCD = false;
    bool canHit = false;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        aimer = GetComponent<Aimer2D>();
        myBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        preCastLineRend.enabled = false;
        myGrappleJoint.transform.parent = null;
        myGrappleJoint.enabled = false;
        lineRend.enabled = false;
        grappleHookTransform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckCD();
    }

    void UpdateHookPosition()
    {
        Vector3 lookPos = (grappleHandsTransform.position - myGrappleJoint.transform.position).normalized;
        grappleHookTransform.up = -lookPos;

        grappleHookTransform.position = myGrappleJoint.transform.position + lookPos * hookOffsetDistance;
        lineRend.SetPosition(1, myGrappleJoint.transform.position + lookPos * lineEndOffset);
    }

    private void LateUpdate()
    {
        UpdateAim();
        if (justShot)
        {
            Vector2 destination = hit.point;
            Vector3 moveDir = Vector3.zero;
            if (movablePlatform != null)
            {
                destination = new Vector2(destination.x, movablePlatform.platform.transform.position.y);
                moveDir = (destination - (Vector2)myGrappleJoint.transform.position).normalized * Time.deltaTime * shotSpeed;
                if (Vector3.Distance(myGrappleJoint.transform.position, destination) < shotDistanceCheck)
                {
                    AttachGrappleToPlatform();
                    moveDir = Vector2.zero;
                }
            }
            else if(hit.collider != null)
            {
                moveDir = (hit.point - (Vector2)myGrappleJoint.transform.position).normalized * Time.deltaTime * shotSpeed;
                if (Vector3.Distance(myGrappleJoint.transform.position, hit.point) < shotDistanceCheck)
                {
                    AttachGrapple(hit.point);
                    moveDir = Vector2.zero;
                }
            }

            myGrappleJoint.transform.position += moveDir;
            lineRend.SetPosition(0, grappleHandsTransform.position);
            lineRend.SetPosition(1, myGrappleJoint.transform.position);
            UpdateHookPosition();
        }

        if (attached)
        {
            if (!hanging)
            {
                if (movablePlatform != null && movablePlatform.inMotion)
                {
                    //Need to determine speed
                    myGrappleJoint.distance -= (Time.deltaTime * grappleSpeed / 10);
                }
                else
                {
                    myGrappleJoint.distance -= (Time.deltaTime * grappleSpeed);
                }
            }
            lineRend.SetPosition(0, grappleHandsTransform.position);
            lineRend.SetPosition(1, myGrappleJoint.transform.position);
            UpdateHookPosition();

            if (myGrappleJoint.distance <= minDistance)
            {
                hanging = true;
                //RemoveGrapple();
                //Or Detach
            }
        }
    }

    void UpdateAim()
    {
        if (!justShot)
        {
            hit = Physics2D.Raycast(transform.position, aimer.aimDirection, attachDistance, collisionMask);
            if (!attached)
            {
                Collider2D colliderHit = hit.collider;
                if (colliderHit != null)
                {
                    if (colliderHit.CompareTag("MovablePlatform"))
                        movablePlatform = colliderHit.GetComponentInParent<MovablePlatform>();
                    else
                        movablePlatform = null;
                }
                else
                    movablePlatform = null;
            }
        }
        if (hit.collider != null)
        {
            if (!attached)
            {
                aimer.aimDistance = hit.distance;
            }

            canHit = !attached & !justShot & !(transform.position.y > hit.point.y) & !onCD;
        }
        else
        {
            canHit = false;
            aimer.aimDistance = attachDistance;
        }

        if (canHit)
        {
            preCastLineRend.enabled = true;
            preCastLineRend.SetPosition(0, hit.point);
            preCastLineRend.SetPosition(1, transform.position);
        }
        else
        {
            preCastLineRend.enabled = false;
        }
    }

    void CheckCD()
    {
        if (onCD)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer <= 0.0f)
            {
                onCD = false;
                currentTimer = GRAPPLE_CD;
            }
        }
    }

    void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (attached & !justShot)
            {
                RemoveGrapple();
            }
            else if (justShot)
            {
                StopShot();
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (hit.collider != null && canHit &! movement.pauseMovement)
            {
                ShootGrapple();
            }
        }

        //if (Input.GetMouseButtonUp(0))
        //{
        //    RemoveGrapple();
        //}
    }

    void StopShot()
    {
        justShot = false;
        lineRend.enabled = false;
        grappleHookTransform.gameObject.SetActive(false);
        myAnim.SetBool("Grapple", false);
    }

    void RemoveGrapple()
    {
        if (attached)
        {
            hanging = false;
            attached = false;
            currentTimer = GRAPPLE_CD;
            onCD = true;
            myGrappleJoint.enabled = false;
            lineRend.enabled = false;
            grappleHookTransform.gameObject.SetActive(false);
            myAnim.SetBool("Grapple", false);
            myBody.AddForce(Vector2.up * grappleReleaseSpeed, ForceMode2D.Impulse);
            aimer.crosshair.gameObject.SetActive(true);
            audioSource.PlayOneShot(detachClip);
            audioSource.PlayOneShot(wooshClip);
            movablePlatform = null;
        }
    }

    void ShootGrapple()
    {
        currentTimer = GRAPPLE_CD;
        onCD = true;
        lineRend.enabled = true;
        grappleHookTransform.gameObject.SetActive(true);
        myGrappleJoint.transform.parent = null;
        myGrappleJoint.transform.position = transform.position;
        justShot = true;
        myAnim.SetBool("Grapple", true);
        audioSource.PlayOneShot(shootClip);
    }

    void AttachGrappleToPlatform()
    {
        myGrappleJoint.transform.parent = movablePlatform.platform.transform;
        AttachGrapple(myGrappleJoint.transform.position);
    }

    void AttachGrapple(Vector3 hitPos)
    {
        attached = true;
        justShot = false;
        myGrappleJoint.enabled = true;
        aimer.crosshair.gameObject.SetActive(false);

        currentHitPos = hitPos;

        myGrappleJoint.enabled = true;
        myGrappleJoint.transform.position = hitPos;

        lineRend.SetPosition(0, grappleHandsTransform.position);
        lineRend.SetPosition(1, hitPos);
        UpdateHookPosition();
        float hitDistance = Vector3.Distance(transform.position, hitPos);
        myGrappleJoint.distance = hitDistance;

        aimer.crosshair.gameObject.SetActive(false);
        //audioSource.PlayOneShot(attachClip);
        audioSource.PlayOneShot(pullInClip);
        CheckHit(hitPos);

        if (movablePlatform != null)
        {
            if(!movablePlatform.inMotion)
                movablePlatform.OnActivate();
        }
    }

    Collider2D CheckHit(Vector3 hitPos) {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, hitPos);
        return hit.collider;
    }
}
