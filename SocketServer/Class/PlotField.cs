namespace SocketServer.Class
{
    public enum PlotFieldType {
        Byte,
        BCD,
        ALFANUMERIC,
        Float
    }

    /// <summary>
    /// Cristian Garcia
    /// cgarcia@ariqmo.com
    /// https://github.com/crisstroyer
    /// Class in charge of managing the configuration of each row of the existing frames
    /// </summary>
    public class PlotField
    {
        #region Variables
        #endregion
        #region Properties
        /// <summary>
        /// Hatch row key
        /// </summary>
        public string key{ get; set; }

        /// <summary>
        /// plot row size in bytes
        /// </summary>
        public int length { get; set; }

        /// <summary>
        /// Data type of the raster row
        /// </summary>
        public PlotFieldType Type { get; set; }

        /// <summary>
        /// Description of the plot row
        /// </summary>
        public string Description { get; set; }


        #endregion
        #region Metodos Publicos
        public PlotField(string key, string descripcion, PlotFieldType type, int length){
            this.key = key;
            this.Description = descripcion;
            this.Type = type;
            this.length = length;
        }
        #endregion
        #region Metodos Privados
        #endregion

    }
}
