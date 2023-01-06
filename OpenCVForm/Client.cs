using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace OpenCVForm
{
    internal class Client
    {
        Socket mainSock;
        public Client()
        {
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void Connect(string serverIp = "10.10.20.97", int m_port = 2500)
        {
            IPAddress serverAddr = IPAddress.Parse(serverIp);
            IPEndPoint clientEP = new IPEndPoint(serverAddr, m_port);
            mainSock.BeginConnect(clientEP, ConnectCallback, mainSock);
        }
        public void Close()
        {
            if (mainSock != null)
            {
                mainSock.Close();
                mainSock.Dispose();
            }
        }
        public class StateObject
        {
            public Socket workSocket;
            public const int BUFFER_SIZE = 1920 * 1080 * 3;
            public byte[] buffer = new byte[BUFFER_SIZE];
            public StateObject(Socket socket)
            {
                workSocket = socket;
            }
        }
        void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket mainSock2 = (Socket)ar.AsyncState;
                mainSock2.EndConnect(ar);
                StateObject obj = new StateObject(mainSock2);
                Connected?.Invoke(true, EventArgs.Empty);
                mainSock.BeginReceive(obj.buffer, 0, StateObject.BUFFER_SIZE, 0, DataReceived, obj);
            }
            catch (Exception e)
            {
                Connected?.Invoke(false, EventArgs.Empty);
                System.Console.WriteLine(e.Message);
            }
        }
        void DataReceived(IAsyncResult ar)
        {
            try
            {
                StateObject obj = (StateObject)ar.AsyncState;
                int received = obj.workSocket.EndReceive(ar);
                byte[] rawData = new byte[received - 4];
                Array.Copy(obj.buffer, 4, rawData, 0, received - 4);
                obj.workSocket.BeginReceive(obj.buffer, 0, StateObject.BUFFER_SIZE, 0, DataReceived, obj);

                Decode dcd = (Decode)(new DecodeTCP(rawData));
                DataResponsed?.Invoke(dcd, EventArgs.Empty);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
        public void SendEncode(Encode ecd)
        {
            List<byte> totalBytes = new List<byte>();
            totalBytes.AddRange(ecd.totalSizeByte());
            totalBytes.AddRange(ecd.HeaderBytes);
            totalBytes.AddRange(ecd.DataBytes);
            mainSock.BeginSend(totalBytes.ToArray(), 0, totalBytes.Count, 0, FinishSend, mainSock);
        }

        public void FinishSend(IAsyncResult ar)
        {
            Socket mainSock2 = (Socket)ar.AsyncState;
            mainSock2.EndSend(ar);
        }

        public void SendDrivingImage(Mat img)
        {
            EcdDrivingImage ecdImage = new EcdDrivingImage(img);
            SendEncode(ecdImage);
        }

        public event EventHandler? Connected;
        public event EventHandler? DataResponsed;
    }
}
