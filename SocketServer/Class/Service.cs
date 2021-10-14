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

            //Ingresar el Encagezado Generico
            PlotConfig plotConfig = new PlotConfig("EG");
            
            plotConfig.Add(new PlotField(PlotFieldKey.MessageLength, "", PlotFieldType.BCD, 2));
            plotConfig.Add(new PlotField(PlotFieldKey.PlotVersion, "Versión Trama", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.TypeEncrypted, "Tipo Cifrado", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.Integrity, "Integridad", PlotFieldType.Byte, 3));

            //Encabezado Estandar
            plotConfig.Add(new PlotField(PlotFieldKey.SerialDevice, "Serial Dispositivo", PlotFieldType.BCD, 4));
            plotConfig.Add(new PlotField(PlotFieldKey.RFCode, "Código RF", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCode, "Código Sensor", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.CanalCode, "Canal", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.Command, "Comando", PlotFieldType.ALFANUMERIC, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.ResponseCode, "Código respuesta", PlotFieldType.Byte, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.TimeStamp, "Fecha y Hora", PlotFieldType.BCD, 7));
            plotConfig.Add(new PlotField(PlotFieldKey.NUT, "NUT", PlotFieldType.BCD, 4));
            State.plotConfigList.Add(plotConfig.key, plotConfig);

            //Trama comando m
            plotConfig = new PlotConfig("m");
            plotConfig.Add(new PlotField(PlotFieldKey.Alert, "Alertas", PlotFieldType.Byte, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.Measure, "Medición", PlotFieldType.Float, 4));
            State.plotConfigList.Add(plotConfig.key, plotConfig);

            //Trama comando n
            plotConfig = new PlotConfig("n");
            plotConfig.Add(new PlotField(PlotFieldKey.ListSlave, "Lista Esclavos", PlotFieldType.Byte, 1));
            State.plotConfigList.Add(plotConfig.key, plotConfig);

            //Trama comando l
            plotConfig = new PlotConfig("l");
            plotConfig.Add(new PlotField(PlotFieldKey.SensorList, "Lista Sensores", PlotFieldType.Byte, 1));
            State.plotConfigList.Add(plotConfig.key, plotConfig);

            //Trama comando p
            plotConfig = new PlotConfig("p");
            plotConfig.Add(new PlotField(PlotFieldKey.DateHour, "Fecha y Hora", PlotFieldType.ALFANUMERIC, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.ListSlave, "Lista Esclavos", PlotFieldType.ALFANUMERIC, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorList, "Lista Sensores", PlotFieldType.ALFANUMERIC, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.configuration, "Configuración", PlotFieldType.ALFANUMERIC, 1));
            State.plotConfigList.Add(plotConfig.key, plotConfig);

            //Trama comando o
            plotConfig = new PlotConfig("o");
            plotConfig.Add(new PlotField(PlotFieldKey.ReleNumber, "Número Relé", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.Operation, "Operación", PlotFieldType.BCD, 1));
            State.plotConfigList.Add(plotConfig.key, plotConfig);

            //Trama comando v
            plotConfig = new PlotConfig("v");
            plotConfig.Add(new PlotField(PlotFieldKey.VersionP1, "Versión P1", PlotFieldType.BCD, 2));
            plotConfig.Add(new PlotField(PlotFieldKey.VersionP2, "Versión P2", PlotFieldType.BCD, 2));
            plotConfig.Add(new PlotField(PlotFieldKey.VersionP3, "Versión P3", PlotFieldType.BCD, 2));
            plotConfig.Add(new PlotField(PlotFieldKey.VersionHw, "Versión Hw", PlotFieldType.BCD, 2));
            State.plotConfigList.Add(plotConfig.key, plotConfig);

            //Trama de envio comando D            
            plotConfig = new PlotConfig(PlotCommand.SendDateHour);
            State.plotConfigList.Add(plotConfig.key, plotConfig);

            //Trama de envio comando C
            plotConfig = new PlotConfig(PlotCommand.SendConfiguration);
            plotConfig.Add(new PlotField(PlotFieldKey.PeriodicalReportingTime, "Tiempo Reporte Periodico", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.HighLowWaitTime, "Tiempo Espera Alto o Bajo", PlotFieldType.BCD, 2));
            plotConfig.Add(new PlotField(PlotFieldKey.HighHighValue, "Valor Alto-Alto", PlotFieldType.Float, 4));
            plotConfig.Add(new PlotField(PlotFieldKey.HighValue, "Valor Alto", PlotFieldType.Float, 4));
            plotConfig.Add(new PlotField(PlotFieldKey.LowValue, "Valor Bajo", PlotFieldType.Float, 4));
            plotConfig.Add(new PlotField(PlotFieldKey.LowLowValue, "Valor Bajo-Bajo", PlotFieldType.Float, 4));
            State.plotConfigList.Add(plotConfig.key, plotConfig);

            //Trama de envio comando E
            plotConfig = new PlotConfig(PlotCommand.SendListSlave);
            plotConfig.Add(new PlotField(PlotFieldKey.ListSlave, "Lista Esclavos", PlotFieldType.Byte, 1));
            State.plotConfigList.Add(plotConfig.key, plotConfig);

            //Trama de envio comando S
            plotConfig = new PlotConfig(PlotCommand.SendSensorList);
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCodeA, "Codigo Sensor A", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCanalA, "Canal Sensor A", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorTypeA, "Tipo Sensor A", PlotFieldType.Byte, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCodeB, "Codigo Sensor B", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCanalB, "Canal Sensor B", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorTypeB, "Tipo Sensor B", PlotFieldType.Byte, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCodeC, "Codigo Sensor C", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCanalC, "Canal Sensor C", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorTypeC, "Tipo Sensor C", PlotFieldType.Byte, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCodeD, "Codigo Sensor D", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCanalD, "Canal Sensor D", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorTypeD, "Tipo Sensor D", PlotFieldType.Byte, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCodeE, "Codigo Sensor E", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCanalE, "Canal Sensor E", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorTypeE, "Tipo Sensor E", PlotFieldType.Byte, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCodeF, "Codigo Sensor F", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCanalF, "Canal Sensor F", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorTypeF, "Tipo Sensor F", PlotFieldType.Byte, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCodeG, "Codigo Sensor G", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCanalG, "Canal Sensor G", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorTypeG, "Tipo Sensor G", PlotFieldType.Byte, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCodeH, "Codigo Sensor H", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorCanalH, "Canal Sensor H", PlotFieldType.BCD, 1));
            plotConfig.Add(new PlotField(PlotFieldKey.SensorTypeH, "Tipo Sensor H", PlotFieldType.Byte, 1));
            State.plotConfigList.Add(plotConfig.key, plotConfig);

            //Trama de envio comando A    
            plotConfig = new PlotConfig(PlotCommand.RequestMeasurement);
            State.plotConfigList.Add(plotConfig.key, plotConfig);
        }
        #endregion
    }
}
