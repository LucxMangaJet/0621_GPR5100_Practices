using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace UdpPractice
{   
    public enum PacketType
    {
        Message = 1,
        Disconnect,
        SetUsername,
        SetColor

    }

    class ProgramTCP
    {
        private static PacketExcutor packetExcutor;


        public static void MainTCP(string[] args)
        {
            packetExcutor = new PacketExcutor();
            packetExcutor.DefineExecutionFor(typeof(MessagePacket), HandleMessage);


            Console.WriteLine("Start as TCP Server? (YES/NO)");

            string r = Console.ReadLine();

            if (r.ToLowerInvariant() == "YES".ToLowerInvariant())
            {
                Console.WriteLine("Starting as TCP Server");
                TCPServerRoutine();
            }
            else
            {
                Console.WriteLine("Starting as TCP Client");
                TCPClientRoutine();
            }
        }

        private static void TCPServerRoutine()
        {
            var server = new TcpListener(new IPEndPoint(IPAddress.Any, 7777));

            Console.WriteLine("Server is at " + server.LocalEndpoint.ToString());
            server.Start();
            
            while (true)
            {
                Console.WriteLine("Waiting for connection...");

                TcpClient newClient = server.AcceptTcpClient(); //pauses thread untill client joins
                Console.WriteLine("Connected to: " + newClient.Client.RemoteEndPoint);
                Console.WriteLine("Local to: " + newClient.Client.LocalEndPoint);

                Task.Run(() => ReceiveDataTask(newClient));
                Task.Run(() => SendDataTask(newClient));

            }
        }

        static void SendDataTask(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            while (true)
            {
                string line = Console.ReadLine();

                if (line.Length > 0)
                {
                    Packet msg = new MessagePacket(0, line);
                    byte[] data = msg.GetData();

                    stream.Write(data, 0, data.Length);
                }
            }
        }

        private static void ReceiveDataTask(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] bytes = new byte[1024];

            while (true)
            {
                int bytesRead = 0;

                try
                {
                    bytesRead = stream.Read(bytes, 0, bytes.Length);
                }
                catch (System.IO.IOException)
                {
                    return;
                }

                if (bytesRead > 0)
                {
                    try
                    {
                        Packet p = PacketFactory.ToPacket(bytes);

                    } catch (DecodingException e)
                    {
                        Console.WriteLine("Decoding Exception: " + e);
                    }
                }
            }
        }

        private static void TCPClientRoutine()
        {
            var tcpClient = new TcpClient();
            //Console.WriteLine("Socket bound to: " + tcpClient.Client.LocalEndPoint.ToString());

            Console.WriteLine("Write target address: ");
            string targetAddress = Console.ReadLine();
            var targetEndPoint = CreateIPEndPoint(targetAddress);
            Console.WriteLine("Endpoint set to: " + targetEndPoint.ToString());

            try
            {
                tcpClient.Connect(targetEndPoint);

                Task.Run(() => ReceiveDataTask(tcpClient));
                Task.Run(() => SendDataTask(tcpClient));
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error: " + e.SocketErrorCode + " " + (int)e.SocketErrorCode);
                Console.WriteLine("Msg: " + e.Message);
            }
            Thread.Sleep(100000);//Keep the thread alive
        }

        public static IPEndPoint CreateIPEndPoint(string endPoint)
        {
            string[] ep = endPoint.Split(':');
            if (ep.Length != 2) throw new FormatException("Invalid endpoint format");
            IPAddress ip;
            if (!IPAddress.TryParse(ep[0], out ip))
            {
                throw new FormatException("Invalid ip-adress");
            }
            int port;
            if (!int.TryParse(ep[1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Invalid port");
            }
            return new IPEndPoint(ip, port);
        }

        private static void HandleMessage(Packet p )
        {
            MessagePacket packet = (MessagePacket)p;

                    //get username from id
                    Console.WriteLine(packet.Message);

                   //If message 
                    // console.write line message
        }
    }
}
