using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;
    private PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void Start()
    {
        currentHealth = maxHealth;
        GetComponent<Animator>().Play("MotionBow");

        if (PV.IsMine)
        {
            UIController.instance.healthText.text = "100";
        }
    }

    public void DamagePlayer(int damageAmount)
    {
        PV.RPC("RPC_DamagePlayer", RpcTarget.All, damageAmount);
    }

    [PunRPC]
    public void RPC_DamagePlayer(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (PV.IsMine)
        {
            UIController.instance.healthText.text = currentHealth.ToString();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        currentHealth = 0;
        GetComponent<Animator>().Play("Death");
        this.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 4);

        if (PV.IsMine)
        {
            GameManager.instance.PlayerDied(this.gameObject);
        }
    }
}