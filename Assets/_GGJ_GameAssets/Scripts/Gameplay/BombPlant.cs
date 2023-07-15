using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPlant : MonoBehaviour
{
    [SerializeField] GameObject bombPrefab;

    [SerializeField] Transform spawnPosition;
    [SerializeField] float respawnCD;

    Bomb currentBomb;
    // Start is called before the first frame update
    void Start()
    {
        SpawnBomb();
    }

    void SpawnBomb()
    {
        if(currentBomb == null)
        {
            currentBomb = Instantiate(bombPrefab, spawnPosition).GetComponent<Bomb>();
            currentBomb.myBody.isKinematic = true;
            LeanTween.scale(currentBomb.gameObject, currentBomb.transform.localScale * 1.15f, .15f).setEasePunch();
            currentBomb.onPickup += OnBombPickup;
        }
    }

    private void OnBombPickup()
    {
        currentBomb.onPickup -= OnBombPickup;
        currentBomb = null;
        StartCoroutine(RespawnBomb());
    }

    IEnumerator RespawnBomb()
    {
        yield return new WaitForSeconds(respawnCD);
        SpawnBomb();
    }
}
