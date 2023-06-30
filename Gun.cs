using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject ammo;

    public float fireRate;

    [HideInInspector]
    public float fireCounter;
    public int currentAmmo;
    public int maxAmmo;

    public float zoomAmount;
    void Start()
    {

    }

    void Update()
    {
        if (fireCounter > 0)
        {
            fireCounter -= Time.deltaTime;
        }
    }
}
