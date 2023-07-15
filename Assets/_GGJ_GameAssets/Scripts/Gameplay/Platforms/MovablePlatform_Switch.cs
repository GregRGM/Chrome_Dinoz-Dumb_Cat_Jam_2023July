using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class MovablePlatform_Switch : MovablePlatform
{
    [SerializeField] SubTrigger subTrigger;
    [SerializeField] BasicSwitch mySwitch;
    [SerializeField] CinemachineVirtualCamera vCam;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        subTrigger.onTriggerEntered += SubTrigger_onTriggerEntered;
        subTrigger.onTriggerExited += SubTrigger_onTriggerExited;
    }

    private void SubTrigger_onTriggerEntered(Collider2D c)
    {
        if (c.CompareTag("Player"))
        {
            mySwitch.ToggleSwitch(true);
            if (!inMotion)
            {
                OnActivate();
                if (vCam != null)
                {
                    vCam.enabled = true;
                    StartCoroutine(ResetCamAfterTime());
                }
            }
        }
    }

    private void SubTrigger_onTriggerExited(Collider2D c)
    {
        if (c.CompareTag("Player"))
        {
            mySwitch.ToggleSwitch(false);
        }
    }

    public override void OnActivate()
    {
        base.OnActivate();
    }


    IEnumerator ResetCamAfterTime()
    {
        yield return new WaitForSeconds(2.0f);
        vCam.enabled = false;
    }
}
