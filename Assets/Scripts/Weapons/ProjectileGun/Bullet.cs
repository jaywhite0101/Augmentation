// Copyright (c) TigardHighGDC
// SPDX-License SPDX-License-Identifier: Apache-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Variables changeable by items
    public static float C_Damage = 1.0f;
    public static int C_BulletPierce = 1;
    public static float C_Knockback = 1.0f;

    [HideInInspector]
    public WeaponData Data;

    private Rigidbody2D rb;
    private int remainingPierce;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke("DestroyBullet", Data.DespawnTime);
        remainingPierce = Data.BulletPierce;
    }

    private void OnTriggerEnter2D(Collider2D collide)
    {
        switch (collide.gameObject.tag)
        {
        case "Enemy":
            NonPlayerHealth nonPlayerHealth = collide.gameObject.GetComponent<NonPlayerHealth>();
            collide.GetComponent<Rigidbody2D>().AddForce(
                transform.up * (Data.Knockback * CorruptionLevel.KnockbackIncrease * C_Knockback), ForceMode2D.Impulse);
            nonPlayerHealth.Damage(Data.Damage * C_Damage);

            // Calls current bullet hit modifiers
            ItemHandling.BulletHit?.Invoke();
            remainingPierce--;

            if (remainingPierce * C_BulletPierce <= 0)
            {
                DestroyBullet();
            }

            BounceBullet(collide);

            break;
        case "Wall":
            if (!Data.BulletBounce)
            {
                DestroyBullet();
            }
            else
            {
                remainingPierce--;

                if (remainingPierce * C_BulletPierce <= 0)
                {
                    DestroyBullet();
                }

                BounceBullet(collide);
            }
            break;
        }
    }

    private void BounceBullet(Collider2D collide)
    {
        rb.velocity = Vector3.Reflect(rb.velocity, collide.transform.up);
    }

    // DestroyBullet() is called in the invoke function
    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
