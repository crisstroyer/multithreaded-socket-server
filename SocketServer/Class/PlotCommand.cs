namespace SocketServer.Class
{
    /// <summary>
    /// Cristian Garcia
    /// cgarcia@ariqmo.com
    /// https://github.com/crisstroyer
    /// 
    /// Plot commands
    /// These are examples of common commands in communication with sensors and plc
    /// </summary>
    public static class PlotCommand{
        /// <summary>
        /// Command to maintain the connection so that the mobile operator does not knock it down
        /// </summary>
        public const string KeepAlive = "k";

        /// <summary>
        /// Periodic shipment of measurement or an alert (immediate shipment) of each sensor
        /// </summary>
        public const string PeriodicMeasurement = "m";

        /// <summary>
        /// Command to request server configuration (master-server address)
        /// </summary>
        public const string ParameterRequest = "p";

        /// <summary>
        /// Command to send the configuration of the levels to the device-master
        /// </summary>
        public const string SendConfiguration = "C";

        /// <summary>
        /// Command to send the server date and time settings
        /// </summary>
        public const string SendDateHour = "D";

        /// <summary>
        /// Command to send the configuration of the slave list to the server
        /// </summary>
        public const string SendListSlave = "E";

        /// <summary>
        /// Command to send the configuration of the sensor list to the server
        /// </summary>
        public const string SendSensorList = "S";

        /// <summary>
        /// Measurement request by the server
        /// </summary>
        public const string RequestMeasurement = "A";

        /// <summary>
        /// Server-Master configuration request
        /// </summary>
        public const string RequestConfiguration = "B";

        /// <summary>
        /// Request for list of slaves configured in the master
        /// </summary>
        public const string RequestListSlave = "F";

        /// <summary>
        /// Request for list of sensors configured in the master
        /// </summary>
        public const string RequestSensorList = "G";

        /// <summary>
        /// Master relay status request
        /// </summary>
        public const string RequestReleState = "H";

        /// <summary>
        /// Measurement request for a specific sensor
        /// External Connection
        /// </summary>
        public const string RequestMeasurementExt = "a";

        /// <summary>
        /// Server-Master configuration request
        /// External Connection
        /// </summary>
        public const string RequestConfigurationExt = "b";

        /// <summary>
        /// Request for list of slaves configured in the master
        /// External Connection
        /// </summary>
        public const string RequestListSlaveExt = "f";

        /// <summary>
        /// Request for a list of sensors configured in the master
        /// External Connection
        /// </summary>
        public const string RequestSensorListExt = "g";

        /// <summary>
        /// Master relay status request
        /// External Connection
        /// </summary>
        public const string RequestReleStateExt = "h";
    }
}
