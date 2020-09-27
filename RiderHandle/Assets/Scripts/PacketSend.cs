using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.requestEnteredLobby))
        {
            _packet.Write(Client.instance.sendToUserId); //the server can confirm that the client claimed the correct Id.
            _packet.Write("Handle Middleware");

            SendTCPData(_packet);
        }
    }

    public static void PlayerRotation(Quaternion _rotation)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_rotation);
            _packet.Write(Client.instance.sendToUserId);

            SendUDPData(_packet);
        }
    }
    #endregion
}