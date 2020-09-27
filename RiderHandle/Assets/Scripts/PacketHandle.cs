using System.Collections;
using System.Net;
using System.Collections.Generic;
using UnityEngine;

public class PacketHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet) //read the value of the packets send from the server in the same order they were send
    {
        string _msg = _packet.ReadString();
        int _id = _packet.ReadInt();

        Client.instance.middlewareId = _id;

        // Send packet back to the server
        PacketSend.WelcomeReceived();

        Debug.Log($"Message from server: {_msg}. RiderII handle application connected with id: {Client.instance.middlewareId}");

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);

        HandleControl.startSendingPackets = true;
    }

    public static void StartSendingPackets(Packet _packet)
    {
        int _id = _packet.ReadInt();
        HandleControl.startSendingPackets = true;
    }
}
