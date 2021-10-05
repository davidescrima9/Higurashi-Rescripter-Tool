using System;
using System.IO;
using System.Text;

namespace Higurashi_Rescripter
{
    class ErrorHandler
    {

        //
        // Private variables
        //

        private StringBuilder sb;


        //
        // Constructor
        //

        public ErrorHandler()
        {
            sb = new StringBuilder();
        }


        //
        // Methods
        //

        public void NotifyError(String message, int level)
        {
            sb.Append(DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss] "));
            sb.Append("[");
            sb.Append(level);
            sb.Append("] ");
            sb.Append(message);
            sb.Append('\n');

            if (level == 1)
            {
                throw new Exception(message);
            }
        }

        public void SaveLog()
        {
            String outputFile = $@"{Config.ExecutableLocation()}\log.txt";

            File.AppendAllText(outputFile, sb.ToString(), new UTF8Encoding(false));

            ClearLog();
        }

        public void ClearLog()
        {
            sb = new StringBuilder();
        }

    }
}
