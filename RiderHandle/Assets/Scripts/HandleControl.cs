using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleControl : MonoBehaviour
{
    public static bool startSendingPackets = false;
    private GyroManager gyroInstance;
    public static float s1 = 0.1f;
    public static float s2 = 180.5f;
    public static float tiltPos = 90f;
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
            Debug.Log($"{Quaternion.Euler(0f, ((gyroInstance.GetGyroRotation().y + s1) * s2) + tiltPos, 0f)} y value: {gyroInstance.GetGyroRotation().y}");
            tiltPos += ((gyroInstance.GetGyroRotation().y + s1) * s2) / 10;
            PacketSend.PlayerRotation(Quaternion.Euler(0f, ((gyroInstance.GetGyroRotation().y + s1) * s2) + tiltPos, 0f));
        }
    }
}
