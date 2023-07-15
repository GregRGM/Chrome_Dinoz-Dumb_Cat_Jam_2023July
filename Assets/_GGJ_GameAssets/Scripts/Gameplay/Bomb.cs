using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bomb : Grabbable
{
    enum ExplosionState {None, FastPulse, Exploded}
    ExplosionState explosionState = ExplosionState.None;
    [SerializeField] float lifeTime = 4.0f;
    [SerializeField] GameObject explosionObject;
    [SerializeField] Color explosiveColor;
    [SerializeField] float exlsionRadius = .15f;
    [SerializeField] AudioClip fastPulseClip;
    [SerializeField] AudioClip explodeClip;
    bool exploded = false;
    bool pickedUp = false;

    public delegate void PickUpCallback();
    public event PickUpCallback onPickup;

    public delegate void BombExplodeCallback(Bomb b);
    public event BombExplodeCallback onExplode;

    public override void PickUp()
    {
        base.PickUp();
        pickedUp = true;
        onPickup?.Invoke();
        LeanTween.cancel(myRend.gameObject);
        LeanTween.color(myRend.gameObject, startColor, 0.0f);
        UpdateExplosive(.5f);
        audioSource.Play();
    }

    private void Update()
    {
        if (pickedUp)
        {
            lifeTime -= Time.deltaTime;

            if(lifeTime <= 2.0f && explosionState != ExplosionState.FastPulse)
            {
                explosionState = ExplosionState.FastPulse;
                UpdateExplosive(.1f);
            }

            if (lifeTime <= 0.0f & !exploded)
            {
                Explode();
            }
        }
    }

    void UpdateExplosive(float flickerTime)
    {
        if(flickerTime == .1f)
        {
            audioSource.clip = fastPulseClip;
            audioSource.Play();
        }
        LeanTween.cancel(myRend.gameObject);
        LeanTween.color(myRend.gameObject, startColor, 0.0f);
        LeanTween.color(myRend.gameObject, explosiveColor, flickerTime).setLoopPingPong().setEaseLinear();
    }

    void Explode()
    {
        explosionObject.transform.parent = null;
        explosionObject.transform.rotation = Quaternion.identity;
        exploded = true;
        explosionObject.SetActive(true);
        myCollider.enabled = false;
        myBody.bodyType = RigidbodyType2D.Kinematic;
        myBody.velocity = Vector2.zero;
        myBody.angularVelocity = 0.0f;
        StartCoroutine(RemoveAfterTime());
        audioSource.Stop();
        audioSource.PlayOneShot(explodeClip);
        onExplode?.Invoke(this);
    }

    IEnumerator RemoveAfterTime()
    {
        yield return new WaitForSeconds(.2f);
        myRend.enabled = false;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(myBody.position, exlsionRadius);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag("FakeWall"))
            {
                hitColliders[i].GetComponent<HiddenWalls>().ExplodeWall();
            }
        }
        yield return new WaitForSeconds(1.0f);
        Destroy(explosionObject);
        Destroy(gameObject);
    }
}
