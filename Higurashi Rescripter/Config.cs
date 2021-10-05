using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Higurashi_Rescripter
{
    class Config
    {

        //
        // Public variables
        //

        public readonly Encoding TextEncoding = new UTF8Encoding(false);


        //
        // Private variables
        //

        ErrorHandler error;


        //
        // Gets/Sets
        //

        public String ConfigPath { get; set; }
        public String InputScriptFolder { get; set; }
        public String InputSheetFolder { get; set; }
        public String OutputScriptFolder { get; set; }
        public String OutputSheetFolder { get; set; }
        public Boolean AskConfirmationBeforeAction { get; set; }
        public Boolean ReplaceEscapeCharacter { get; set; }
        public Boolean IsRelativePath { get; set; }
        public Boolean IsInitialized { get; private set; }
        public Dictionary<String, String[]> FileMap { get; private set; }


        //
        // Constructor
        //

        public Config(ErrorHandler error)
        {
            this.error = error;

            ConfigPath = $@"{ExecutableLocation()}\config.cfg";

            InputScriptFolder = String.Empty;
            InputSheetFolder = String.Empty;
            OutputScriptFolder = String.Empty;
            OutputSheetFolder = String.Empty;

            AskConfirmationBeforeAction = false;
            ReplaceEscapeCharacter = false;
            IsRelativePath = false;

            IsInitialized = false;

            FileMap = new Dictionary<string, string[]>();

            Load();
        }


        //
        // Methods
        //

        public static String ExecutableLocation()
        {
            String executableLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            return executableLocation;
        }

        public void Load()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    error.NotifyError("[CFG-001] Unable to find config.cfg", 1);
                }

                // Read config lines and skip all comments
                String[] configLines = StringExtension.ReadAllLinesFromFile(ConfigPath, TextEncoding, StringSplitOptions.RemoveEmptyEntries).
                                            Where(x => !x.StartsWith("//")).ToArray();

                InputScriptFolder = ReadString("InputScriptFolder", configLines);
                InputSheetFolder = ReadString("InputSheetFolder", configLines);
                OutputScriptFolder = ReadString("OutputScriptFolder", configLines);
                OutputSheetFolder = ReadString("OutputSheetFolder", configLines);
                AskConfirmationBeforeAction = ReadBoolean("AskConfirmationBeforeAction", configLines);
                ReplaceEscapeCharacter = ReadBoolean("ReplaceEscapeCharacter", configLines);
                IsRelativePath = ReadBoolean("IsRelativePath", configLines);
                FileMap = ReadMap("FileMap", configLines);

                if (IsRelativePath)
                {
                    InputScriptFolder = $@"{ExecutableLocation()}\{InputScriptFolder}";
                    InputSheetFolder = $@"{ExecutableLocation()}\{InputSheetFolder}";
                    OutputScriptFolder = $@"{ExecutableLocation()}\{OutputScriptFolder}";
                    OutputSheetFolder = $@"{ExecutableLocation()}\{OutputSheetFolder}";
                }

                InputScriptFolder = Path.GetFullPath(InputScriptFolder);
                InputSheetFolder = Path.GetFullPath(InputSheetFolder);
                OutputScriptFolder = Path.GetFullPath(OutputScriptFolder);
                OutputSheetFolder = Path.GetFullPath(OutputSheetFolder);

                Directory.CreateDirectory(InputScriptFolder);
                Directory.CreateDirectory(InputSheetFolder);
                Directory.CreateDirectory(OutputScriptFolder);
                Directory.CreateDirectory(OutputSheetFolder);

                IsInitialized = true;
            }
            catch (Exception ex)
            {
                error.NotifyError($"[CFG-000] Unexpected error: {ex.Message}", 1);
            }
        }

        public void LoadDefaultValues()
        {
            InputScriptFolder = @"Episode_X\Input\Script";
            InputSheetFolder = @"Episode_X\Input\Sheet";
            OutputScriptFolder = @"Episode_X\Output\Script";
            OutputSheetFolder = @"Episode_X\Output\Sheet";
            AskConfirmationBeforeAction = true;
            ReplaceEscapeCharacter = true;
            IsRelativePath = true;
        }

        public void Save()
        {
            StringBuilder configSB = new StringBuilder();

            SaveValue(configSB, "InputScriptFolder", ConvertPathForSaving(InputScriptFolder), true);
            SaveValue(configSB, "InputSheetFolder", ConvertPathForSaving(InputSheetFolder));
            SaveValue(configSB, "OutputScriptFolder", ConvertPathForSaving(OutputScriptFolder));
            SaveValue(configSB, "OutputSheetFolder", ConvertPathForSaving(OutputSheetFolder));
            SaveValue(configSB, "AskConfirmationBeforeAction", AskConfirmationBeforeAction);
            SaveValue(configSB, "ReplaceEscapeCharacter", ReplaceEscapeCharacter);
            SaveValue(configSB, "IsRelativePath", IsRelativePath);

            File.WriteAllText(ConfigPath, configSB.ToString(), TextEncoding);
        }

        private String ReadString(String variableName, String[] configLines)
        {
            foreach (String s in configLines)
            {
                if (s.Length > variableName.Length + 2 && s.StartsWith(variableName + "="))
                {
                    return s.Substring(variableName.Length + 1);
                }
            }

            error.NotifyError($"[CFG-002] Unable to find the variable {variableName}", 1);

            return null;
        }

        private Boolean ReadBoolean(String variableName, String[] configLines)
        {
            String stringValue = ReadString(variableName, configLines);
            return Convert.ToBoolean(stringValue);
        }

        private Dictionary<String, String[]> ReadMap(String variableName, String[] configLines)
        {
            Dictionary<String, String[]> map = new Dictionary<String, String[]>();

            String s = ReadString(variableName, configLines);

            String[] sLines = s.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < sLines.Length; i++)
            {
                String[] lineArgs = sLines[i].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                String sheetName = lineArgs[0];
                List<String> fileNames = new List<String>();

                for (int j = 1; j < lineArgs.Length; j++)
                {
                    fileNames.Add(lineArgs[j]);
                }

                map.Add(sheetName, fileNames.ToArray());
            }

            return map;
        }

        private void SaveValue(StringBuilder sb, String name, Object value, Boolean firstValue = false)
        {
            if (!firstValue)
            {
                sb.Append('\n');
            }

            sb.Append(name);
            sb.Append('=');
            sb.Append(value);
        }

        private String ConvertPathForSaving(String path)
        {
            return IsRelativePath ? path.Substring(ExecutableLocation().Length) : path;
        }

    }
}
