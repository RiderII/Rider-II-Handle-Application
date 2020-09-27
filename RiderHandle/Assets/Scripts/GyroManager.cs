using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroManager : MonoBehaviour
{
    #region Instance
    public static GyroManager instance;
    public static GyroManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GyroManager>();
                if (instance == null)
                {
                    instance = new GameObject("Spawned GyroManager", typeof(GyroManager)).GetComponent<GyroManager>();
                }
            }

            return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    [Header("Logic")]
    private Gyroscope gyro;
    private Quaternion rotation;
    private bool gyroActive;

    [Header("Tweaks")]
    [SerializeField] private Quaternion baseRotation = new Quaternion(0, 0, 1, 0);

    public void EnableGyro()
    {
        if (gyroActive)
            return;

        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            gyroActive = gyro.enabled;
            Debug.Log("Your phone has gyroscope!.");
        }
        else
        {
            Debug.Log("Your phone doesn't have gyroscope!.");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (gyroActive)
        {
            rotation = gyro.attitude;
        }
    }

    public Quaternion GetGyroRotation()
    {
        return rotation * baseRotation;
    }

    public bool GetGyroActive()
    {
        return gyroActive;
    }
}
