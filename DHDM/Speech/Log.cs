using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.CodeRush.Platform.Diagnostics {
    public class Log
    {
        public static void SendException(Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        
        public static void SendError(string str)
        {
            Debug.WriteLine($"Error: {str}");
        }

        public static void SendException(string message, Exception ex)
        {
            Debug.WriteLine("!!!");
            Debug.WriteLine(message);
            Debug.WriteLine(ex.Message);
            Debug.WriteLine(ex.StackTrace);
            Debug.WriteLine("---");
            Debug.WriteLine("");
        }
    }
}
