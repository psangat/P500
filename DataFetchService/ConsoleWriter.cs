using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFetchService
{
    class ConsoleWriter : TextWriter
    {

        private TextWriter originalOut;

        public ConsoleWriter()
        {
            originalOut = Console.Out;
        }

        public override Encoding Encoding
        {
            get { return new System.Text.ASCIIEncoding(); }
        }

        public override void WriteLine(string message)
        {
            originalOut.WriteLine(String.Format("{0} {1}", DateTime.Now.ToString("[MM-dd-yyyy H:mm:ss]"), message));
        }

        public override void WriteLine()
        {
            base.WriteLine();
        }

        public override void Write(string message)
        {
            originalOut.Write(message);
        }
    }


}
