using System.IO;
using System.Net;
using System;
using System.Threading;
using N = System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            List<TcpClient> connectedClients = new List<TcpClient>();
            TcpListener chatServer = new TcpListener(IPAddress.Any, 5000);
            chatServer.Start();

            while (true)
            {
                TcpClient chatConnectionClient = chatServer.AcceptTcpClient();
                connectedClients.Add(chatConnectionClient);



                Thread clientThread = new Thread(() => HandleClientComm(chatConnectionClient));
                clientThread.Start();
                

                Console.WriteLine("Client Connected!!");
            }

            void HandleClientComm(TcpClient client)
            {

                while (true)
                {
                    NetworkStream ns = client.GetStream();
                    string str = String.Empty;
                    byte[] Msg = new byte[255];

                    try
                    {                       
                        ProtocolWriteMessage(ns, out str);

                        Console.WriteLine(str);
                        Msg = Encoding.Default.GetBytes(str);

                        Broadcast(connectedClients, Msg);
                    }
                    catch (IOException ex)
                    {
                        for (int i = 0; i < connectedClients.Count; i++)
                        {
                            if (connectedClients[i].Connected == false)
                                connectedClients.Remove(connectedClients[i]);
                        }
                        return;
                    }

                   
                }
            }            
        }

        private static void Broadcast(List<TcpClient> connectedClients, byte[] Msg)
        {
            foreach (var clientOnline in connectedClients)
            {
                NetworkStream nS = clientOnline.GetStream();
                nS.Write(BitConverter.GetBytes(Msg.Length), 0, 4);
                nS.Write(Msg, 0, Msg.Length);
                nS.Flush();
            }
        }

        private static void ProtocolWriteMessage(NetworkStream ns,out string str)
        {
            var ms = new MemoryStream();
            byte[] lengthBytes = new byte[4];
            ns.Read(lengthBytes, 0, 4);
            int length = BitConverter.ToInt32(lengthBytes);
            byte[] buffer = new byte[8];

            int numBytesRead;

            while (ms.Length != length)
            {
                numBytesRead = ns.Read(buffer, 0, buffer.Length);
                ms.Write(buffer, 0, numBytesRead);
            }

            str = Encoding.ASCII.GetString(ms.ToArray());
        }
    }
}

