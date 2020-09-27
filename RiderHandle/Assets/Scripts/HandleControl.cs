using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleControl : MonoBehaviour
{
    public static bool startSendingPackets = false;
    private GyroManager gyroInstance;
    // Start is called before the first frame update
    void Start()
    {
        gyroInstance = GyroManager.Instance;
        gyroInstance.EnableGyro();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (gyroInstance.GetGyroActive() && startSendingPackets)
        if (startSendingPackets)
        {
            //Debug.Log($"{Quaternion.Euler(0f, (gyroInstance.GetGyroRotation().y + 0.1f) * 180.5f, 0f)} y value: {gyroInstance.GetGyroRotation().y}");
            PacketSend.PlayerRotation(Quaternion.Euler(0f, (gyroInstance.GetGyroRotation().y + 0.1f) * 180.5f, 0f));
        }
    }
}
