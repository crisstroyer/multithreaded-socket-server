using System;
using System.Linq;

namespace SocketServer.Class
{    
    public enum NotificationType{
        AlertMeasure = 1,
        DeviceDisconnected = 2
    }

    /// <summary>
    /// Cristian Garcia
    /// cgarcia@ariqmo.com
    /// https://github.com/crisstroyer
    /// Class in charge of interpreting the message sent by the tcp socket according to the configured frames
    /// </summary>
    class Interpreter
    {
        #region Metodos Publicos

        /// <summary>
        /// Method in charge of reading and interpreting the message of the plot
        /// and execute the relevant action according to the request
        /// </summary>
        /// <param name="clientId">Customer identification</param>
        /// <param name="handler">Client socket driver</param>
        /// <param name="plot">Plot object</param>
        public void Read(string clientId, Handler handler, Plot plot) {
            //Get the command of the sent frame
            string command = (string)plot.Get(PlotFieldKey.Command);
            Console.WriteLine("Plot Command " + command + ":" + plot.ToString());

            //Si la trama es enviada por el puerto de entrada
            //if (handler.Port == State.portIn){
            if (command.Any(char.IsLower)) {
                //Process frame only if client is connected
                if (State.clients.ContainsKey(clientId)){
                    //Execute the action according to the command of the frame sent
                    this.ExecuteAction(command, plot, handler, clientId);
                    //Send ACK
                    SendACK(handler, plot);
                }
            }
            else {
                //ACK of the sending of the information that comes from the master
                Console.WriteLine("ACK-Maestro " + command + ":" + plot.ToString());
            }
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Method in charge of executing the required action according to the frame sent from the plc
        /// </summary>
        /// <param name="command">Command to process</param>
        /// <param name="plot">Plot sent</param>
        /// <param name="handler">Generic socket driver</param>
        /// <param name="clientId">High level identification of the plc</param>
        private void ExecuteAction(string command, Plot plot, Handler handler, string clientId) {
            //Process the command according to the frame sent from the integrator or web client
            switch (command) {
                case PlotCommand.ParameterRequest:
                    break;
                case PlotCommand.KeepAlive:
                    //Update the keepAlive of the connection
                    //handler.keepAlive = DateTime.Parse((string)plot.Get(PlotFieldKey.TimeStamp));
                    break;
                case PlotCommand.PeriodicMeasurement:
                    break;
                case PlotCommand.RequestMeasurementExt:
                case PlotCommand.RequestConfigurationExt:
                case PlotCommand.RequestListSlaveExt:
                case PlotCommand.RequestSensorListExt:
                    break;
            }
        }

        /// <summary>
        /// Return ACK of the current request (Inbound Handler)
        /// This is only done for client request frames,
        /// for response frames ack is not performed
        /// </summary>
        /// <param name="handler">Generic plc connection socket driver</param>
        /// <param name="plot">Plot sent</param>
        private void SendACK(Handler handler, Plot plot) {
            Console.WriteLine("ACK: {1}.", handler.clientId, String.Join("", Utils.ToHexArray(plot.ToByteArray("EG"))));
            //Return only the general header
            handler.Send(plot.ToByteArray("EG"));
        }

        
        #endregion
    }
}
