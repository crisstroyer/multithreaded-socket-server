using System;
using System.Collections.Generic;
using System.Linq;

namespace SocketServer.Class
{
    /// <summary>
    /// Cristian Garcia
    /// cgarcia@ariqmo.com
    /// https://github.com/crisstroyer
    /// Class in charge of managing the high-level frames that allow communication between the client and the server
    /// </summary>
    public class Plot
    {
        #region Variables
        Dictionary<string, PlotFieldValue> field;
        private byte[] _buffer;
        #endregion

        #region Public Method
        /// <summary>
        /// Initialize a frame with an array of bytes with the representation of the frame sent
        /// </summary>
        /// <param name="buffer">Array de bytes enviado entre Cliente-Servidor</param>
        public Plot(byte[] buffer) {
            field = new Dictionary<string, PlotFieldValue>();
            _buffer = buffer;
            //Leer y decodificar el buffer enviado en el array de bytes
            ParseHead(buffer, null);
        }

        /// <summary>
        /// Inicializa la trama vacia segun key enviado
        /// Se utiliza para crear una trama de envio con todas sus filas vacias
        /// </summary>
        /// <param name="Key"></param>
        public Plot(string Key) {
            field = new Dictionary<string, PlotFieldValue>();
            //Crear un buffer con el tamaño configurado de la trama
            _buffer = new byte[(State.plotConfigList[Key].Length + 20)]; //se le suma 20 del encabezado estandar
            //Leer y decodificar el buffer enviado en el array de bytes
            ParseHead(_buffer, Key);            
            //Asignar el tamaño por defecto de la trama
            this.Set(PlotFieldKey.MessageLength, (State.plotConfigList[Key].Length + 20)); //Sumar el encabezado estandar 20 bytes
        }

        /// <summary>
        /// Method in charge of assigning a value to a field of the frame according to the key sent
        /// </summary>
        /// <param name="key">plot field key</param>
        /// <param name="value">value</param>
        public void Set(string key, object value) {
            byte[] result = null;

            //retrieve the value of the current frame
            PlotFieldValue valueField = this.field[key.ToString()];
            //Assign the value according to the type
            switch (valueField.PlotField.Type)
            {
                case PlotFieldType.BCD:
                    result = Utils.GetBcd((long)Convert.ToDecimal(value), valueField.PlotField.length);
                    break;
                case PlotFieldType.Float:
                    result = Utils.Reverse(Utils.ToByteArray((float)value)); // Utils.ToByteArray((float)value);
                    break;
                case PlotFieldType.ALFANUMERIC:
                    result = Utils.ToByteArray((string)value);
                    break;
                case PlotFieldType.Byte:
                    //If it is string convert it to byte
                    if (value.GetType().Equals("System.String"))
                        value = Utils.ToByte((string)value);

                    result = new byte[] { (byte)value };
                    break;
            }
            valueField.Value = result;
        }
        
        public object Get(string key) {
            object result = null;
            //Retrieve the value of the submitted key
            PlotFieldValue valueField = this.field[key.ToString()];

            //Return the value of the key according to its type
            switch (valueField.PlotField.Type) {
                case PlotFieldType.BCD:
                    //result = Utils.GetDecimal(valueField.Value);
                    result = String.Join("", Utils.ToHexArray(valueField.Value));
                    break;
                case PlotFieldType.Float:
                    result = Utils.ToFloat(valueField.Value);
                    break;
                case PlotFieldType.ALFANUMERIC:
                case PlotFieldType.Byte:
                    result = Utils.ToString(valueField.Value);
                    break;
            }
            //Convert the bytes of the sent key to its decimal representation (base 10)
            return result;
        }

        public byte[] GetBytes(string key){
            return this.field[key.ToString()].Value;
        }

        /// <summary>
        /// Method in charge of returning the current byte array of the frame
        /// </summary>
        /// <returns>Array de bytes</returns>
        public byte[] ToByteArray() {
            //Final array default size
            byte[] result = new byte[1024];
            int index = 0;

            //Update frame date when generating the array
            this.Set(PlotFieldKey.TimeStamp, Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss")));

            //Step through the fields of the current frame and create the byte array with the complete frame
            foreach (KeyValuePair<string, PlotFieldValue> plotValue in this.field) {
                PlotFieldValue plotFieldValue = (PlotFieldValue)plotValue.Value;
                foreach (byte _byte in plotFieldValue.Value) {
                    result[index] = _byte;
                    index++;
                }
            }
            //Resize the final array with the actual bytes read
            Array.Resize(ref result, index);
            return result;
        }

        /// <summary>
        /// Method in charge of returning an array of bytes with the frame according to a frame key
        /// It is used only to return a part of the frame as the header
        /// </summary>
        /// <param name="plotKey"></param>
        /// <returns></returns>
        public byte[] ToByteArray(string plotKey) {
            byte[] result = new byte[1024];
            int index = 0;

            //Update frame date when generating the array
            this.Set(PlotFieldKey.TimeStamp, Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss")));

            //Assign the header size
            this.Set(PlotFieldKey.MessageLength, 20);
            //Cycle through the fields of the sent frame
            foreach (KeyValuePair<string, PlotField> fieldRow in State.plotConfigList[plotKey].GetFields()){
                //Traverse the bytes of the frame field
                foreach (byte _byte in this.field[fieldRow.Key].Value){
                    result[index] = _byte;
                    index++;
                }
            }
            //Redimensionar el array final con los bytes reales leidos
            Array.Resize(ref result, index);
            return result;
        }

        /// <summary>
        /// Method in charge of Copying the standard header from other frames
        /// </summary>
        /// <param name="plot">trama de origen</param>
        public void CopyHead(Plot plot) {
            //Cache current values ​​of the frame
            string currentCommand = (string)this.Get(PlotFieldKey.Command);

            //Go through the configured fields of the General Header frame (Includes header and standard header)
            foreach (KeyValuePair<string, PlotField> fieldRow in State.plotConfigList["EG"].GetFields()){
                //Asignar el nuevo valor desde la trama de origen a la trama actual                
                ((PlotFieldValue)this.field[fieldRow.Key]).Value = plot.field[fieldRow.Key].Value;
            }

            //Assign the default size of the frame
            this.Set(PlotFieldKey.MessageLength, (State.plotConfigList[currentCommand].Length + 20)); //Sumar el encabezado estandar 20 bytes
            //Assign current frame command (overwrite copy)
            this.Set(PlotFieldKey.Command, currentCommand);
        }

        /// <summary>
        /// Method in charge of returning a hexadecimal array with the information of the frame
        /// </summary>
        /// <returns></returns>
        public string[] ToHexArray() {
            return Utils.ToHexArray(this.ToByteArray());
        }

        public override string ToString() {
            return String.Join("", this.ToHexArray());
        }
        #endregion
        
        #region Private methods
        private void ParseHead(byte[] buffer, string Key) {
            int startIndex = 0;

            //Walk through the General Header settings
            foreach (KeyValuePair<string, PlotField> fieldRow in State.plotConfigList["EG"].GetFields()) {
                PlotField plotField = (PlotField)fieldRow.Value;
                //agregar el campo de la trama
                AddPlotField(buffer,  plotField, startIndex);
                startIndex += plotField.length;
            }

            //Assign the command of the current frame according to the key sent
            if (Key != null) this.Set(PlotFieldKey.Command, Key);

            //Pending to refactor
            string commandPlot = (string)this.Get(PlotFieldKey.Command);
            //If the plot sent has any details
            if (State.plotConfigList.ContainsKey(commandPlot)){
                foreach (KeyValuePair<string, PlotField> fieldRow in State.plotConfigList[commandPlot].GetFields()){
                    PlotField plotField = (PlotField)fieldRow.Value;
                    //add the plot field
                    AddPlotField(buffer, plotField, startIndex);
                    startIndex += plotField.length;
                }
            }
        }

        /// <summary>
        /// Method in charge of adding the bytes belonging to the field of the frame according to their size in bytes
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="plotField"></param>
        /// <param name="index"></param>
        private void AddPlotField(byte[] buffer, PlotField plotField, int index) {
            //The value and configuration of said field is added in the frame
            field.Add(plotField.key, new PlotFieldValue() { Value = buffer.Skip(index).Take(plotField.length).ToArray(), PlotField = plotField });
        }

        #endregion
    }
}
