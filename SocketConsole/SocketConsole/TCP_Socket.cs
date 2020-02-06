using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketConsole
{
    class TCP_Socket
    {
        private string lasterror;
        private string server;
        private int port;
        private Socket s;
        ASCIIEncoding encoding = new ASCIIEncoding();

        public TCP_Socket()
        {

        }

        public TCP_Socket(string Server, int Port)
        {
            server = Server;
            port = Port;
        }

        public string LastError
        {
            get { return this.lasterror; }
            set { this.lasterror = value; }
        }

        public string Server
        {
            get { return this.server; }

            set { this.server = value; }
        }
        public int Port
        {
            get { return this.port; }

            set { this.port = value; }
        }

        public bool Connect()
        {
            // Create a socket connection with the specified server and port.
            s = ConnectSocket(server, port);

            if (s == null)
                return false;
            else
                return true;
        }

        public bool IsConnect()
        {
            return s.Connected;
        }

        public bool Disconnect()
        {
            try
            {
                if (s.Connected)
                {
                    s.Disconnect(false);
                }
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        public bool SendData(byte[] data, bool waitResponse, int timeoutSend = 60000)
        {
            try
            {
                // Send request to the server.
                s.SendTimeout = timeoutSend;
                s.Send(data, data.Length, 0);
                byte[] res_byte = new byte[100];
                if (waitResponse)
                {
                    s.Receive(res_byte);

                    string res = encoding.GetString(res_byte).Replace("\0", string.Empty);


                    if (res == "true")
                        return true;
                    else
                        return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }



        }

        public bool SendData(string data)
        {
            try
            {
                byte[] data_byte = encoding.GetBytes(data);
                // Send request to the server.
                s.Send(data_byte, data_byte.Length, 0);
                byte[] res_byte = new byte[100];
                s.Receive(res_byte);

                string res = encoding.GetString(res_byte).Replace("\0", string.Empty);


                if (res == "true")
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public bool SendData(string data,out string responseMessage, bool waitResponse = false, int sendTimeOut = 0, int recieveTimeOut = 0)
        {
            try
            {
                byte[] data_byte = encoding.GetBytes(data);
                // Send request to the server.
                s.SendTimeout = sendTimeOut;
                s.Send(data_byte, data_byte.Length, 0);
                byte[] res_byte = new byte[100];
                if (waitResponse)
                {
                    s.ReceiveTimeout = recieveTimeOut;
                    s.Receive(res_byte);

                    string res = encoding.GetString(res_byte).Replace("\0", string.Empty);
                    responseMessage = res;
                    return true;
                }
                responseMessage = "";
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                responseMessage = null;
                return false;
            }
        }

        public string recieve(int recieveTimeOut = 0)
        {
            try
            {
                byte[] res_byte = new byte[100];
                s.ReceiveTimeout = recieveTimeOut;
                s.Receive(res_byte);

                string res = encoding.GetString(res_byte).Replace("\0", string.Empty);
                return res;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return null;
            }
        }

        private static void ProcessVehicleId(byte[] message, byte[] vehicleIdBytes)
        {
            System.Buffer.BlockCopy(vehicleIdBytes, 0, message, 0, vehicleIdBytes.Length);
            int k = 0;
            for (; k < 20 - vehicleIdBytes.Length - 1; k++)
            {
                //message.Add(0x00);
                message[k] = 0x00;
            }
            //message.Add(0x00);
            message[19] = 0x00;
        }

        private byte[] GetBytes(string str, int Size = 20)
        {
            byte[] bytes = new byte[Size];
            if (Size > str.Length * sizeof(char))
                System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, str.Length * sizeof(char));
            else
                System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        //private Socket ConnectSocket(string server, int port)
        //{
        //    if (String.IsNullOrEmpty(server))
        //    {
        //        throw new Exception("Server name cannot be null. Please initial server name or server IP.");
        //    }

        //    if (port == 0)
        //    {
        //        throw new Exception("port number cannot be 0. Please initail server port.");
        //    }

        //    if(providerID == 0)
        //    {
        //        throw new Exception("ProviderID cannot be 0. Please initail ProviderID.");
        //    }

        //    Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    IPAddress remoteIPAddress = IPAddress.Parse(server);
        //    IPEndPoint remoteEndPoint = new IPEndPoint(remoteIPAddress, port);
        //    s.Connect(remoteEndPoint);

        //    return s;
        //}

        private Socket ConnectSocket(string server, int port)
        {
            if (String.IsNullOrEmpty(server))
            {
                throw new Exception("Server name cannot be null. Please initial server name or server IP.");
            }

            if (port == 0)
            {
                throw new Exception("port number cannot be 0. Please initail server port.");
            }

            try
            {
                Socket s = null;
                IPHostEntry hostEntry = null;

                // Get host related information.
                hostEntry = Dns.GetHostEntry(server);

                // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid 
                // an exception that occurs when the host IP Address is not compatible with the address family 
                // (typical in the IPv6 case). 
                foreach (IPAddress address in hostEntry.AddressList)
                {
                    IPEndPoint ipe = new IPEndPoint(address, port);
                    // Check IPv4?
                    if (ipe.AddressFamily == AddressFamily.InterNetwork)
                    {
                        // Create Socket TCP
                        Socket tempSocket =
                            new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        tempSocket.Connect(ipe);

                        if (tempSocket.Connected)
                        {
                            s = tempSocket;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                return s;
            }
            catch (Exception ex)
            {
                //throw new Exception("Error : " + ex.Message);
                return null;
            }

        }
    }
}
