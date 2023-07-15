using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public Color gemColor;
    [SerializeField] SpriteRenderer gemRend;
    [SerializeField] ParticleSystem[] particles;
    [SerializeField] ParticleSystem loopParticles;
    [SerializeField] ParticleSystem collectedParticle;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip pickupClip;
    [SerializeField] float moveDistance;
    [SerializeField] float moveSpeed;
    [SerializeField] LeanTweenType curveType;

    internal bool collected = false;

    Vector3 startPos;
    MaterialPropertyBlock propBlock;
    private void Start()
    {
        startPos = transform.position;
        LeanTween.moveLocalY(gameObject, transform.localPosition.y + moveDistance, moveSpeed).setEase(curveType).setLoopPingPong();
        SetColor();
    }

    [ContextMenu("Set Color")]
    public void SetColor()
    {
        propBlock = new MaterialPropertyBlock();
        gemRend.GetPropertyBlock(propBlock);
        propBlock.SetColor("_BaseColor", gemColor);
        gemRend.color = gemColor;
        gemRend.SetPropertyBlock(propBlock);

        ParticleSystem.MainModule mainMod;
        foreach(ParticleSystem p in particles)
        {
            mainMod = p.main;
            mainMod.startColor = gemColor;
            gemRend.color = gemColor;
        }
    }

    [ContextMenu("Reset Tween")]
    public void ResetTween()
    {
        transform.position = startPos;
        LeanTween.cancel(gameObject);
        LeanTween.moveLocalY(gameObject, transform.localPosition.y + moveDistance, moveSpeed).setEase(curveType).setLoopPingPong();
    }

    [ContextMenu("Pick Up")]
    public void PickUp()
    {
        collected = true;
        loopParticles.Stop();
        audioSource.Stop();
        audioSource.PlayOneShot(pickupClip);
        collectedParticle.Play();
        gemRend.enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PickUp();
        }
    }
}
