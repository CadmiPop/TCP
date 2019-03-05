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
    class Prgram
    {
        static void Main(string[] args)
        {
            List<TcpClient> connectedClients = new List<TcpClient>();
            TcpListener chatServer = new TcpListener(IPAddress.Any, 5000);
            chatServer.Start();

            while (true)
            {
                TcpClient chatConnection = chatServer.AcceptTcpClient();
                connectedClients.Add(chatConnection);

                Thread clientThread = new Thread(HandleClientComm);
                clientThread.Start(chatConnection);

                Console.WriteLine("Client Connected!!");
            }

            void HandleClientComm(object client)
            {
                while (true)
                {
                    TcpClient tcpClient = (TcpClient)client;
                    NetworkStream ns = tcpClient.GetStream();

                    byte[] Msg = new byte[256];
                    int bytesRead = 0;

                    try
                    {
                        bytesRead = ns.Read(Msg, 0, Msg.Length);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    Array.Resize(ref Msg, bytesRead);

                    Console.WriteLine(Encoding.Default.GetString(Msg));

                    foreach (var a in connectedClients)
                    {
                        ns.Write(Msg, 0, Msg.Length);
                        ns.Flush();
                    }
                    
                }  
            }
        }


        private static void WriteToAllClients(NetworkStream ns)
        {
            byte[] newMsg = new byte[256];
            int a = ns.Read(newMsg, 0, newMsg.Length);
            Array.Resize(ref newMsg, a);
            Console.WriteLine(Encoding.Default.GetString(newMsg));
            ns.Write(newMsg, 0, newMsg.Length);
        }
    }
}

