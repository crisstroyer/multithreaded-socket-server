using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace COAService2
{
    /// <summary>
    /// Clase encargada de administrar las conexiones tcp socket de los pcl conectados
    /// </summary>
    public class Client
    {
        #region Variables
        //Tamaño del buffer para recibir
        private const int BufferSize = 1024;
        //Buffer para recibir
        private byte[] buffer = new byte[BufferSize];
        //Datos recibidos en string.
        private StringBuilder sb = new StringBuilder();
        //Lista de sockets asociados al cliente de acuerdo a los puertos publicados
        private Dictionary<int, Socket> sockets;


        private state.receive reseive;
        #endregion

        #region Propiedades
        public string id { get; set; }
        #endregion

        #region Metodos Publicos
        /// <summary>
        /// Crear instancia de Cliente con su id y su handler segun conexion y puerto
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handler"></param>
        public Client(string id, Socket handler, state.receive _delegate)
        {
            //Inicializar los handlers de cada cliente
            sockets = new Dictionary<int, Socket>();
            this.id = id;
            this.reseive = _delegate;
            //Iniciar la escucha del socket para recibir informacion
            BeginReceive(handler);
            //Agregar el handler por defecto
            addHandler(handler);            
        }

        /// <summary>
        /// Metodo encargado de enviar datos al socket conectado
        /// Este metodo envia por el servidor y puerto de envio
        /// </summary>
        /// <param name="data"></param>
        public void Send(string data) {
            //this.SendData(this.handlerOut, data);
        }

        public void addHandler(Socket handler) {
            sockets.Add(((IPEndPoint)handler.RemoteEndPoint).Port, handler);
        }
        #endregion

        #region Metodos Privados
        private void BeginReceive(Socket handler){
            handler.BeginReceive(this.buffer, 0, BufferSize, 0, new AsyncCallback(ReadCallback), handler);
        }

        /// <summary>
        /// Delegado asincrono para recibir datos en el socket
        /// </summary>
        /// <param name="ar"></param>
        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            //String id2 = (String)ar.AsyncState;
            //Socket handler = sockets[12];

            
            //StateObject state = (StateObject)ar.AsyncState;
            Socket handler = (Socket)ar.AsyncState; ;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                sb.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = sb.ToString();
                //if (content.IndexOf("<EOF>") > -1)
               // {
                    // All the data has been read from the 
                    // client. Display it on the console.
                    Console.WriteLine("{0} bytes, socket : {1}, Datos : {2}", content.Length, this.id, content);

                    reseive(handler, content);
                    // Echo the data back to the client.
                    Send(handler, "ack");
                    //Send(handler, content);

                    //handler.BeginReceive(buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

                    BeginReceive(handler);

                    //Send(sockets[501], "0102");
                /* }
                 else
                 {
                     // Not all data received. Get more.
                     handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                     new AsyncCallback(ReadCallback), state);
                 }*/
            }
        }

        private void Send(Socket handler, string data) {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar){
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #endregion
    }

}
