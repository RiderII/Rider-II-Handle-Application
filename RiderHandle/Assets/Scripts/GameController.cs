using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class GameController : MonoBehaviour
{
    private bool start = false;
    public TMP_InputField userId;
    public Button button;
    public string ip = "127.0.0.1";
    // Start is called before the first frame update
    void Start()
    {
        button.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (userId.text != "")   
        {
            button.interactable = true;
        }

        button.onClick.AddListener(() =>
        {
            if (!start)
            {
                Client newClient = new Client(ip);
                newClient.sendToUserId = Int32.Parse(userId.text);
                newClient.StartConnetcion();
                start = true;
            }
            
        });
    }
}
