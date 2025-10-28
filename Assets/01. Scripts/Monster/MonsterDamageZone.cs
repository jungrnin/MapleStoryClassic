using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDamageZone : MonoBehaviour
{
    private MonsterBase monster;

    private void Awake()
    {
        monster = GetComponent<MonsterBase>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(1, transform);
            }
        }
    }
}
