using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;


using System.Timers;

namespace COAService2
{
    class Service2
    {
        #region Variables
        delegate double receive(Socket handler, String data);

        //Señal para los hilos asyncronos.
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static List<Client> clients;
        #endregion

        public int port { get; set; }
        public IPAddress ipAddress { get; set; }
        
        public Service(IPAddress ipAddress, int port){
            this.ipAddress = ipAddress;
            this.port = port;                        
        }

        public void Start(object clients) {
            //Asignar la lista de clientes existentes
            clients = (List<Client>)clients;

            //Establecer el punto final local para el socket.        
            IPEndPoint localEndPoint = new IPEndPoint(this.ipAddress, this.port);

            //Vincular el socket al punto final local y escuchar las conexiones entrantes
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    //Establecer el evento en estado no señalado.
                    allDone.Reset();

                    //Iniciar el socket asincrono para esperar conexiones
                    Console.WriteLine("Esperando conexiones puerto " + this.port + "...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    //Esperar hasta que se realice una conexión antes de continuar.
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void end() { }

        #region Metodos Privados

        /// <summary>
        /// Delegado asincrono para aceptar conexiones del socket
        /// </summary>
        /// <param name="ar"></param>
        private static void AcceptCallback(IAsyncResult ar)
        {
            //Señalar el hilo principal para continuar.
            allDone.Set();

            //Obtener el socket que controla la solicitud del cliente.
            Socket listener = (Socket)ar.AsyncState;
            
            Socket handler = listener.EndAccept(ar);
            string id = Guid.NewGuid().ToString();

            System.Timers.Timer timer = new System.Timers.Timer(1000);
              timer.Elapsed += async(sender, e) => await HandleTimer();
              timer.Start();
              Console.Write("Press any key to exit... ");
              Console.ReadKey();
   

            //if(clients.ContainsKey(id))
                //clients[id].
           // else
            state.clients.Add(new Client(id, handler, delegate(Socket socket, String data) {
                String d = data;
                Socket s = socket;
            }));

           /* Client newClient;
            foreach (Client client in clients) { 
                if(client.id = 
            }*/

            //clients.Add(new Client(id, handler));
            Console.WriteLine("Conexion aceptada puerto " + ((IPEndPoint)handler.RemoteEndPoint).Port);
            //new Client(id, handler);

            //Crear el objeto cliente
            /*StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);*/
        }

       
        #endregion
    }
}
