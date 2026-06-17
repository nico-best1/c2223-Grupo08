using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DeathComponent : MonoBehaviour
{
    #region references
    [SerializeField]
    private LayerMask layer;

    [SerializeField]
    private string cause;
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1<<collision.gameObject.layer) & layer.value) != 0 && PlayerManager.Instance.IsAlive())
        {
            PlayerManager.Instance.GetComponent<LiveComponent>().Death();


            float2 pos = new float2(PlayerManager.Instance.transform.position.x, PlayerManager.Instance.transform.position.y);
            Tracker.Instance.TrackEvent(new Player_Death("level_" + GameManager.Instance.getLevel(), "room_" + GameManager.Instance.getRoom(), pos, cause));
            Tracker.Instance.Flush();
        }
    }
}