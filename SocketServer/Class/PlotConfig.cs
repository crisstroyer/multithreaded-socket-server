using System.Collections.Generic;

namespace SocketServer.Class
{
    /// <summary>
    /// Cristian Garcia
    /// cgarcia@ariqmo.com
    /// https://github.com/crisstroyer
    /// </summary>
    public class PlotConfig
    {
        #region Variables
        private Dictionary<string, PlotField> fields;
        #endregion

        #region Properties
        /// <summary>
        /// Plot key
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// Frame size in bytes (Default in configuration)
        /// </summary>
        public int Length {
            get {
                int result = 0;
                foreach (KeyValuePair<string, PlotField> fieldRow in this.fields) {
                    result += ((PlotField)fieldRow.Value).length;
                }
                return result; //Subtract header size from 7 bytes
            }
        }
        #endregion

        #region Public Method

        /// <summary>
        /// Initialize the configuration class for the frames
        /// </summary>
        /// <param name="key">Plot command key</param>
        public PlotConfig(string key){
            this.key = key;
            this.fields = new Dictionary<string, PlotField>();
        }

        /// <summary>
        /// Add a row of the plot with its respective configuration
        /// </summary>
        /// <param name="plotField"></param>
        public void Add(PlotField plotField) {
            this.fields.Add(plotField.key, plotField);
        }

        /// <summary>
        /// Obtain a row of the plot with its respective configuration
        /// </summary>
        /// <param name="plotFieldKey">Plot row key, see PlotFieldKey</param>
        /// <returns></returns>
        public PlotField Get(string plotFieldKey) {
            return this.fields[plotFieldKey];
        }

        public Dictionary<string, PlotField> GetFields() {
            return this.fields;
        }
        
        #endregion
    }
}
