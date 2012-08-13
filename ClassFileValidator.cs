using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace FlatFileValidator
{
    class ClassFileValidator
    {
        private string filePath; // Stores the path of the file that needs to be cleaned
        private string fileName;
        private string goodFilePath; //Path of the directory where cleaned file is to be saved
        private string badFilePath; // Path of the directory where errored out rows need to be stored.
        private List<string> fileContent = new List<string>(); // List of strings to contain original file content       
        private List<string> fileContentCleaned = new List<string>(); //List of strings to contain cleaned file content
        private List<string> fileContentCorrupt = new List<string>();
        private string delimiter; // Store the delimiter
        private string qualifier; // Store the text qualifier
        private int fileType; // 0 if file is medical and 1 if file is elibigility
        
       
        public int[] countDelim;
        public int[] countQualifier;
        public int[] lineLenght;
        public List<int> DelimOutlier = new List<int>();
        public List<int> QualiOutlier = new List<int>();
        public List<int> LengthOutlier = new List<int>();
       
        public ClassFileValidator(string catchPath, string gPath, string bPath)
        {
            
            filePath = catchPath;
            goodFilePath = gPath;
            badFilePath = bPath;

            if(File.Exists(catchPath))
            {
                fileName = Path.GetFileName(filePath);
                Regex patternMedical = new Regex("Med");
                Regex patternEligibility = new Regex("Mem");
                Match medical = patternMedical.Match(fileName);
                Match eligibility = patternEligibility.Match(fileName);
                if (medical.Success)
                {
                    delimiter = (string)",";
                    qualifier = (string) "\"";
                    fileType = 0;
                }
                else if (eligibility.Success)
                {
                    delimiter = (string)",";
                    qualifier = (string) "\"";
                    fileType = 1;
                }
                string[] lines = System.IO.File.ReadAllLines(@filePath);

                //Add All the lines to a list fileContent
                foreach (string line in lines)
                {
                    if (line.Length > 50)
                    {
                        fileContent.Add(line.Trim());
                    }
                }
                countDelim = new int[fileContent.Count];
                countQualifier = new int[fileContent.Count];
                lineLenght = new int[fileContent.Count];
            }
         }

        private void frequencyCalc()
        {
                      
            //Character frequency count            
            for (int i = 0; i < fileContent.Count; i++)
            {
                string singleLine = fileContent[i];
                char[] charsInSingleLine = new char[singleLine.Length];
                for (int s = 0; s < singleLine.Length; s++)
                {
                    charsInSingleLine[s] = singleLine[s];
                }
                Hashtable characterCount = new Hashtable();
                lineLenght[i] = singleLine.Length;
                for (int j = 0; j < singleLine.Length; j++)
                {
                    if (!characterCount.ContainsKey(charsInSingleLine[j]))
                    {
                        characterCount.Add(charsInSingleLine[j], 1);
                    }
                    else
                    {
                        characterCount[charsInSingleLine[j]] = (int)characterCount[charsInSingleLine[j]] + 1;
                     }
                }
                if (characterCount.ContainsKey(delimiter[0]))
                {
                    countDelim[i] = (int)characterCount[delimiter[0]];
                }
                else
                {
                    countDelim[i] = 0;
                }
                if (characterCount.ContainsKey(qualifier[0]))
                {
                    countQualifier[i] = (int)characterCount[qualifier[0]];
                }
                else
                {
                    countQualifier[i] = 0;
                }

            }          
   
       
        }


        private void cleanFile()
        {
            for (int i = 0; i < fileContent.Count; i++)
            {
                string singleLine = fileContent[i];

                string[] columnValues = singleLine.Split(',');

                int noOfColumns = columnValues.Length;
                string cleanedLine = "";
                if (noOfColumns == 81 && fileType == 0)
                {
                    int insertCount = 0;

                    foreach (string col in columnValues)
                    {
                        if (insertCount != 0)
                        {
                            cleanedLine = cleanedLine + ",";
                            if (insertCount == 14)
                            {
                                cleanedLine = cleanedLine + '"';
                            }
                        }

                        if (insertCount == 13)
                        {
                            cleanedLine = cleanedLine + col;
                            cleanedLine = cleanedLine + '"';
                            insertCount++;

                        }
                        else
                        {
                            cleanedLine = cleanedLine + col;
                            insertCount++;
                        }



                    }
                }

                if (noOfColumns == 80 && fileType == 0)
                {
                    int insertCount = 0;
                    foreach (string col in columnValues)
                    {
                        if (insertCount != 0)
                        {
                            cleanedLine = cleanedLine + ",";
                        }

                        if (insertCount == 14)
                        {

                            cleanedLine = cleanedLine + '"';
                            cleanedLine = cleanedLine + ' ';
                            cleanedLine = cleanedLine + '"';
                            cleanedLine = cleanedLine + ',';
                            cleanedLine = cleanedLine + col;
                            insertCount = insertCount + 3;
                        }
                        else
                        {
                            cleanedLine = cleanedLine + col;
                            insertCount++;
                        }
                    }
                }

                if (noOfColumns == 82 && fileType == 0)
                {
                    int insertCount = 0;
                    foreach (string col in columnValues)
                    {
                        if (insertCount != 0)
                        {
                            cleanedLine = cleanedLine + ",";
                            if (insertCount == 14)
                            {
                                cleanedLine = cleanedLine + '"';
                            }
                        }

                        if (insertCount == 13)
                        {
                            cleanedLine = cleanedLine + col;
                            cleanedLine = cleanedLine + '"';
                            insertCount++;

                        }
                        else
                        {
                            cleanedLine = cleanedLine + col;
                            insertCount++;
                        }
                    }
                }

                if ((noOfColumns == 65 || noOfColumns == 66 || noOfColumns == 67) && fileType == 1)
                {
                    int insertCount = 0;
                    foreach (string col in columnValues)
                    {
                        if (insertCount != 0)
                        {
                            cleanedLine = cleanedLine + ",";

                        }
                        cleanedLine = cleanedLine + col;
                        insertCount++;

                    }
                }

                if (noOfColumns != 80 && noOfColumns != 81 && noOfColumns != 82 && fileType == 0)
                {
                     fileContentCorrupt.Add(singleLine);
                }

                if (noOfColumns != 65 && noOfColumns != 66 && noOfColumns != 67 && fileType == 1)
                {
                   fileContentCorrupt.Add(singleLine);
                }


                if (cleanedLine.Length > 0)
                {
                    fileContentCleaned.Add(cleanedLine);
                }
            }
        }


        public void validateFile()
        {
            frequencyCalc();
            cleanFile();
            // Path for file containing good rows
            string cleanedFilePath = goodFilePath + fileName;
            string errorPath = badFilePath + fileName;          
                using (StreamWriter sw = new StreamWriter(@cleanedFilePath))
                {
                    for (int k = 0; k < fileContentCleaned.Count; k++)
                    {
                        sw.WriteLine(fileContentCleaned[k]);
                    }
                }
                if (fileContentCorrupt.Count > 0)
                {
                    using (StreamWriter se = new StreamWriter(@errorPath))
                    {
                        for (int j = 0; j < fileContentCleaned.Count; j++)
                        {
                            se.WriteLine(fileContentCorrupt[j]);
                        }
                    }
                }


        }


       

        
    }
}
