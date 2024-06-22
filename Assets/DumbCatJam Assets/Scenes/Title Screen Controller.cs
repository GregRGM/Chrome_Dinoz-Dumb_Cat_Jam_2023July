using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CriticalTrialsTitleScreen : MonoBehaviour {

    public bool canProceed = false;
    public string sceneToOpen;
    public GameObject logoSet, titleSet/*, controllerSet*/;
	
    IEnumerator TitleSequence()
    {
        if(logoSet != null)
            logoSet.SetActive(true);
        else
        titleSet.SetActive(false);
        yield return new WaitForSeconds(2.0f);
        logoSet.SetActive(false);
        //yield return new WaitForSeconds(2.0f);
        titleSet.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        titleSet.SetActive(false);
        SceneManager.LoadScene(sceneToOpen);
        yield return null;

    }
    // Use this for initialization
	void Start () {
        //StartCoroutine("TitleSequence");
        SceneManager.LoadScene(sceneToOpen);
	}
	
	// Update is called once per frame
	void Update () {
		if(canProceed)
        {
            if(Input.GetButton("Submit") || Input.anyKey)
            {
                SceneManager.LoadScene(sceneToOpen);
            }
        }
	}
}
