﻿
using UnityEngine;

public class WeaponSwitching : MonoBehaviour {

    public int selectedWeapon = 0;
    public AudioClip handleSound;
    new AudioSource audio;
    // Use this for initialization
    void Start () {
        audio = GetComponent<AudioSource>();
        SelectWeapon();

	}
	
	// Update is called once per frame
	void Update () {
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon--;
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
                audio.PlayOneShot(handleSound, 1.0F);
            }
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }
}
