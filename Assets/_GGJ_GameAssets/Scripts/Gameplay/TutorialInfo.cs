using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInfo : MonoBehaviour
{
    public GameObject tutorialCanvasObject;
    public Guide guide;

    Vector3 startScale;
    private void Awake()
    {
        startScale = tutorialCanvasObject.transform.localScale;
        tutorialCanvasObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tutorialCanvasObject.SetActive(true);
            tutorialCanvasObject.transform.localScale = startScale;
            LeanTween.scale(tutorialCanvasObject, startScale * 1.15f, .2f).setEasePunch();
            guide.transform.position = transform.position;
            guide.PlayAppear();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tutorialCanvasObject.SetActive(false);
            guide.PlayDisappear();
        }
    }
}
