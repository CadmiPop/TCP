using System.IO;
using System.Net;
using System;
using System.Threading;
using Chat = System.Net;
using System.Collections;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class ChatServer
    {

        public static Hashtable nickName;
        public static Hashtable nickNameByConnect;
        private TcpListener chatServer;

        public ChatServer()
        {
            nickName = new Hashtable(100);
            nickNameByConnect = new Hashtable(100);
            Server();
        }

        public TcpListener Server()
        {
            chatServer = new TcpListener(IPAddress.Any, 5000);
            chatServer.Start();

            while (true)
            {
                if (chatServer.Pending())
                {
                    TcpClient chatConnection = chatServer.AcceptTcpClient();
                    Console.WriteLine("Connection made");
                    NetworkStream ns = chatConnection.GetStream();
                    byte[] hello = new byte[100];
                    hello = Encoding.Default.GetBytes("hello world");

                    ns.Write(hello, 0, hello.Length);

                    while (chatConnection.Connected)
                    {

                        byte[] msg = new byte[1024];
                        ns.Read(msg, 0, msg.Length);
                        Console.WriteLine(Encoding.Default.GetString(msg));
                    }
                    Console.WriteLine("You are now connected");
                }
            }
        }
    }
}
