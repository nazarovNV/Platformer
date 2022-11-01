using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicineChest : MonoBehaviour
{
    public int bonusHealth;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<Health>())
        {
            Health health = col.gameObject.GetComponent<Health>();
            health.SetHealth(bonusHealth);
            Destroy(gameObject);
        }
    }
}
