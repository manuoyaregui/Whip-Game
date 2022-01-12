using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCollisionManager : MonoBehaviour
{
    public static event Action OnPlayerCollidesWithEnemyEvent;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            OnPlayerCollidesWithEnemyEvent?.Invoke();
        }
    }
}
