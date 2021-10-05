using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Higurashi_Rescripter
{
    public class DocumentCache
    {
        public String FileName { get; set; }
        public String[] FileContentSpl { get; set; }

        public DocumentCache(String path, Encoding encoding)
        {
            FileName = Path.GetFileName(path);

            FileContentSpl = File.ReadAllText(path, encoding).Split(new[] { '\n' }, StringSplitOptions.None);

            for (int i = 0; i < FileContentSpl.Length; i++)
            {
                FileContentSpl[i] = FileContentSpl[i].TrimEnd(new[] { '\r' });
            }
        }

        public void Save(String path, Encoding encoding)
        {
            StringBuilder outputFile = new StringBuilder();

            for (int i = 0; i < FileContentSpl.Length - 1; i++)
            {
                outputFile.Append(FileContentSpl[i] + "\n");
            }

            outputFile.Append(FileContentSpl[FileContentSpl.Length - 1]);

            File.WriteAllText(path + "\\" + FileName, outputFile.ToString(), encoding);
        }
    }
}
