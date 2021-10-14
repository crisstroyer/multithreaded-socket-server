using System;
using System.Collections.Generic;

namespace SocketServer.Class
{
    /// <summary>
    /// Cristian Garcia
    /// cgarcia@ariqmo.com
    /// https://github.com/crisstroyer
    /// Class in charge of managing at a high level the clients connected by TCP Socket controllers
    /// </summary>
    public class Client{
        #region Variables
        public Dictionary<int, Handler> handlers;
        #endregion

        #region Propiedades
        public String Id { get; set; }
        #endregion

        #region Metodos
        /// <summary>
        /// Initialize the Client
        /// </summary>
        public Client(){
            this.handlers = new Dictionary<int, Handler>();
        }

        /// <summary>
        /// Initialize the Client
        /// </summary>
        /// <param name="id">Identificacion Alto Nivel del cliente</param>
        public Client(string id){
            this.handlers = new Dictionary<int, Handler>();
            this.Id = id;            
        }

        /// <summary>
        /// Initialize the Client
        /// </summary>
        /// <param name="id">High level customer identification</param>
        /// <param name="port">Connection port</param>
        /// <param name="handler">Controller TCP Socket connection</param>
        public Client(string id, int port, Handler handler){
            this.handlers = new Dictionary<int, Handler>();
            this.Id = id;
            this.addHandler(port, handler);
        }

        /// <summary>
        /// Add a tcp socket driver to the client
        /// </summary>
        /// <param name="port">Port through which the connection is made</param>
        /// <param name="handler">controller</param>
        public void addHandler(int port, Handler handler){            
            handlers.Add(port, handler);
        }

        /// <summary>
        /// Retrieve the client driver according to the port sent
        /// </summary>
        /// <param name="port">Port through which the connection is made</param>
        /// <returns></returns>
        public Handler getHandler(int port){
            return handlers[port];
        }
        #endregion
    }
}
