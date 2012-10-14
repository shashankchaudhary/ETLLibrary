using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace AHAPatientXMLReporting
{
    public class ReadTxtFile
    {
        //Variable Declaration
        protected string file_Path;
        protected string[] fileLines;

        //Constructor definition
        public ReadTxtFile()
        {
            file_Path = "";
            
        }

        //get set methods for private members
        public string filePath
        {
            get
            {
                return file_Path;
            }

            set
            {
                file_Path = value;
            }
        }

        //Methods

        protected void readFileData()
        {
            try
            {
                if (File.Exists(file_Path))
                {
                    fileLines = File.ReadAllLines(file_Path);
                }
                else
                {
                    fileLines = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] getTxtLines()
        {
            try
            {
                readFileData();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return fileLines;
        }
    }
}
