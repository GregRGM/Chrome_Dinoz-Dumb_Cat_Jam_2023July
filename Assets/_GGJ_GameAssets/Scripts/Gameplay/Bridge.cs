using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Bridge : MonoBehaviour
{
    [SerializeField] AltarHelper[] altarHelpers;
    public CinemachineVirtualCamera vCam;
    // Start is called before the first frame update
    void Start()
    {
        vCam.enabled = false;
        for (int i = 0; i < altarHelpers.Length; i++)
        {
            altarHelpers[i].myAltar.onPoweredUp += MyAltar_onPoweredUp;
        }
    }

    private void MyAltar_onPoweredUp()
    {
        if (AllPoweredUp())
        {
            StartCoroutine(PlayOpenRoutine());
        }
    }
    
    bool AllPoweredUp()
    {
        for (int i = 0; i < altarHelpers.Length; i++)
        {
            if (!altarHelpers[i].myAltar.poweredUp)
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator PlayOpenRoutine()
    {
        vCam.enabled = true;
        yield return new WaitForSeconds(.25f);

        float initialShakeTime = 1.5f;
        for (float i = 0; i < initialShakeTime; i += Time.deltaTime)
        {
            float shakePerc = Mathf.Lerp(.01f, .1f, i / initialShakeTime);
            yield return null;
        }
        for (int i = 0; i < altarHelpers.Length; i++)
        {
            LeanTween.rotate(altarHelpers[i].bridgeTransform.gameObject, new Vector3(0, 0, altarHelpers[i].rotationAmount), altarHelpers[i].rotationSpeed).setEaseOutBounce();
        }
        yield return new WaitForSeconds(2.0f);
        vCam.enabled = false;
    }
}

[System.Serializable]
public struct AltarHelper
{
    public Altar myAltar;
    public Transform bridgeTransform;
    public float rotationAmount;
    public float rotationSpeed;
}