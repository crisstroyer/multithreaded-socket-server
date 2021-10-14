using System;
using System.Linq;
using System.Text;

namespace SocketServer.Class
{
    /// <summary>
    /// Cristian Garcia
    /// cgarcia@ariqmo.com
    /// https://github.com/crisstroyer
    /// Class in charge of the data conversion facilities for the communication frames between the client and the serve
    /// </summary>
    public static class Utils
    {
        public static float ToFloat(byte[] buffer) {
            return BitConverter.ToSingle(Utils.Reverse(buffer), 0);
        }

        /// <summary>
        /// Method in charge of converting a base 10 (decimal) value to its representation in base 16 (hexadecimal)
        /// </summary>
        /// <param name="dec"></param>
        /// <returns>Returns the complete hex string</returns>
        public static string ToHex(Int16 dec){
            return dec.ToString("X2");
        }

        /// <summary>
        /// Method in charge of converting an array of bytes to their hexadecimal representation in a string arrary
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string[] ToHexArray(byte[] buffer) {
            return BitConverter.ToString(buffer).Split('-');
        }

        /// <summary>
        /// Method in charge of converting a decimal value into its respective byte.
        /// </summary>
        /// <param name="value">value between 0 and 255, 2-byte Int16</param>
        /// <returns>Returns a Byte of representation of the decimal value</returns>
        public static byte ToByte(Int16 value){
            if (value < 0 || value > 255)
                throw new Exception("El valor en base 10 no puede superar 1 byte, se debe usar un rango de 0 a 255");

            return Convert.ToByte(value.ToString(), 10);
        }

        /// <summary>
        /// Method in charge of converting a hexadecimal value into its respective byte
        /// </summary>
        /// <param name="hex">one-byte hexadecimal value</param>
        /// <returns>Returns a Byte representing the hexadecimal sent</returns>
        public static byte ToByte(string hex){
            return Convert.ToByte(hex, 16);
        }

        /// <summary>
        /// Method in charge of converting an array of hex to its exact representation in bytes
        /// </summary>
        /// <param name="hex">string array with hex representation</param>
        /// <returns>Returns an Array of Bytes with the representation of the hex array sent</returns>
        public static byte[] ToByteArray(string[] hex) {
            byte[] result = hex.Select(i => Utils.ToByte(i)).ToArray();
            return result;
        }

        /// <summary>
        /// Method in charge of converting a value in base 10 (decimal) to its representation in bytes (decimal representation of each binary in an array of bytes)
        /// </summary>
        /// <param name="dec">Decimal value</param>
        /// <returns>Returns an Array of Bytes with the representation of the sent decimal</returns>
        public static byte[] ToByteArray(Int16 dec) {
            return BitConverter.GetBytes(dec);
        }

        /// <summary>
        /// Method in charge of converting a value in base 10 (decimal) to its representation in bytes (decimal representation of each binary in an array of bytes)
        /// </summary>
        /// <param name="dec">decimal value</param>
        /// <returns>Returns an Array of Bytes with the representation of the sent decimal</returns>
        public static byte[] ToByteArray(int dec){
            return BitConverter.GetBytes(dec);
        }

        /// <summary>
        /// Method in charge of converting a value in base 10 (floating point decimal) to its representation in bytes (decimal representation of each binary in an array of bytes)
        /// </summary>
        /// <param name="dec">decimal value</param>
        /// <returns>Returns an Array of Bytes with the representation of the sent decimal</returns>
        public static byte[] ToByteArray(float dec){
            return BitConverter.GetBytes(dec);
        }

        /// <summary>
        /// Method in charge of converting a decimal value to its hexadecimal representation
        /// Returns an array for each byte of the hexadecimal value
        /// </summary>
        /// <param name="dec"></param>
        /// <returns>Returns a string array with each byte represented with its corresponding hexadecimal</returns>
        public static string[] ToHexArray(Int16 dec) {
            return ToHexArray(ToByteArray(dec));
        }

        /// <summary>
        /// Method in charge of converting an array of bytes in its decimal representation
        /// </summary>
        /// <param name="buffer">Byte array</param>
        /// <returns>decimal representation of value</returns>
        public static uint ToDecimal(byte[] buffer) {
            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>
        /// Method in charge of converting a decimal to a Binary-Coded Decimal (BCD) or Binary-coded Decimal
        /// https://es.wikipedia.org/wiki/Decimal_codificado_en_binario
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="bytes">number of bytes that apply to the sent value, since int handles 4 bytes by default</param>
        /// <returns>Returns the representation in a byte array of the final value of the BCD</returns>
        public static byte[] GetBcd(long value, int bytes) {
            //We create the copy array, only 2 bytes will be handled taking into account that Int16 only handles 2 bytes
            byte[] ret = new byte[bytes];
            //We go through the possible bytes in the sent value
            for (int i = 0; i < bytes; i++)
            {
                ret[i] = (byte)(value % 10);
                value /= 10;
                ret[i] |= (byte)((value % 10) << 4);
                value /= 10;
            }
            //Return the value and invert the bytes
            return Reverse(ret);
        }

        /// <summary>
        /// Method in charge of converting an array of bytes in BCD format to its representation in decimal
        /// It must be taken into account that the sent array has bcd applied and its bytes are inverted
        /// See Utils.Reverse
        /// </summary>
        /// <param name="bcd">Byte array in BCD format</param>
        /// <returns></returns>
        public static int ToDecimalFromBcd(byte[] bcd) {            
            int v = 0;
            //Bytes are traversed in bcd format and converted to decimal format            
            foreach (byte b in bcd){
                int r = (b & 0x0F) + 10 * ((b >> 4) & 0x0F);
                v *= 100;
                v += r;
            }
            return v;
        }

        /// <summary>
        /// Method in charge of returning a decimal value from an array of bits in BCD format
        /// </summary>
        /// <param name="bcd"></param>
        /// <returns></returns>
        public static Int64 GetDecimal(byte[] bcd) {
            return Convert.ToInt64(String.Join("", Utils.ToHexArray(Utils.Reverse(bcd))), 16);
        }


        /// <summary>
        /// Method in charge of inverting the order of the bytes for their representation in the string
        /// as hex, since by default the remaining bytes remain at the end, causing them to remain as 0
        /// on the right.
        /// </summary>
        /// <param name="buffer">Byte array to invert</param>
        /// <returns>Inverted Bytes</returns>
        public static byte[] Reverse(byte[] buffer) {
            return buffer.Reverse().ToArray();
        }

        /// <summary>
        /// Method in charge of converting an array of bytes in its alphanumeric representation
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string ToString(byte[] buffer) {
            return System.Text.Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Method in charge of converting an alphanumeric string to a byte array
        /// </summary>
        /// <param name="value">string</param>
        /// <returns>Byte array with the representation of the sent string</returns>
        public static byte[] ToByteArray(string value) {
            return Encoding.Default.GetBytes(value);
        }
    }
}
