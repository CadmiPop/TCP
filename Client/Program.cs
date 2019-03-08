using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = ConnectClient();
                             
            Console.WriteLine("Enter Username:");
            string username = Console.ReadLine();

            Thread clientThread = new Thread(()=>HandleClientComm(client));
            clientThread.Start();

            while (client.Connected)
            {
                Console.WriteLine("Send Message:");
                NetworkStream ns = client.GetStream();
                
                string msg = Console.ReadLine();
                if (msg == "exit")
                {
                    Disconnect(ns, client);                    
                    break;
                }               
                byte[] buffer = Encoding.ASCII.GetBytes(username + ": " + msg);
                
                ns.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
                ns.Write(buffer, 0, buffer.Length);
                ns.Flush();
                       
            }
        }

        private static void HandleClientComm(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            byte[] Msg = new byte[255];
            string str = String.Empty;
            while (true)
            {
                try
                {
                    ProtocolWriteMessage(ns,out str);
                }
                catch (Exception)
                {
                    return;
                }
                
                Console.WriteLine(str);
            }          
        }

        private static TcpClient ConnectClient()
        {
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse("127.0.0.1"), 5000);
            Console.WriteLine("Connected to Server!!");
            return client;
        }

        private static void Disconnect(NetworkStream ns, TcpClient client)
        {
            ns.Close();
            client.Close();
        }

        private static void ProtocolWriteMessage(NetworkStream ns, out string str)
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


        
