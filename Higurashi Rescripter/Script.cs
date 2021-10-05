using System;
using System.IO;
using System.Text;

namespace Higurashi_Rescripter
{
    class Script
    {

        //
        // Gets/Sets
        //

        public String FileName { get; private set; }
        public String[] Lines { get; set; }
        public Encoding Encoding { get; private set; }


        //
        // Constructor
        //

        public Script(String path, Encoding encoding)
        {
            FileName = Path.GetFileName(path);
            Encoding = encoding;

            Lines = StringExtension.ReadAllLinesFromFile(path, Encoding, StringSplitOptions.None);
        }


        //
        // Methods
        //

        // Save the current scripts lines
        public void Save(String path)
        {
            StringBuilder outputFile = new StringBuilder(Lines[0], Lines.Length);

            for (int i = 1; i < Lines.Length; i++)
            {
                outputFile.Append('\n');
                outputFile.Append(Lines[i]);
            }

            File.WriteAllText($@"{path}\{FileName}", outputFile.ToString(), Encoding);
        }

    }
}
