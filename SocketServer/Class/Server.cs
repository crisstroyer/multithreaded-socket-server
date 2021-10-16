using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketServer.Class
{
    public enum StateServer { 
        running = 1,
        close = 2
    }

    /// <summary>
    /// Cristian Garcia
    /// cgarcia@ariqmo.com
    /// https://github.com/crisstroyer
    /// Multithread Socket TCPIP Server
    /// </summary>
    public class Server{
        #region Variables
        //Signal for asynchronous wires.
        public ManualResetEvent allDone = new ManualResetEvent(false);
        private Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Server Delegate event
        private static State.receive _eventReceive;
        #endregion

        #region Properties
        public int port { get; set; }
        public IPAddress ipAddress { get; set; }

        public StateServer state { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Start Server instance
        /// </summary>
        /// <param name="ipAddress">IP of the publication</param>
        /// <param name="port">port</param>
        /// <param name="receive">Delegated event to receive data from any socket to the service running the TCP Socket server</param>
        public Server(IPAddress ipAddress, int port, State.receive receive){
            this.ipAddress = ipAddress;
            this.port = port;
            _eventReceive = receive;
        }

        /// <summary>
        /// Start the TCP Socket server on the assigned IP and port
        /// </summary>
        public void Start(){
            //Set the local endpoint for the socket.        
            IPEndPoint localEndPoint = new IPEndPoint(this.ipAddress, this.port);

            //Bind the socket to the local endpoint and listen for incoming connections
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    //Set the event to an unreported state.
                    allDone.Reset();

                    //Start the asynchronous socket to wait for connections
                    Console.WriteLine("Waiting for port connections " + this.port + "...");
                    this.state = StateServer.running;
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    //Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            }
            catch (Exception e){
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Terminate the TCP Socket server
        /// </summary>
        public void End() {
            this.listener.Disconnect(true);
            this.state = StateServer.close;
        }
        #endregion

        #region Metodos Privados
        /// <summary>
        /// Asynchronous delegate to accept socket connections
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCallback(IAsyncResult ar)
        {
            //Point to the main thread to continue.
            allDone.Set();
            //Get the socket that handles the request from the client.
            Socket listener = (Socket)ar.AsyncState;
            Socket socket = listener.EndAccept(ar);

            String id = Guid.NewGuid().ToString();

            //Create and add the new connected socket driver
            State.handlers.Add(id, new Handler(id, ((IPEndPoint)socket.LocalEndPoint).Port, socket, _eventReceive));
        }
        #endregion
    }
}
