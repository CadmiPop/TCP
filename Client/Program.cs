using System;
using System.Collections.Specialized;
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
                    clientThread.Interrupt();
                    break;
                }

                byte[] buffer = Encoding.ASCII.GetBytes(username + ": " + msg);

                ns.Write(buffer, 0, buffer.Length);
                ns.Flush();
                
                //byte[] receivedBytes = new byte[256];
                //int byteCount = 0;
                //string data = String.Empty;
                //while (true)
                //{
                //    try
                //    {
                //        byteCount = ns.Read(receivedBytes, 0, receivedBytes.Length);
                //        byte[] formated = new byte[byteCount];

                //        Array.Copy(receivedBytes, formated, byteCount);
                //        Array.Resize(ref formated, buffer.Length);
                //        data = Encoding.ASCII.GetString(formated);
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine(ex.Message);
                //    }
                //    Console.WriteLine(data);
                //}               
            }
        }

        private static void HandleClientComm(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            byte[] Msg = new byte[256];
            int bytesRead = 0;
            while (true)
            {
                try
                {
                    bytesRead = ns.Read(Msg, 0, Msg.Length);
                }
                catch (ObjectDisposedException ex)
                {
                    Console.WriteLine("gfg");
                }
                Array.Resize(ref Msg, bytesRead);
                Console.WriteLine(Encoding.Default.GetString(Msg));
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

    }
}


        
