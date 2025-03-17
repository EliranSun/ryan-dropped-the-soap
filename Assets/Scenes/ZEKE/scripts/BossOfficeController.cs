using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialog.Scripts;

public class BossOfficeController : MonoBehaviour
{
    [SerializeField] private GameObject zekeShouts;
    [SerializeField] private GameObject zekeBossStub;

    private void Awake()
    {
        if (zekeShouts) zekeShouts.SetActive(false);
        if (zekeBossStub) zekeBossStub.SetActive(false);
    }

    public void OnNotify(GameEventData gameEventData)
    {
        if (gameEventData.Name == GameEvents.TriggerZekeShout)
        {

            if (zekeShouts) zekeShouts.SetActive(true);
            zekeShouts.GetComponent<ZekeShoutsController>().Init();
        }

        if (gameEventData.Name == GameEvents.TriggerZekeBossStub)
        {
            if (zekeBossStub) zekeBossStub.SetActive(true);
        }
    }
}

