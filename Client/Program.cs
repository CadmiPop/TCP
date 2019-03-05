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

            //Thread clientThread = new Thread(HandleClientComm);


            while (client.Connected)
            {
                NetworkStream ns = client.GetStream();
                Console.WriteLine("Send Message:");
                string msg = Console.ReadLine();
                byte[] buffer = Encoding.ASCII.GetBytes(username + ": " + msg);

                ns.Write(buffer, 0, buffer.Length);
                ns.Flush();

                byte[] receivedBytes = new byte[256];
                int byteCount = 0;
                string data = String.Empty;
                try
                {
                    //blocks until a client sends a message
                    byteCount = ns.Read(receivedBytes, 0, receivedBytes.Length);
                    byte[] formated = new byte[byteCount];

                    Array.Copy(receivedBytes, formated, byteCount);
                    Array.Resize(ref formated, buffer.Length);
                    data = Encoding.ASCII.GetString(formated);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine(data);
            }
        }

        private static void HandleClientComm(object obj)
        {
            throw new NotImplementedException();
        }

        private static TcpClient ConnectClient()
        {
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse("127.0.0.1"), 5000);
            Console.WriteLine("Connected to Server!!");
            return client;
        }
    }
}


        
