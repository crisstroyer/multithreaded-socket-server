using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using System.Threading;

namespace SocketServer.Class
{
    /// <summary>
    /// Cristian Garcia
    /// cgarcia@ariqmo.com
    /// https://github.com/crisstroyer
    /// Class in charge of administering the COA service
    /// </summary>
    public class Service{

        #region Variables
        Interpreter interpreter;
        #endregion

        #region Public Method
        /// <summary>
        /// Start the service
        /// </summary>
        public void Start() {
            //Initialize frame settings
            initPlotConfig();

            //Start the plot interpreter
            interpreter = new Interpreter();
            //default port
            State.portIn = 12986;
            //State.portOut = 12987;            

            /*Delegated event to receive data from any connection socket controller
             Method that centralizes receiving data from any socket, from any published server
             */
            State.receive eventReceive = delegate(int port, string handlerId, byte[] data){
                this.eventReceive(port, handlerId, data);
            };

            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ip = ipHostInfo.AddressList[0];

            //Instantiate the necessary servers for two sending and receiving channels
            State.servers.Add(State.portIn, new Server(ip, State.portIn, eventReceive));
            //State.servers.Add(State.portOut, new Server(ip, State.portOut, eventReceive));

            //Start and publish the instantiated servers
            foreach (KeyValuePair<int, Server> s in State.servers){
                //Iniciar cada servidor en un hilo diferente
                (new Thread(new ThreadStart(((Server)s.Value).Start))).Start();
            }

            //Start connection manager
            System.Timers.Timer checker = new System.Timers.Timer(600000);
            checker.Elapsed += checkerEvent;
            checker.Start();
        }

        /// <summary>
        /// End the service
        /// </summary>
        public void End() {
            //Stop published servers
            foreach (KeyValuePair<int, Server> s in State.servers){
                ((Server)s.Value).End();
            }

            //Stop connected controllers
            foreach (KeyValuePair<string, Handler> h in State.handlers){
                ((Handler)h.Value).close();
            }
        }
        #endregion

        #region Private Method

        /// <summary>
        ///Event delegate that consolidates messages received by published servers
        /// and its controllers with their sockets
        /// </summary>
        /// <param name="port">Port</param>
        /// <param name="handler">controller</param>
        /// <param name="data">receive data</param>
        private void eventReceive(int port, string handlerId, byte[] data){
            //Process plot request
            Plot plot = new Plot(data);
            //Get the id of the connected client (The one that sends the message)
            string clientId = plot.Get(PlotFieldKey.SerialDevice).ToString();

            //Retrieve the handler according to the id sent
            Handler handler = State.handlers[handlerId];
            //If the client has not been created, instantiate and assign a controller according to the port
            if (!State.clients.ContainsKey(clientId)){
                State.clients.Add(clientId, new Client(clientId, port, handler));
            }else {
                //If the client already exists, check the controller according to the port
                Client client = State.clients[clientId];
                if (!client.handlers.ContainsKey(port))
                    //Assign the new controller with the port
                    client.addHandler(port, handler);
            }

            //Assign the controller clientId if you don't have it
            if (handler.clientId == null)
                handler.clientId = clientId;

            //Interpret the sent frame
            interpreter.Read(clientId, handler, plot);

        }

        /// <summary>
        /// General service verifier delegated event
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void checkerEvent(Object source, ElapsedEventArgs e){

            //Check the status of the servers
            foreach (KeyValuePair<int, Server> s in State.servers){
                Server server = ((Server)s.Value);
                //If the server is not running remove it from the connections
                if (server.state == StateServer.close) {
                    server.End();
                    State.servers.Remove(server.port);
                }
            }

            //Check the status of each of the controllers connected with their sockets
            foreach (KeyValuePair<string, Handler> h in State.handlers) {
                Handler handler = (Handler)h.Value;
                //Check the last keepAlive
                if (handler.keepAlive != null){
                    //Accept keepAlive time                    

                    //If the keepAlive is valid but the controller has no relationship with the client, close connection
                    //if (handler.clientId == null)
                    //handler.close();
                }
                else {
                    //check creation date and time
                }
            }
        }

        /// <summary>
        /// Method in charge of initializing the configurations of the existing frames
        /// Note: This method initializes the configuration of the frames to be handled with the PLC system, 
        /// this is an example of the configuration for a measurement frame for turbidity, temperature and water level sensors.
        /// </summary>
        private void initPlotConfig() {

            PlotConfig plotConfig = new PlotConfig("GH");
            //Example 1
            plotConfig.Add(new PlotField(PlotFieldKey.MessageLength, "", PlotFieldType.BCD, 2));
            plotConfig.Add(new PlotField(PlotFieldKey.PlotVersion, "version", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.TypeEncrypted, "Type", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.Integrity, "Integri", PlotFieldType.Byte, 3));

            //Example 2
            plotConfig.Add(new PlotField(PlotFieldKey.SerialDevice, "Serial", PlotFieldType.BCD, 4));
            plotConfig.Add(new PlotField(PlotFieldKey.RFCode, "RFCode", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCode, "SensorCode", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.Command, "Command", PlotFieldType.ALFANUMERIC, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.ResponseCode, "ResponseCode", PlotFieldType.Byte, 1));
            State.plotConfigList.Add(plotConfig.key, plotConfig);

            //Example 3
            plotConfig = new PlotConfig("s");
            plotConfig.Add(new PlotField(PlotFieldKey.Alert, "Alert", PlotFieldType.Byte, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.Measure, "Measure", PlotFieldType.Float, 4));
            State.plotConfigList.Add(plotConfig.key, plotConfig);
            
        }
        #endregion
    }
}
