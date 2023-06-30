using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float moveSpeed, lifeTime;
    public Rigidbody theRb;
    public GameObject ImpactEffect;

    public int damage = 50;
    public bool damagePlayer;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        theRb.velocity = transform.forward * moveSpeed;

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null && damagePlayer)
        {
            Instantiate(ImpactEffect, transform.position, transform.rotation);

            if (other.gameObject.tag == "Player")
            {
                playerHealth.RPC_DamagePlayer(damage);
                anim.SetTrigger("DamageFront");
            }
            else if (other.gameObject.tag == "HeadShot")
            {
                playerHealth.RPC_DamagePlayer(damage * 2);
                anim.SetTrigger("DamageHS");
            }

            Destroy(gameObject);
        }
    }
}
