﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Doors : MonoBehaviour
{
    public static bool canOpen = false;
    public AudioClip doorSound;
   //  AudioSource audio;
    new AudioSource audio;
    Animator animator;
    bool doorOpen;

    // Use this for initialization
    void Start()
    {
        doorOpen = false;
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            doorOpen = true;
            DoorControl("Open");
            audio.PlayOneShot(doorSound, 1.0F);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (doorOpen)
        {
            doorOpen = false;
            DoorControl("Close");
            audio.PlayOneShot(doorSound, 1.0F);
        }
    }

    public IEnumerator Open()
    {
        yield return null;
    }

    public IEnumerator Close()
    {
        yield return null;
    }

    void DoorControl(string direction)
    {
        animator.SetTrigger(direction);
    }
}
