namespace SocketServer.Class
{
    /// <summary>
    /// Cristian Garcia
    /// cgarcia@ariqmo.com
    /// https://github.com/crisstroyer
    /// Class in charge of managing the values ​​of each field of the plot
    /// </summary>
    public class PlotFieldValue
    {
        /// <summary>
        /// Property for handling the value of the plot field
        /// </summary>
        public byte[] Value { get; set; }
        /// <summary>
        /// plot field settings
        /// </summary>
        public PlotField PlotField { get; set; }
    }
}
