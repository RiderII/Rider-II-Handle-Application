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
    public Button toggleButton;
    public TMP_InputField sensitivity1;
    public TMP_InputField sensitivity2;
    public TMP_InputField tiltPos;
    public string ip = "127.0.0.1";
    Client newClient;
    // Start is called before the first frame update
    void Start()
    {
        toggleButton.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (userId.text != "")   
        {
            toggleButton.interactable = true;
        }
    }

    public void ToggleHandler()
    {
        if (!start)
        {
            HandleControl.s1 = float.Parse(sensitivity1.text);
            HandleControl.s2 = float.Parse(sensitivity2.text);
            HandleControl.tiltPos = float.Parse(tiltPos.text);
            newClient = new Client(ip);
            newClient.sendToUserId = Int32.Parse(userId.text);
            newClient.StartConnetcion();
            start = true;
            sensitivity1.enabled = false;
            sensitivity2.enabled = false;
            tiltPos.enabled = false;
            toggleButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Detener";
        }
        else
        {
            newClient.Disconnect();
            sensitivity1.enabled = true;
            sensitivity2.enabled = true;
            tiltPos.enabled = true;
            start = false;
            toggleButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Iniciar";
        }
    }
}
