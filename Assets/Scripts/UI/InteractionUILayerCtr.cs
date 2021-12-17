using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionUILayerCtr : MonoBehaviour
{
    [SerializeField]
    GameObject[] ObjectsNeedTurnOffOnCapture;


    public GameObject ImageCanvas;

    public GameObject VideoCanvas;

    public static InteractionUILayerCtr instance;

    public void Start()
    {
        instance = this;

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
        for (int i = 0; i < ObjectsNeedTurnOffOnCapture.Length; i++)
        {
            ObjectsNeedTurnOffOnCapture[i].SetActive(true);
        }
    }


    private void turnOffCaptureRect()
    {
        for (int i = 0; i < ObjectsNeedTurnOffOnCapture.Length; i++)
        {
            ObjectsNeedTurnOffOnCapture[i].SetActive(false);
        }
    }
}
