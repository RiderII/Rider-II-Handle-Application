﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;
    public string ip = "127.0.0.1";
    public int port = 26950;
    public int middlewareId = 0;
    public int sendToUserId = 0;
    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    public Client(string ipEntered)
    {
        instance = this;
        ip = ipEntered;
    }

    public void StartConnetcion()
    {
        InitializeClientData();
        tcp = new TCP();
        udp = new UDP();
        isConnected = true;
        tcp.Connect();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public class TCP
    {
        public TcpClient socket;

        public NetworkStream stream;
        public Packet receivedData;
        public byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                return;
            }

            //if we pass we are connected to the server and can start to send and receive data
            Debug.Log("Connected successfully to the server!");
            Debug.Log($"Rotation packets will be sent to user with id: { instance.sendToUserId }");
            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    // Disconnect
                    instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                // handle data
                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null); //continue to reading data from the stream
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error receiving TCP data: {_ex}");
                // Handle disconnect
                Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        if (_packetId == 1) packetHandlers[_packetId](_packet);
                    }
                });

                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        private void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void Connect(int _localPort) //the users port number not the server
        {
            if (endPoint == null)
            {
                endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
            }
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);

            // Initiates the connection with the server and opens up the local port so that the client can receive messages.
            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }

        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.middlewareId);
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to the server via UDP: {_ex}");
            }
        }

        private void Disconnect()
        {
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    }


    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.enterLobby, PacketHandle.Welcome },
            { (int)ServerPackets.startMiddleware, PacketHandle.StartSendingPackets },
        };
        Debug.Log("Initialized packets!");
    }

    public void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;

            if (tcp != null)
            {
                tcp.socket.Close();
                tcp.stream = null;
                tcp.receivedData = null;
                tcp.receiveBuffer = null;
                tcp.socket = null;
            }
            if (udp != null)
            {
                udp.socket.Close();
                udp.endPoint = null;
                udp.socket = null;
            }

            Debug.Log("Disconnected from server.");
        }
    }
}
