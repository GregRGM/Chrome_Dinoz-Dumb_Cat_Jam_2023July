using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Altar : MonoBehaviour
{
    [SerializeField] GameObject messageDisplay;
    [SerializeField] Color poweredDownColor;
    [SerializeField] Color altarFlashColor;
    [SerializeField] Color altarPowerOnColor;
    [SerializeField] Collider2D colliderNeeded;
    [SerializeField] TMP_Text myText;
    [SerializeField] LineRenderer powerLineRend;

    [SerializeField] Transform powerSpot;

    [SerializeField] float flashTime = .5f;
    [SerializeField] int flashLoopCount = 1;

    [SerializeField] float moveToSpotTime = .5f;
    [SerializeField] int powerOnTime = 1;
    [SerializeField] AudioClip noPowerClip;
    [SerializeField] AudioClip powerOnClip;

    internal bool poweredUp = false;
    Vector3 startScale;
    AudioSource audioSource;

    public delegate void PoweredUpCallack();
    public event PoweredUpCallack onPoweredUp;



    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        startScale = messageDisplay.transform.localScale;
        messageDisplay.SetActive(false);
        LeanTween.color(gameObject, poweredDownColor, 0.0f);

        powerLineRend.startColor = poweredDownColor; 
        powerLineRend.endColor = poweredDownColor;

        myText.text = "Required For Power";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            messageDisplay.SetActive(true);
            LeanTween.scale(messageDisplay, startScale * 1.05f, .25f).setEasePunch();
            if (!poweredUp)
            {
                PlayerGrabber grabber = collision.GetComponent<PlayerGrabber>();
                if (grabber.attachedGrabbable != null && grabber.holding && grabber.attachedGrabbable.myCollider == colliderNeeded)
                {
                    grabber.DropAttachedGrabbable();
                    PowerOnAltar();
                    
                }
                else
                {
                    audioSource.PlayOneShot(noPowerClip);
                    LeanTween.color(gameObject, poweredDownColor, 0.0f);
                    LeanTween.color(gameObject, altarFlashColor, flashTime).setEaseSpring().setLoopPingPong(flashLoopCount);
                }
            }
        }
        else if (collision.CompareTag("Grabbable"))
        {
            Grabbable grabbable = collision.GetComponent<Grabbable>();
            if(grabbable.myCollider == colliderNeeded)
            {
                PowerOnAltar();
            }
        }
    }

    void PowerOnAltar()
    {
        StartCoroutine(PowerOnRoutine());
        myText.text = "<color=green>Power Restored</color>";
    }

    IEnumerator PowerOnRoutine()
    {
        poweredUp = true;
        LeanTween.color(gameObject, poweredDownColor, 0.0f);
        LeanTween.color(gameObject, altarPowerOnColor, .1f).setEasePunch();
        colliderNeeded.enabled = false;
        audioSource.PlayOneShot(powerOnClip);
        yield return new WaitForSeconds(.1f);
        Rigidbody2D colliderBody = colliderNeeded.GetComponent<Rigidbody2D>();
        colliderBody.bodyType = RigidbodyType2D.Kinematic;
        colliderBody.velocity = Vector2.zero;
        colliderBody.angularVelocity = 30.0f;
        LeanTween.move(colliderNeeded.gameObject, powerSpot.position, moveToSpotTime).setEaseInCubic();
        yield return new WaitForSeconds(moveToSpotTime);
        colliderBody.angularVelocity = 0.0f;
        LeanTween.color(gameObject, altarPowerOnColor, powerOnTime).setEaseOutCubic();
        LeanTween.color(powerLineRend.gameObject, altarPowerOnColor, powerOnTime).setEaseOutCubic();
        for (float i = 0; i < powerOnTime; i += Time.deltaTime)
        {
            powerLineRend.startColor = Color.Lerp(poweredDownColor, altarPowerOnColor, i / powerOnTime);
            powerLineRend.endColor = Color.Lerp(poweredDownColor, altarPowerOnColor, i / powerOnTime);
        }

        onPoweredUp?.Invoke();
        yield return new WaitForSeconds(powerOnTime);
        audioSource.Play();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            messageDisplay.SetActive(false);
        }
    }
}
