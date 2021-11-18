using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject CaptureRect;


    public void Start()
    {
        EventCenter.AddListener(EventDefine.onVideoStartCapture, BoardcastTurnOffUIEvent);
        EventCenter.AddListener(EventDefine.onVideoStopCapture, BoardcastTurnOnUIEvent);

        EventCenter.AddListener(EventDefine.TurnOnUI, turnOnCaptureRect);
        EventCenter.AddListener(EventDefine.TurnOffUI, turnOffCaptureRect);
    }

    private void BoardcastTurnOffUIEvent()
    {
        EventCenter.Broadcast(EventDefine.TurnOffUI);
    }

    private void BoardcastTurnOnUIEvent()
    {
        EventCenter.Broadcast(EventDefine.TurnOnUI);
    }

    private void turnOnCaptureRect()
    {
        CaptureRect.SetActive(false);
    }


    private void turnOffCaptureRect()
    {
        CaptureRect.SetActive(true);
    }


    public void BoardcastStartCaptureEvent() { EventCenter.Broadcast(EventDefine.onVideoStartCapture); }
    public void BoardcastStopCaptureEvent() { EventCenter.Broadcast(EventDefine.onVideoStopCapture); }


}
