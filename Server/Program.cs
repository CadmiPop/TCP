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
                TcpClient chatConnection = chatServer.AcceptTcpClient();
                connectedClients.Add(chatConnection);

                Thread clientThread = new Thread(()=>HandleClientComm(chatConnection));
                clientThread.Start();

                Console.WriteLine("Client Connected!!");
            }

            void HandleClientComm(TcpClient client)
            {
                while (true)
                {                  
                    NetworkStream ns = client.GetStream();

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

                    foreach (var clientOnline in connectedClients)
                    {
                        NetworkStream nS = clientOnline.GetStream();
                        nS.Write(Msg, 0, Msg.Length);
                        nS.Flush();                       
                    }                   
                }  
            }
        }


        private static void ProtocolWritekMessage(string message, TcpClient client)
        {
            NetworkStream networkStream = client.GetStream();

            byte[] messageBytes = Encoding.ASCII.GetBytes(message); 
            int length = messageBytes.Length;
            byte[] lengthBytes = System.BitConverter.GetBytes(length);

            networkStream.Write(lengthBytes, 0, lengthBytes.Length);
            networkStream.Write(messageBytes, 0, length);
        }
    }
}

