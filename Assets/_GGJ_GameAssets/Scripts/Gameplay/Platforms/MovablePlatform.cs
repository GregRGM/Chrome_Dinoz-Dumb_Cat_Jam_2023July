using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
    [SerializeField] LineRenderer lineRend;
    [SerializeField] SpriteRenderer startRend;
    [SerializeField] SpriteRenderer endRend;
    [SerializeField] Sprite triangleSprite;
    [SerializeField] Sprite squareSprite;

    [SerializeField] Color moveColor;
    [SerializeField] Color stopColor;

    public Transform startingPosition;
    public Transform endPosition;

    public GameObject platform;

    [SerializeField] bool retractOnComplete = false;
    internal bool inMotion = false;

    AudioSource myAudioSource;
    float lineLocation;
    [SerializeField] float moveVelocity;
    float destination;
    float distance;

    void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    protected virtual void Start()
    {
        platform.transform.position = startingPosition.position;
        lineLocation = 0;
        destination = lineLocation;
        distance = Vector3.Distance(startingPosition.position, endPosition.position);

        SetupLines();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // Draw line between start and end position.
        var lerpDelta = (moveVelocity / distance);
        if (lineLocation > destination) {
            lineLocation -= lerpDelta * Time.deltaTime;
        }
        else if (lineLocation < destination) {
            lineLocation += lerpDelta * Time.deltaTime;
        }
        platform.transform.position = Vector3.Lerp(startingPosition.position, endPosition.position, lineLocation);

        // Clamp location to beginning and end.
        if (lineLocation <= 0)
        {
            lineLocation = 0;
            platform.transform.position = startingPosition.position;
            inMotion = false;

            startRend.sprite = triangleSprite;
            startRend.color = moveColor;

            if (retractOnComplete)
            {
                endRend.sprite = triangleSprite;
                endRend.color = moveColor;
            }
            else
            {
                endRend.sprite = squareSprite;
                endRend.color = stopColor;
            }
        }
        else if (lineLocation >= 1)
        {
            lineLocation = 1;
            platform.transform.position = endPosition.position;
            inMotion = false;
            if (retractOnComplete)
            {
                OnActivate();
            }
            else
            {
                startRend.sprite = squareSprite;
                startRend.color = stopColor;
                endRend.sprite = triangleSprite;
                endRend.color = moveColor;
            }
        }
        else
            inMotion = true;
    }

    [ContextMenu("Setup Line")]
    public void SetupLines()
    {
        lineRend.SetPosition(0, startingPosition.position);
        lineRend.SetPosition(1, endPosition.position);

        startRend.sprite = triangleSprite;
        startRend.color = moveColor;

        startRend.transform.right = (endRend.transform.position - startRend.transform.position).normalized;
        endRend.transform.right = (startRend.transform.position - endRend.transform.position).normalized;

        if (retractOnComplete)
        {
            endRend.sprite = triangleSprite;
            endRend.color = moveColor;
        }
        else
        {
            endRend.sprite = squareSprite;
            endRend.color = stopColor;
        }
    }

    [ContextMenu("Activate")]
    public virtual void OnActivate()
    {
        myAudioSource.Play();
        if (lineLocation <= 0) {
            destination = 1;
        }
        else if (lineLocation >= 1) {
            destination = 0;
        }
    }
}
