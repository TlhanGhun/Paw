using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Paw
{
    class HelperFunctions
    {
        static public string convertFileToWindowsPath(string path)
        {
            if(!path.ToLower().StartsWith("file")) {
                return path;
            } 
            else
            {
                path = path.Substring(6);
                path = path.Replace("\\\\", "\\");
                return path;
            }
        }

        static public string getPawDefaultIconPath()
        {
            return System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + "\\paw.ico";
        }

        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        static public string EncodeTo64(byte[] toEncode)
        {
            string returnValue
                  = System.Convert.ToBase64String(toEncode);
            return returnValue;
        }

        static public string DecodeFrom64ToString(string encodedData)
        {
            byte[] encodedDataAsBytes
                = System.Convert.FromBase64String(encodedData);
            string returnValue =
               System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }

        static public byte[] DecodeFrom64ToByteArray(string encodedData)
        {
            return System.Convert.FromBase64String(encodedData);
        }
    }


}
