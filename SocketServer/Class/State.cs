using System.Collections.Generic;

namespace SocketServer.Class
{
    /// <summary>
    /// Cristian Garcia
    /// cgarcia@ariqmo.com
    /// https://github.com/crisstroyer
    /// Class in charge of controlling the status of the executed service
    /// </summary>
    public static class State{
        /// <summary>
        /// Default port for actions to receive customer data
        /// </summary>
        public static int portIn;

        /// <summary>
        /// Default port for actions to send data to clients
        /// </summary>
        public static int portOut;

        /// <summary>
        /// Dictionary with connected clients
        /// </summary>
        public static Dictionary<string, Client> clients = new Dictionary<string, Client>();

        /// <summary>
        /// Dictionary with all Socket TCP controllers connected
        /// </summary>
        public static Dictionary<string, Handler> handlers = new Dictionary<string, Handler>();

        /// <summary>
        /// Dictionary with published servers
        /// </summary>
        public static Dictionary<int, Server> servers = new Dictionary<int, Server>();

        /// <summary>
        /// Server delegated event to receive messages from any connected handler
        /// </summary>
        /// <param name="port">Puerto por el que sale el socket</param>
        /// <param name="handler">Controlador del Socket</param>
        /// <param name="data">Datos recibidos</param>
        public delegate void receive(int port, string handlerId, byte[] data);

        /// <summary>
        /// Dictionary with the configuration of existing frames
        /// the dictionary key is the frame command
        /// </summary>
        public static Dictionary<string, PlotConfig> plotConfigList = new Dictionary<string, PlotConfig>();
    }
}
