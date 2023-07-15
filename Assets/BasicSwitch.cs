using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSwitch : MonoBehaviour
{
    [SerializeField] SpriteRenderer topRend;
    [SerializeField] SpriteRenderer baseRend;

    [SerializeField] Color baseColor;
    [SerializeField] Color litColor;

    [SerializeField] Sprite pressedSprite;
    [SerializeField] Sprite unPressedSprite;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip pressedClip;
    [SerializeField] AudioClip releasedClip;

    bool toggled = false;

    [ContextMenu("Press Switch")]
    public void ToggleSwitch()
    {
        toggled = !toggled;
        UpdateSwitch();
    }

    public void ToggleSwitch(bool isToggled)
    {
        toggled = isToggled;
        UpdateSwitch();
    }

    void UpdateSwitch()
    {
        if (toggled)
        {
            topRend.sprite = pressedSprite;
            baseRend.material.SetColor("_MaskColor", litColor);
            audioSource.PlayOneShot(pressedClip);
        }
        else
        {
            topRend.sprite = unPressedSprite;
            baseRend.material.SetColor("_MaskColor", baseColor);
            audioSource.PlayOneShot(releasedClip);
        }
    }
}
