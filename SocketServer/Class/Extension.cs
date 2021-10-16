using System;

namespace SocketServer.Class
{
    /// <summary>
    /// Cristian Garcia
    /// cgarcia@ariqmo.com
    /// https://github.com/crisstroyer
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Extension method of the string class to convert a hex value to its representation in bytes
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToByte(this string str){
            if (str.Length > 2)
                throw new Exception("Only one byte representation is allowed for a hexadecimal");
            return Utils.ToByte(str);
        }        
    }
}
