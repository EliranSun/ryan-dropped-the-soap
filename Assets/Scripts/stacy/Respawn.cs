using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] GameObject player;

    public void OnNotify(GameEventData eventData)
    {
        if (eventData.Name == GameEvents.Dead)
        {
            TriggerRespawn();
        }
    }

    private void TriggerRespawn()
    {
        player.transform.position = transform.position;
        player.SetActive(true);
    }
}
