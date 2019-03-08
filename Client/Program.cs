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
                NetworkStream ns = client.GetStream();
                
                string msg = Console.ReadLine();
                ClearLastLine();
                if (msg == "exit")
                {
                    Disconnect(ns, client);                    
                    break;
                }               
                byte[] buffer = Encoding.ASCII.GetBytes(username + ": " + msg);

                if (buffer.Length > 255)
                {
                    Console.WriteLine("Text can't be bigger then 255 chars!");
                    break;
                }
                
                ns.Write(BitConverter.GetBytes(buffer.Length), 0, 1);
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
            byte[] buffer = new byte[1];
            ns.Read(buffer, 0, 1);
            int length = buffer[0];

            while (ms.Length != length)
            {               
                ms.Write(buffer, 0, ns.Read(buffer, 0, buffer.Length));
            }

            str = Encoding.ASCII.GetString(ms.ToArray());
        }

        public static void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }
    }
}


        
