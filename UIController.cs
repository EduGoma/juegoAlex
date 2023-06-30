using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public TMP_Text healthText, ammoText;
    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        
    }
}
