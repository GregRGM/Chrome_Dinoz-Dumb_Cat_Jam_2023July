using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenWalls : MonoBehaviour
{
    public GameObject wallExplosionObject;
    public GameObject hiddenVisuals;
    public GameObject destroyedVisuals;

    BoxCollider2D myCollider;

    AudioSource mySource;

    private void Awake()
    {
        myCollider = GetComponent<BoxCollider2D>();
        mySource = GetComponent<AudioSource>();
        hiddenVisuals.SetActive(true);
        destroyedVisuals.SetActive(false);
    }

    public void ExplodeWall()
    {
        myCollider.enabled = false;
        wallExplosionObject.SetActive(true);
        mySource.Play();
        StartCoroutine(WallExplosionRoutine());
        FindObjectOfType<PlayerLevelStats>().UpdateSpecialCount(1);
    }

    IEnumerator WallExplosionRoutine()
    {
        yield return new WaitForSeconds(.15f);
        hiddenVisuals.SetActive(false);
        destroyedVisuals.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        wallExplosionObject.SetActive(false);

    }
}
