using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace AHAPatientXMLReporting
{
    class ParseCSV : ReadTxtFile
    {
        //Variable declaration
        private DataTable csvDT;
        private string[] csvRows;
        private char delimiter;
        private char textSeparator;
        private int rowAsHeading;
        //Constructor
        public ParseCSV()
        {
            csvDT = new DataTable();
            csvRows = null;
            delimiter = ',';
            textSeparator = '\"';
            rowAsHeading = 0;
        }

        //Methods

        public DataTable getCSVDataTable(string fPath, char delim, char tSeparator, int firstRow)
        {
            this.filePath = fPath;
            this.csvRows = getTxtLines();
            this.delimiter = delim;
            this.textSeparator = tSeparator;
            this.rowAsHeading = firstRow;

            if (firstRow == 0)
            {
                string columnHeading = "";
                int tSepCounter = 0;
                int delimMarker = 0;
                foreach (char currentChar in csvRows[0])
                {
                    if (currentChar == textSeparator && tSepCounter == 0)
                    {
                        tSepCounter = 1;
                    }
                    else if (currentChar == textSeparator && tSepCounter == 1)
                    {
                        tSepCounter = 2;
                    }

                    if (delimMarker == 0 && currentChar == delimiter && (tSepCounter == 0 || tSepCounter == 2))
                    {
                        delimMarker = 1;
                    }

                    if (delimMarker == 1)
                    {
                        csvDT.Columns.Add(columnHeading.ToString(), typeof(string));
                        columnHeading = "";
                        delimMarker = 0;
                        tSepCounter = 0;
                    }
                    else
                    {
                        columnHeading = columnHeading + (currentChar.ToString());
                    }
                }

                csvDT.Columns.Add(columnHeading.ToString(), typeof(string));
                int skipFirstRow = 0;

                foreach (string currentString in csvRows)
                {
                    if (skipFirstRow == 0)
                    {
                        skipFirstRow = 1;
                    }
                    else
                    {
                        int rowIndex = 0;
                        DataRow singleRow = csvDT.NewRow();
                        tSepCounter = 0;
                        delimMarker = 0;
                        string columnValue = "";
                        foreach (char element in currentString)
                        {
                            if (element == textSeparator && tSepCounter == 0)
                            {
                                tSepCounter = 1;
                            }
                            else if (element == textSeparator && tSepCounter == 1)
                            {
                                tSepCounter = 2;
                            }

                            if (delimMarker == 0 && element == delimiter && (tSepCounter == 0 || tSepCounter == 2))
                            {
                                delimMarker = 1;
                            }

                            if (delimMarker == 1)
                            {
                                singleRow[rowIndex] = columnValue.Trim();
                                columnValue = "";
                                rowIndex++;
                                delimMarker = 0;
                                tSepCounter = 0;
                            }
                            else
                            {
                                if (element != textSeparator)
                                {
                                    columnValue = columnValue + (element.ToString());
                                }
                            }

                        }
                        singleRow[rowIndex] = columnValue.Trim();
                        csvDT.Rows.Add(singleRow);
                    }

                }

            }

            return csvDT;


        }


 

    }
}
