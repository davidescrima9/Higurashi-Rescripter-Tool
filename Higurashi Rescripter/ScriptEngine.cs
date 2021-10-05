using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Higurashi_Rescripter
{
    class ScriptEngine
    {

        //
        // Private variables
        //

        private Config config;
        private ErrorHandler error;

        private const String colorFormula = "=IF(INDIRECT(ADDRESS(ROW() - 1; 6))=INDIRECT(ADDRESS(ROW(); 6));INDIRECT(ADDRESS(ROW() - 1; 8));IF(INDIRECT(ADDRESS(ROW() - 1; 8))=0; 1;0))";


        //
        // Constructor
        //

        public ScriptEngine(Config config, ErrorHandler error)
        {
            this.config = config;
            this.error = error;
        }


        //
        // Methods
        //

        // Generate sheets
        public void GenerateSheets()
        {
            try
            {
                ConvertScriptsToStringAndProcessNames(out StringBuilder scriptSB, out int censorIndex);

                ConvertScriptsStringToSheets(scriptSB, censorIndex);
            }
            catch (Exception ex)
            {
                error.NotifyError($"[SHT0-000] Unexpected error: {ex.Message}", 1);
            }
        }

        // Convert scripts to almost-processed-sheets and process&save names
        private void ConvertScriptsToStringAndProcessNames(out StringBuilder scriptSB, out int censorIndex)
        {
            // Store the processed names
            StringBuilder namesSB = new StringBuilder();

            // Store the processed scripts
            scriptSB = new StringBuilder();

            // Store data for separating censored and uncensored files
            int processedScriptLinesCount = 0;
            censorIndex = -1;

            // Get script paths and order them by censorship, then by name
            String[] scriptsToProcess = Directory.GetFiles(config.InputScriptFolder, "*.txt", SearchOption.TopDirectoryOnly).
                                            OrderBy(x => new Regex("_vm[0x]{2,2}_n[01]{2,2}.txt$").IsMatch(x) ? 1 : 0).ThenBy(x => x).ToArray();

            // Store unique names
            HashSet<String> namesSet = new HashSet<String>();

            //
            // Process the scripts
            //
            for (int i = 0; i < scriptsToProcess.Length; i++)
            {
                // Script data
                String currentScript = scriptsToProcess[i];
                String scriptFileName = Path.GetFileName(currentScript);

                // Load the script lines
                String[] scriptLines = StringExtension.ReadAllLinesFromFile(currentScript, config.TextEncoding, StringSplitOptions.None, true);

                // Temp variables to store data that involves multiple lines
                String nameTemp = String.Empty;
                String censorFunctionTemp = String.Empty;

                // Process the script lines
                for (int j = 0; j < scriptLines.Length; j++)
                {
                    // Store current line
                    String currentScriptLine = scriptLines[j];

                    if (new Regex($"^[ \t]*//").IsMatch(currentScriptLine))
                    {
                        //
                        // Detected comment
                        //
                        continue;
                    }
                    else if (new Regex(@"^[ \t]*if \(GetGlobalFlag\(GADVMode\)\)").IsMatch(currentScriptLine))
                    {
                        if (currentScriptLine.Contains("ClearMessage();") || currentScriptLine.Contains("OutputLineAll(\"\"")
                                   || (currentScriptLine.Contains("OutputLine(NULL") && !currentScriptLine.Contains("<color="))
                                   || currentScriptLine.Contains("OutputLineAll(NULL"))
                        {
                            //
                            // Detected function clears the current name
                            //
                            nameTemp = String.Empty;
                        }
                        else if (currentScriptLine.Contains("<color="))
                        {
                            //
                            // Detected a name to parse
                            //

                            // Parse any name that matches the regex
                            Regex nameRegex = new Regex(@"<color=[^>]+>(?<name>[^<]+)<\/color>");
                            MatchCollection nameMatches = nameRegex.Matches(currentScriptLine);

                            // If there are english and japaneses names (even number), proceed to extract them
                            if (nameMatches.Count % 2 == 0)
                            {
                                // Count of japanese names in the current line
                                int japNamesCount = nameMatches.Count / 2;

                                // Clear currentActor to let it store multiple names, if available
                                nameTemp = String.Empty;

                                // Iterate half array (just japanese names)
                                for (int k = 0; k < japNamesCount; k++)
                                {
                                    // Get the japanese and english names
                                    String japName = nameMatches[k].Groups["name"].Value;
                                    String engName = nameMatches[k + japNamesCount].Groups["name"].Value;

                                    // Composing the currentActor name
                                    nameTemp += engName + (k < japNamesCount - 1 ? " & " : String.Empty);

                                    // Add the name to the sheet if not present
                                    if (namesSet.Add(engName))
                                    {
                                        StringExtension.AppendLineOnSB(namesSB, $"{japName}\t{engName}\t");
                                    }
                                }
                            }
                            else if (nameMatches.Count > 0)
                            {
                                if (nameMatches.Count == 1)
                                {
                                    //
                                    // If the regex found just one name, check if it's part of the 
                                    // name set, and if it's true set nameTemp to it
                                    //
                                    String nameToCheck = nameMatches[0].Groups["name"].Value;

                                    if (namesSet.Contains(nameToCheck))
                                    {
                                        nameTemp = nameToCheck;
                                    }
                                }
                                else
                                {
                                    error.NotifyError("[SHT1-001] Names count can't be odd", 1);
                                }
                            }
                        }
                        else
                        {
                            error.NotifyError("[SHT1-002] Unexpected content", 1);
                        }
                    }
                    else if (new Regex(@"^[ \t]*OutputLine\(").IsMatch(currentScriptLine))
                    {
                        //
                        // Detected a dialogue
                        //

                        // Next line data
                        String nextScriptLine = scriptLines[j + 1];

                        // Regex data
                        Regex lineRegex = new Regex(@"^[^""]+, ""(?<line>.+)""[^""]+$");

                        // Regex result
                        String jap = lineRegex.Match(currentScriptLine).Groups["line"].Value;
                        String eng = lineRegex.Match(nextScriptLine).Groups["line"].Value;

                        // Error checks
                        if (jap.Length == 0 && eng.Length == 0)
                        {
                            error.NotifyError("[SHT1-003] Parsed lines can't be both null", 1);
                        }
                        else if (nextScriptLine.Contains("Line_Normal);") && !nextScriptLine.EndsWith("Line_Normal);"))
                        {
                            error.NotifyError("[SHT1-004] Unexpected Line_Normal position", 1);
                        }

                        // Escape characters for Google Sheet
                        if (config.ReplaceEscapeCharacter)
                        {
                            eng = StringExtension.EncodeGoogleSheetString(eng);
                            jap = StringExtension.EncodeGoogleSheetString(jap);
                        }

                        // Set censorIndex if it is not set and the current script file is censored
                        if (censorIndex == -1 && new Regex("_vm[0x]{2,2}_n[01]{2,2}.txt$").IsMatch(scriptFileName))
                        {
                            censorIndex = processedScriptLinesCount;
                        }

                        // Compose output
                        String tag = currentScript.Contains("_vm00_") ? "Original" : currentScript.Contains("_vm0x_") ? "Censored" : String.Empty;
                        String parseMode = "D";
                        String sheetLine = $"{nameTemp}\t=\"{jap}\"\t=\"{eng}\"\t\t{tag}\t{scriptFileName}\t{j}\t{colorFormula}\t{censorFunctionTemp}\t{parseMode}\t";

                        // Save the line
                        StringExtension.AppendLineOnSB(scriptSB, sheetLine);

                        // Detect parameters to clear current name
                        if (nextScriptLine.EndsWith("Line_Normal);"))
                        {
                            nameTemp = String.Empty;
                        }

                        // Increase variable for censor detection purpose
                        processedScriptLinesCount++;

                        // Increase j because 2 lines have been processed, not just one
                        j++;
                    }
                    else if (new Regex("^[ \t]*void dialog").IsMatch(currentScriptLine))
                    {
                        //
                        // Detected begin of a censored function
                        //

                        // Regex data
                        Regex functionRegex = new Regex(@"^[ \t]*void (?<function>[A-z]+[0-9]{2,3})\(\)");
                        String functionName = functionRegex.Match(currentScriptLine).Groups["function"].Value;

                        // Error checks
                        if (functionName.Length == 0)
                        {
                            error.NotifyError("[SHT1-005] Censor function can't be null", 1);
                        }

                        // Set the censor function
                        censorFunctionTemp = functionName;
                    }
                    else if (currentScriptLine.StartsWith("}"))
                    {
                        //
                        // Detected end of a censored function
                        //
                        censorFunctionTemp = String.Empty;
                    }
                    else if (currentScriptLine.Contains("GetGlobalFlag(GCensor)") && currentScriptLine.Contains("ModCallScriptSection"))
                    {
                        //
                        // Detected a call to a censored function
                        //

                        // Regex data
                        Regex censorRegex = new Regex(@"\(""(?<file>[A-z0-9_]+)"",""(?<function>[A-z]+[0-9]{2,3})""\)");
                        Match censorMatch = censorRegex.Match(currentScriptLine);

                        // Regex result
                        String censorFile = censorMatch.Groups["file"].Value;
                        String censorFunction = censorMatch.Groups["function"].Value;

                        // Error checks
                        if (censorFile.Length == 0)
                        {
                            error.NotifyError("[SHT1-006] Parsed censor file can't be null", 1);
                        }
                        else if (censorFunction.Length == 0)
                        {
                            error.NotifyError("[SHT1-007] Parsed censor function can't be null", 1);
                        }

                        // Compose output
                        String censorOutput = $"{censorFile},{censorFunction}";
                        String censorLine = String.Format($"\t\t\t\t\t\t\t\t\t\t{censorOutput}");

                        // Save the line
                        StringExtension.AppendLineOnSB(scriptSB, censorLine);

                        // Increase variable for censor detection purpose
                        processedScriptLinesCount++;
                    }
                    else if (new Regex(@"^[ \t]*char [A-z0-9_]+\[[0-9]+\];").IsMatch(currentScriptLine))
                    {
                        //
                        // Detected a Choice
                        //

                        // Choice array data
                        String arrayLine = currentScriptLine;
                        Regex arrayCountRegex = new Regex(@"^[ \t]*char [A-z0-9_]+\[(?<arrayCount>[0-9])+\];");
                        Match arrayCountMatch = arrayCountRegex.Match(arrayLine);
                        String arrayCountString = arrayCountMatch.Groups["arrayCount"].Value;
                        int arrayCount = Convert.ToInt32(arrayCountString);

                        // Error check
                        if (arrayCount <= 0)
                        {
                            error.NotifyError("[SHT1-008] arrayCount can't be less or equal to zero", 1);
                        }

                        // Jump to first choice
                        while(!new Regex(@"[ \t]*[A-z0-9_]+\[0\] = ""(?<choice>.+)""").IsMatch(scriptLines[j]))
                        {
                            j++;
                        }

                        // Calculate delta for japanese choice
                        int japChoiceDelta = 1;
                        while (!new Regex(@"[ \t]*[A-z0-9_]+\[0\] = ""(?<choice>.+)""").IsMatch(scriptLines[j + arrayCount + japChoiceDelta]))
                        {
                            japChoiceDelta++;
                        }

                        for (int k = 0; k < arrayCount; k++)
                        {
                            // Choice data
                            String choiceEngLine = scriptLines[j];
                            String choiceJapLine = scriptLines[j + arrayCount + japChoiceDelta];

                            // Choice regex data
                            Regex choiceRegex = new Regex(@"[ \t]*[A-z0-9_]+\[[0-9]+\] = ""(?<choice>.+)""");
                            Match choiceEngMatch = choiceRegex.Match(choiceEngLine);
                            Match choiceJapMatch = choiceRegex.Match(choiceJapLine);

                            // Choice regex result
                            String choiceEng = choiceEngMatch.Groups["choice"].Value;
                            String choiceJap = choiceJapMatch.Groups["choice"].Value;

                            // Error checks
                            if (choiceEng.Length == 0)
                            {
                                error.NotifyError("[SHT1-009] Choice (Eng) can't be null", 1);
                            }

                            if (choiceJap.Length == 0)
                            {
                                error.NotifyError("[SHT1-010] Choice (Eng) can't be null", 1);
                            }

                            // Compose output
                            String parseMode = "C";
                            String tag = "Choice";
                            String dataSheetLine = $"{nameTemp}\t=\"{choiceJap}\"\t=\"{choiceEng}\"\t\t{tag}\t{scriptFileName}\t{j}\t{colorFormula}\t{censorFunctionTemp}\t{parseMode}\t";
                            
                            // Save the lines
                            StringExtension.AppendLineOnSB(scriptSB, dataSheetLine);

                            // Increase variable for censor detection purpose
                            processedScriptLinesCount++;

                            // Go to next choice
                            j++;
                        }

                        j += arrayCount + 2;
                    }
                    else if (new Regex(@"^[ \t]*SavePoint\(""").IsMatch(currentScriptLine))
                    {
                        //
                        // Detected a SavePoint
                        //

                        // Regex data
                        Regex savePointRegex = new Regex(@"[ \t]*SavePoint\(""(?<savejp>.+)"", ""(?<saveen>.+)""\);");
                        Match savePointMatch = savePointRegex.Match(currentScriptLine);

                        // Regex result
                        String saveJap = savePointMatch.Groups["savejp"].Value;
                        String saveEng = savePointMatch.Groups["saveen"].Value;

                        // Error checks
                        if (saveJap.Length == 0)
                        {
                            error.NotifyError("[SHT1-011] SavePoint (Jap) can't be null", 1);
                        }
                        else if (saveEng.Length == 0)
                        {
                            error.NotifyError("[SHT1-012] SavePoint (Eng) can't be null", 1);
                        }

                        // Compose output
                        String parseMode = "S";
                        String savePointSheetLine = $"{nameTemp}\t=\"{saveJap}\"\t=\"{saveEng}\"\t\t\t{scriptFileName}\t{j}\t{colorFormula}\t{censorFunctionTemp}\t{parseMode}\t";

                        // Save the lines
                        StringExtension.AppendLineOnSB(scriptSB, savePointSheetLine);

                        // Increase variable for censor detection purpose
                        processedScriptLinesCount++;
                    }
                }
            }

            // Save the Names sheet
            File.WriteAllText($@"{config.OutputSheetFolder}\Names.txt", namesSB.ToString(), config.TextEncoding);
        }

        // Convert the almost-processed-sheets into full-fledged-sheets rearranging the censored/uncensored lines
        private void ConvertScriptsStringToSheets(StringBuilder scriptSB, int censorIndex)
        {
            // Store the processed scripts
            StringBuilder processedScriptSB = new StringBuilder();

            // Get script lines
            String[] scriptLines = StringExtension.ReadAllLines(scriptSB.ToString(), StringSplitOptions.None);

            // Process script lines
            for (int i = 0; i < censorIndex; i++)
            {
                // Script line data
                String currentScriptLine = scriptLines[i];
                String[] currentScriptLineArgs = currentScriptLine.Split(new char[] { '\t' }, StringSplitOptions.None);

                if (currentScriptLineArgs[10].Length > 0)
                {
                    //
                    // Detected script to import
                    //

                    // Get data from the line
                    String[] censorArgs = currentScriptLineArgs[10].Split(new char[] { ',' }, StringSplitOptions.None);

                    // Processed data
                    String censorFile = censorArgs[0];
                    String censorFunction = censorArgs[1];

                    // Find script to import
                    for (int j = censorIndex; j < scriptLines.Length; j++)
                    {
                        // Import line data
                        String importLine = scriptLines[j];

                        // Get data from the import line
                        String[] importLineArgs = importLine.Split(new char[] { '\t' }, StringSplitOptions.None);

                        // Processed import data
                        String importCensorFile = importLineArgs[5];
                        String importCensorFunction = importLineArgs[8];

                        if (importLineArgs[5] == $"{censorFile}.txt" && importLineArgs[8] == censorFunction)
                        {
                            // Import script line
                            StringExtension.AppendLineOnSB(processedScriptSB, importLine);
                        }
                    }
                }
                else
                {
                    // No script to import, proceed to save the current line
                    StringExtension.AppendLineOnSB(processedScriptSB, currentScriptLine);
                }
            }

            // Get processed script lines
            String[] processedScriptLines = StringExtension.ReadAllLines(processedScriptSB.ToString(), StringSplitOptions.None);

            // Store script name and script content in a map
            Dictionary<String, StringBuilder> scriptMap = new Dictionary<String, StringBuilder>(config.FileMap.Keys.Count);

            // Fill the map
            foreach (String key in config.FileMap.Keys)
            {
                scriptMap.Add(key, new StringBuilder());
            }

            // Import the script lines in the map
            for (int i = 0; i < processedScriptLines.Length; i++)
            {
                // Line data
                String currentLine = processedScriptLines[i];

                // Get data from line
                String[] currentLineArgs = currentLine.Split(new char[] { '\t' }, StringSplitOptions.None);

                // Processed data
                String actor = currentLineArgs[0];
                String jap = currentLineArgs[1];
                String eng = currentLineArgs[2];
                String tag = currentLineArgs[4];
                String fileName = currentLineArgs[5];
                String lineIndex = currentLineArgs[6];
                String colorFormula = currentLineArgs[7];
                String parseMode = currentLineArgs[9];

                // Find the sheet name
                String fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                String currentSheet = config.FileMap.Keys.FirstOrDefault(x => config.FileMap[x].Contains(fileNameWithoutExtension));

                // Error check
                if (currentSheet == null)
                {
                    error.NotifyError("[SHT2-001] Sheet can't be null", 1);
                }

                // Process output line
                String outputLine = $"{actor}\t{jap}\t{eng}\t\t{tag}\t{fileName}\t{lineIndex}\t{colorFormula}\t{parseMode}";

                // Import the line in the current sheet
                StringExtension.AppendLineOnSB(scriptMap[currentSheet], outputLine);
            }

            // Save the processed sheets
            foreach (String key in scriptMap.Keys)
            {
                File.WriteAllText($@"{config.OutputSheetFolder}\{key}.txt", scriptMap[key].ToString(), config.TextEncoding);
            }
        }

        // Generate scripts
        public void GenerateScripts()
        {
            try
            {
                //
                // Load the input sheets and merge them into one string
                //
                StringBuilder sheetsMergedSB = new StringBuilder();
                String[] inputSheetFiles = Directory.GetFiles(config.InputSheetFolder, "*.txt", SearchOption.TopDirectoryOnly).
                                                Where(x => !x.EndsWith(@"\Names.txt")).ToArray();

                for (int i = 0; i < inputSheetFiles.Length; i++)
                {
                    String sheetContent = StringExtension.ReadAllTextFromFile(inputSheetFiles[i], config.TextEncoding);

                    // Skip the header line
                    sheetContent = sheetContent.Substring(sheetContent.IndexOf("\n") + 1);

                    StringExtension.AppendLineOnSB(sheetsMergedSB, sheetContent);
                }

                // Remove untranslated lines, and sort by file, then by index
                String[] sheetLines = StringExtension.ReadAllLines(sheetsMergedSB.ToString(), StringSplitOptions.None, true).
                                                Where(x => x.Split(new char[] { '\t' }, StringSplitOptions.None)[3].Length > 0).
                                                OrderBy(x => x.Split(new char[] { '\t' }, StringSplitOptions.None)[5]).
                                                ThenBy(x => Convert.ToInt32(x.Split(new char[] { '\t' }, StringSplitOptions.None)[6])).ToArray();

                // Store the script name and its content
                Dictionary<String, Script> scriptMap = new Dictionary<String, Script>();

                // Translate dialogue lines
                for (int i = 0; i < sheetLines.Length; i++)
                {
                    // Get data from line
                    String[] sheetLineArgs = sheetLines[i].Split(new char[] { '\t' }, StringSplitOptions.None);

                    // Processed data
                    String jap = sheetLineArgs[1];
                    String eng = sheetLineArgs[2];
                    String ita = sheetLineArgs[3];
                    String fileName = sheetLineArgs[5];
                    int lineIndex = Convert.ToInt32(sheetLineArgs[6]);
                    String parseMode = sheetLineArgs[8];

                    // Decode dialogues
                    if (config.ReplaceEscapeCharacter)
                    {
                        jap = StringExtension.DecodeGoogleSheetString(jap);
                        eng = StringExtension.DecodeGoogleSheetString(eng);
                        ita = StringExtension.DecodeGoogleSheetString(ita);
                    }

                    // Load the script to the memory
                    if (!scriptMap.ContainsKey(fileName))
                    {
                        scriptMap.Add(fileName, new Script($@"{config.InputScriptFolder}\{fileName}", config.TextEncoding));
                    }

                    // Current script
                    Script currentScript = scriptMap[fileName];

                    if (parseMode == "D")
                    {
                        //
                        // Parse Dialogue
                        //

                        // Script data
                        String currentScriptLine = currentScript.Lines[lineIndex];
                        String nextScriptLine = currentScript.Lines[lineIndex + 1];

                        // Error checks
                        if (!new Regex(@"^[ \t]*OutputLine").IsMatch(currentScriptLine))
                        {
                            error.NotifyError("[SCR-001] Unexpected OutputLinePosition", 1);
                        }
                        else if (!currentScriptLine.Contains($"\"{jap}\""))
                        {
                            error.NotifyError("[SCR-002] Unable to find japanese line", 1);
                        }
                        else if (!nextScriptLine.Contains($"\"{eng}\""))
                        {
                            error.NotifyError("[SCR-003] Unable to find english line", 1);
                        }

                        // Translate the dialogue
                        Regex translateLineRegex = new Regex(@"^(?<begin>[^""]+, "").+(?<end>""[^""]+)$");
                        String translatedLine = translateLineRegex.Replace(nextScriptLine, $"${{begin}}{ita}${{end}}");

                        // Save the translated dialogue
                        currentScript.Lines[lineIndex + 1] = translatedLine;

                    }
                    else if (parseMode == "C")
                    {
                        //
                        // Parse Choice
                        //

                        // Script data
                        String itemEngLine = currentScript.Lines[lineIndex];

                        // Error checks
                        if (!new Regex($@"^[ \t]*[A-z0-9_]+\[[0-9]+\] = ""{eng}"";$").IsMatch(itemEngLine))
                        {
                            error.NotifyError("[SCR-004] Unable to find Choice", 1);
                        }

                        // Translate the Item
                        Regex translateLineRegex = new Regex(@"^(?<begin>[ \t]*[A-z0-9_]+\[[0-9]+\] = "").+(?<end>"";)$");
                        String translatedLine = translateLineRegex.Replace(itemEngLine, $"${{begin}}{ita}${{end}}");

                        // Save the translated Item
                        currentScript.Lines[lineIndex] = translatedLine;

                    }
                    else if (parseMode == "S")
                    {
                        //
                        // Parse SavePoint Description
                        //

                        // Script data
                        String currentScriptLine = currentScript.Lines[lineIndex];

                        // Error checks
                        if (!new Regex(@"^[ \t]*SavePoint").IsMatch(currentScriptLine))
                        {
                            error.NotifyError("[SCR-005] Unexpected OutputLinePosition", 1);
                        }
                        else if (!currentScriptLine.Contains($"SavePoint(\"{jap}\", \"{eng}\");"))
                        {
                            error.NotifyError("[SCR-006] Unable to find japanese line", 1);
                        }

                        // Translate the SavePoint
                        Regex translateLineRegex = new Regex(@"^(?<begin>[ \t]*SavePoint\("".+"", "").+(?<end>""\);)$");
                        String translatedLine = translateLineRegex.Replace(currentScriptLine, $"${{begin}}{ita}${{end}}");

                        // Save the translated SavePoint
                        currentScript.Lines[lineIndex] = translatedLine;
                    }
                    else
                    {
                        error.NotifyError("[SCR-007] parseMode not valid", 1);
                    }
                }


                // Translate the names
                if (scriptMap.Count > 0 && File.Exists($@"{config.InputSheetFolder}\Names.txt"))
                {
                    // Name sheet data
                    String[] nameLines = StringExtension.ReadAllLinesFromFile($@"{config.InputSheetFolder}\Names.txt", config.TextEncoding, StringSplitOptions.None, true);

                    // Store all the names to replace into a map
                    Dictionary<String, String> namesMap = new Dictionary<String, String>();

                    // Start from 1 to skip the header
                    for (int i = 1; i < nameLines.Length; i++)
                    {
                        // Get sheet data
                        String[] nameLineArgs = nameLines[i].Split(new char[] { '\t' }, StringSplitOptions.None);

                        // Processed data
                        String engName = nameLineArgs[1];
                        String itaName = nameLineArgs[2];

                        // Store the name in the map if it's valid
                        if (engName.Length > 0 && itaName.Length > 0 && engName != itaName && !namesMap.ContainsKey(engName))
                        {
                            namesMap.Add(engName, itaName);
                        }
                    }

                    // Replace the english name with the italian name
                    if (namesMap.Count > 0)
                    {
                        foreach (var item in scriptMap)
                        {
                            Script currentScript = item.Value;

                            for (int i = 0; i < currentScript.Lines.Length; i++)
                            {
                                String currentLine = currentScript.Lines[i];

                                if (currentLine.StartsWith("\tif (GetGlobalFlag(GADVMode))") && currentLine.Contains("<color="))
                                {
                                    foreach (var name in namesMap)
                                    {
                                        if (currentLine.Contains($">{name.Key}</color>"))
                                        {
                                            currentScript.Lines[i] = currentLine.Replace($">{name.Key}</color>", $">{name.Value}</color>");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Save the scripts
                foreach (var item in scriptMap)
                {
                    item.Value.Save(config.OutputScriptFolder);
                }
            }
            catch (Exception ex)
            {
                error.NotifyError($"[SCR-000] Unexpected error: {ex.Message}", 1);
            }
        }

    }
}
