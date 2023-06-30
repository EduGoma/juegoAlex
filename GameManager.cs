using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float waitAfterDying = 5f;

    public GameObject player;

    public Transform respawn1, respawn2;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), respawn1.position, respawn1.rotation);
        }
        else
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), respawn2.position, respawn2.rotation);
        }
    }

    public void PlayerDied(GameObject deadPlayer)
    {
        StartCoroutine(PlayerDiedCo(deadPlayer));
    }

    public IEnumerator PlayerDiedCo(GameObject deadPlayer)
    {
        yield return new WaitForSeconds(waitAfterDying);

        if (deadPlayer.GetComponent<PhotonView>().IsMine)
        {
            deadPlayer.transform.position = (PhotonNetwork.IsMasterClient) ? respawn1.position : respawn2.position;
            deadPlayer.transform.rotation = (PhotonNetwork.IsMasterClient) ? respawn1.rotation : respawn2.rotation;

            deadPlayer.GetComponent<PlayerHealth>().Start();
        }
    }
}
