// Copyright (c) TigardHighGDC
// SPDX-License SPDX-License-Identifier: Apache-2.0

using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // Variables changeable by items
    public static float C_Size = 1f;
    public static float C_ReloadSpeed = 1f;
    public static float C_BulletSpeed = 1;
    public static float C_CanShootInterval = 1f;
    public static int C_BulletPerTrigger = 1;
    public static int C_AmmoCapacity = 1;
    public static float C_Spread = 1f;
    public static int C_AmmoUsage = 1;

    //  public static float C_Knockback = 1f;

    public WeaponData Data;
    public GameObject Bullet;
    public Camera Camera;
    public Transform SpawnPoint;
    public Transform HandPosition;
    public GameObject WeaponImage;

    [HideInInspector]
    public int AmmoAmount;
    [HideInInspector]
    public bool reloading = false;

    private AmmoCounter ammoCounter;
    private bool shotDelay = false;
    private AudioSource audioPlayer;

    private void Start()
    {
        AmmoAmount = Data.AmmoCapacity * C_AmmoCapacity;
        audioPlayer = gameObject.GetComponent<AudioSource>();
        ammoCounter = GetComponent<AmmoCounter>();
    }

    private void Update()
    {
        RenderWeapon();
        ammoCounter.Text(Data, AmmoAmount);
        Controller();
        PointPlayerToMouse();
    }

    private void Controller()
    {
        if (Input.GetKey(KeyCode.R) && !reloading)
        {
            StartCoroutine((Reload()));
        }

        if (!reloading && !shotDelay && AmmoAmount > 0 && Input.GetButton("Fire1"))
        {
            Fire();
            StartCoroutine(CanShoot());
        }
        else if (Data.AutoReload && !reloading && AmmoAmount <= 0)
        {
            StartCoroutine((Reload()));
        }
    }

    private void Fire()
    {
        // Get player angle relative to mouse
        Vector3 mousePosition = Camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 relativePoint = transform.position - mousePosition;
        float rotation = Mathf.Atan2(relativePoint.y, relativePoint.x) * Mathf.Rad2Deg + 90;

        // Plays sound effect
        if (CorruptionLevel.currentCorruption >= 50.0f)
        {
            // Start 12.5 -> 10.5
            float completion = (CorruptionLevel.currentCorruption - 50.0f) / 50.0f;
            float compression = 12f - (2f * (completion));
            audioPlayer.PlayOneShot(AudioManipulation.BitCrusher(Data.GunShotSound, compression),
                                    Data.GunShotVolume * (3.5f * completion + 3f));
        }
        else
        {
            audioPlayer.PlayOneShot(Data.GunShotSound, Data.GunShotVolume);
        }

        // Spawn bullets
        for (int i = 0; i < Data.BulletPerTrigger * C_BulletPerTrigger; i++)
        {
            Quaternion eulerAngle =
                Quaternion.Euler(0, 0,
                                 rotation + Random.Range(-(Data.Spread * C_Spread * CorruptionLevel.AccuracyDecrease),
                                                         (Data.Spread * C_Spread * CorruptionLevel.AccuracyDecrease)));
            GameObject bullet = Instantiate(Bullet, SpawnPoint.position, eulerAngle);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            bullet.GetComponent<Bullet>().Data = Data;
            bullet.transform.localScale = new Vector3(Data.Size * C_Size, Data.Size * C_Size, 1);
            rb.velocity = bullet.transform.up * Data.BulletSpeed * C_BulletSpeed;
        }
    }

    private void PointPlayerToMouse()
    {
        Vector3 mousePosition = Camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 relativePoint = transform.position - mousePosition;
        transform.localScale =
            new Vector3(Mathf.Sign(relativePoint[0]), transform.localScale[1], transform.localScale[2]);
    }

    private void RenderWeapon()
    {
        WeaponImage.GetComponent<SpriteRenderer>().sprite = Data.Image;
        SpawnPoint.localPosition = new Vector3(Data.WeaponLength, 0, 0);
        HandPosition.localPosition = Data.HandPosition;
    }

    private IEnumerator Reload()
    {
        reloading = true;
        audioPlayer.PlayOneShot(Data.ReloadSound, Data.ReloadVolume);

        // Yield is required to pause the function
        yield return new WaitForSeconds(Data.ReloadSpeed * C_ReloadSpeed);
        AmmoAmount = Data.AmmoCapacity * C_AmmoCapacity;
        reloading = false;
    }

    private IEnumerator CanShoot()
    {
        shotDelay = true;
        AmmoAmount -= C_AmmoUsage;

        // Yield is required to pause the function
        yield return new WaitForSeconds(Data.CanShootInterval * CorruptionLevel.ShootIntervalDecrease *
                                        C_CanShootInterval);
        shotDelay = false;
    }
}
