using System;
using System.IO;
using System.Text;

namespace Higurashi_Rescripter
{
    class StringExtension
    {

        //
        // Methods
        //

        public static String[] ReadAllLinesFromFile(String path, Encoding encoding, StringSplitOptions stringSplitOptions, Boolean removeCarriageReturn = false)
        {
            String textString = ReadAllTextFromFile(path, encoding);

            String[] textLines = ReadAllLines(textString, stringSplitOptions, removeCarriageReturn);

            return textLines;
        }

        public static String ReadAllTextFromFile(String path, Encoding encoding)
        {
            using (StreamReader sr = new StreamReader(path, encoding))
            {
                return sr.ReadToEnd();
            }
        }

        public static String[] ReadAllLines(String s, StringSplitOptions stringSplitOptions, Boolean removeCarriageReturn = false)
        {
            String[] textLines = s.Split(new char[] { '\n' }, stringSplitOptions);

            if (removeCarriageReturn)
            {
                for (int i = 0; i < textLines.Length; i++)
                {
                    if (textLines[i].Length > 0)
                    {
                        textLines[i] = RemoveLastMatchingChar(textLines[i], '\r');
                    }
                }
            }

            return textLines;
        }

        public static String RemoveLastMatchingChar(String value, char character)
        {
            if (value[value.Length - 1] == character)
            {
                value = value.Substring(0, value.Length - 1);
            }

            return value;
        }

        public static void AppendLineOnSB(StringBuilder sb, String line)
        {
            if (sb.Length > 0)
            {
                sb.Append('\n');
            }

            sb.Append(line);
        }

        public static String DecodeGoogleSheetString(String original)
        {
            // When copying the Google Sheets into a file the double quotation marks will be copied as single quotation mark
            return original.Replace("\"", "\\\""); ;
        }

        public static String EncodeGoogleSheetString(String original)
        {
            // Google Sheets formula requires double quotation mark to show a single quotation mark
            return original.Replace("\\\"", "\"\"");
        }

    }
}
