using System;
using System.Text;
using System.Net.Sockets;

namespace SocketServer.Class
{
    /// <summary>
    /// Cristian Garcia
    /// cgarcia@ariqmo.com
    /// https://github.com/crisstroyer
    /// Class in charge of controlling the TCP Socket connection through a specific port
    /// </summary>
    public class Handler
    {
        #region Variable
        //Port through which the controller is connected
        private int _port = 0;
        //Socket associated with the controller
        private Socket _socket;
        //Buffer size to receive
        private const int BufferSize = 1024;
        //Buffer to receive
        private byte[] buffer = new byte[BufferSize];
        //Data received in string.
        private StringBuilder sb = new StringBuilder();
        //Server Delegate Event
        private State.receive _eventReceive;
        #endregion

        #region Properties
        /// <summary>
        /// Indicates the port through which the socket is connected
        /// </summary>
        public int Port { get { return _port;}}

        /// <summary>
        /// Indicates the date and time of the last keepAlive made by the socket
        /// </summary>
        public DateTime keepAlive { get; set; }

        /// <summary>
        /// High level identification of the client to which the controller is associated
        /// </summary>
        public string clientId { get; set; }

        /// <summary>
        /// Socket connection date
        /// </summary>
        public DateTime connectDate { get; set; }

        /// <summary>
        /// Unique identification of the controller
        /// </summary>
        public string Id { get; set; }
        #endregion

        #region Public Method

        /// <summary>
        /// Initialize the driver of the connection made to the socket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="port"></param>
        /// <param name="socket"></param>
        /// <param name="receive">Delegated event to receive data</param>
        public Handler(string id, int port, Socket socket, State.receive receive){
            this._port = port;
            this._socket = socket;
            this._eventReceive = receive;
            this.connectDate = DateTime.Now;
            this.Id = id;
            //Start channel
            BeginReceive(socket);
        }

        /// <summary>
        /// Return the controller socket
        /// </summary>
        /// <returns></returns>
        public Socket getSocket() {
            return this._socket;
        }

        /// <summary>
        /// Close the socket connection
        /// </summary>
        public void close() {
            if (this._socket != null)
                this._socket.Close();
        }

        /// <summary>
        /// Method in charge of sending data through the socket associated with the controller
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data) {
            this.Send(this._socket, data);
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Method in charge of starting the socket receive mode
        /// </summary>
        /// <param name="socket">Connection controller socket</param>
        private void BeginReceive(Socket socket){
            //Resize to default value if value changed
            if (this.buffer.Length != BufferSize)
                Array.Resize(ref buffer, BufferSize);

            socket.BeginReceive(this.buffer, 0, BufferSize, 0, new AsyncCallback(ReadCallback), socket);
        }

        /// <summary>
        /// Asynchronous delegate to receive data on the socket
        /// </summary>
        /// <param name="ar"></param>
        public void ReadCallback(IAsyncResult ar)
        {
            sb = new StringBuilder();
            String content = String.Empty;
            
            //StateObject state = (StateObject)ar.AsyncState;
            Socket handler = (Socket)ar.AsyncState; ;

            try {
                // Read data from the client socket. 
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {

                    //Resize the final Array with the exact size of the received bytes
                    Array.Resize(ref buffer, bytesRead);

                    // There  might be more data, so store the data received so far.
                    sb.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));

                    /*
                     * BitConverter.ToString(buffer) //pasa a hex los bytes
                     * BitConverter.ToString(Encoding.Default.GetBytes("00200110")) //string a bytes y bytes a hex
                     * BitConverter.ToString(Encoding.ASCII.GetBytes("00200110"))
                     */


                    // Check for end-of-file tag. If it is not there, read 
                    // more data.
                    content = sb.ToString();

                    //Console.WriteLine("{0} bytes, socket : {1}, Datos : {2}", content.Length, this.id, content);

                    //Send the data to the server delegate event
                    _eventReceive(this.Port, Id, buffer);

                    // Echo the data back to the client.
                    //Send(handler, "ack");                

                    //Restart the controller to receive data on the socket
                    BeginReceive(handler);
                }
            }
            catch (Exception ex) {
                Console.WriteLine("The PLC client was disconnected: " + ex.Message);
            }
        }

        private void Send(Socket handler, byte[] data){
            // Convert the string data to byte data using ASCII encoding.
            //byte[] byteData = Encoding.ASCII.GetBytes(data);
            byte[] byteData = data;
            Console.WriteLine("Sent: {1}.", this.clientId, String.Join("", Utils.ToHexArray(data)));
            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar){
            try{
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("{0} bytes sent to PLC Client.", bytesSent);

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }catch (Exception e){
                Console.WriteLine(e.ToString());
            }
        }
        #endregion
    }
}
