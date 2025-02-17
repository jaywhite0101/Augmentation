// Copyright (c) TigardHighGDC
// SPDX-License SPDX-License-Identifier: Apache-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionWellSelector : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collide)
    {
        if (collide.tag == "Player" && Input.GetKeyDown(KeyCode.F))
        {
            EnemySpawner spawn = GetComponent<EnemySpawner>();
            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();

            if (spawn.Ambush())
            {
                MusicPlayer playerMusic = GameObject.FindGameObjectWithTag("Music Player").GetComponent<MusicPlayer>();
                playerMusic.SuddenTransition("Enemy Room", "Enemy Transition");
            }
        }
    }
}
