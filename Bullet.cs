using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed, lifeTime;
    public Rigidbody theRb;
    public GameObject ImpactEffect;

    public int damage = 20;
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
        if (other.gameObject.tag == "Player" && damagePlayer)
        {
            Instantiate(ImpactEffect, transform.position, transform.rotation);
            // Get the PlayerHealth script attached to the player this bullet hit
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.RPC_DamagePlayer(damage);
                anim.SetTrigger("DamageFront");
            }
        }

        if (other.gameObject.tag == "HeadShot" && damagePlayer)
        {
            Instantiate(ImpactEffect, transform.position, transform.rotation);
            // Get the PlayerHealth script attached to the parent of the headshot hitbox
            PlayerHealth playerHealth = other.transform.parent.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.RPC_DamagePlayer(damage * 2);
                anim.SetTrigger("DamageHS");
            }
        }

        Destroy(gameObject);
    }
}
