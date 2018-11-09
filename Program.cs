using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Diagnostics;

namespace ExcelToSql
{
    class Program
    {
        #region from Text
        static void Scriptfromtxtfiles()
        {

            string sourcePath;
            Console.WriteLine("Enter Source file full path : ");
            sourcePath = Console.ReadLine();
            string[] lines = System.IO.File.ReadAllLines(sourcePath);
            //string script = @"CREATE TABLE {0} \n(\t\t{1}\t\t);";
            //bool findTableSet = false;
            bool findTable = false;
            bool findColumn = false;
            bool findDatatype = false;
            string TableName = string.Empty;
            List<string> ColumnName = new List<string>();
            List<string> DataType = new List<string>();
            foreach (string input in lines)
            {
                if (input == "TableName")
                {
                    findTable = true;
                    continue;
                }
                if (input == "ColumnName")
                {
                    findColumn = true;
                    continue;
                }
                if (input == "DataType")
                {
                    findDatatype = true;
                    continue;
                }
                if (findTable)
                {
                    TableName = input;
                    findTable = false;
                }
                if (findColumn)
                {
                    ColumnName.Add(input);
                    findColumn = false;
                }
                if (findDatatype)
                {
                    DataType.Add(input);
                    findDatatype = false;
                }
            }
            extractTable(TableName, ColumnName, DataType,new List<string>());

        }
        public static string outputPath = Directory.GetCurrentDirectory();
       
        #endregion

        #region From Excel File


        static String dataBaseName, serverName;
        static bool isDelta;
        static List<string> lsBatchFile = new List<string>();
        private static void createFile(string FileName,string FileExtension, string script)
        {
            // Create the file.
            if (FileExtension != ".bat")
                script = script + "\nGO\n\n";
            else
                script = script + "\n";
            if (!File.Exists(outputPath + FileName + FileExtension))
            {
                Directory.CreateDirectory(outputPath);
                using (FileStream fs = File.Create(outputPath + FileName + FileExtension))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(script);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }
            else
            {
                Directory.CreateDirectory(outputPath);
                File.AppendAllText(outputPath + FileName + FileExtension, script);

            }
        }


        public static string Versioning()
        {
            string Version = "1.0";
            for (int i = 0; true; ++i)
            {
                if (!Directory.Exists(outputPath + "\\" + Version))
                {
                    break;
                }
                else
                {
                    int part1 = Convert.ToInt32(Version.Split('.')[0]);
                    int part2 = Convert.ToInt32(Version.Split('.')[1]);
                    part2++;
                    if (part2 > 9)
                    {
                        part1++;
                        Version = part1.ToString() + ".0";
                    }
                    else
                    {
                        Version = part1.ToString() + "." + part2.ToString();
                    }
                }
            }
            outputPath = outputPath + "\\" + Version + "\\";
            return outputPath;
        }

        private static void extractTable(string TableName, List<string> ColumnName, List<string> DataType, List<string> NotNullColumnList)
        {
            string script;
            string tempbody = string.Empty;
            string strDelta = string.Empty;
           

            int maxLength = ColumnName.Max(x => x.Length);
            string formatColString = @"{0,-" + (maxLength + 5).ToString() + "}";
            maxLength = DataType.Max(x => x.Length);
            string formatDatString = @"{0,-" + (maxLength + 5).ToString() + "}";

            for (int j = 0; j < ColumnName.Count; ++j)
            {
                for (int i = 0; i < DataType.Count; ++i)
                    if (j == i)
                    {
                        if (i == 0 && j == 0)
                            tempbody += "\t\t" + String.Format(formatColString, ColumnName[j]) + String.Format(formatColString, DataType[i].ToUpper()) + " IDENTITY(1,1) NOT NULL,\n";
                        else
                        {
                            bool IsNotNull = CheckIsNotNull(ColumnName[j], NotNullColumnList);
                            if (IsNotNull)
                                tempbody += "\t\t" + String.Format(formatColString, ColumnName[j]) + String.Format(formatColString, DataType[i].ToUpper()) + " NOT NULL ,\n";
                            else
                                tempbody += "\t\t" + String.Format(formatColString, ColumnName[j]) + String.Format(formatColString, DataType[i].ToUpper()) + " NULL ,\n";
                        }
                    }
            }
            tempbody = tempbody.TrimEnd('\n');
            tempbody = tempbody.TrimEnd(',');
            script = string.Format("CREATE TABLE {0} \n( \t\n{1}\n);", TableName, tempbody);
            string primaryKey = "ALTER TABLE " + TableName + "\nADD CONSTRAINT PK_" + TableName + " PRIMARY KEY( " + TableName + "PID );";
            bool isPrimary = true;
            string ForeignKey = string.Empty;
            foreach (string checkPID in ColumnName)
            {
                if (checkPID.ToUpper().EndsWith("PID"))
                {
                    if (isPrimary)
                    {
                        isPrimary = false;
                        continue;
                    }
                    ForeignKey += "ALTER TABLE " + TableName + "\nADD CONSTRAINT FK_" + TableName + "_" + checkPID + " FOREIGN KEY ( " + checkPID + " ) REFERENCES " + checkPID.Substring(0, checkPID.Length - 3) + " ( " + checkPID + " );" + "\n\n";
                }
            }

            if (isDelta)
            {
                script = script + "\n\n" + primaryKey + "\n\n";
                script = string.Format("IF NOT EXISTS (select 1 from INFORMATION_SCHEMA.TABLES Where TABLE_NAME='{0}') \n BEGIN  \n {1} \n END",new string[] { TableName, script });
                isDelta = false;
            }
            else
            {
                script = script + "\n\n" + primaryKey + "\n\n";
            }
            createFile(TableName,".sql", script);
            lsBatchFile.Add(string.Format(@"sqlcmd -S {0} -d {1} -i F:\{2} -o F:\EmpAdds.txt", new string[] { serverName, dataBaseName, TableName + ".sql" }));
            

            if (!string.IsNullOrEmpty(ForeignKey))
            {
                createFile(TableName + "_FK", ".sql", "\n\n" + ForeignKey + "\n\nGO\n");
                lsBatchFile.Add( string.Format(@"sqlcmd -S {0} -d {1} -i F:\{2} -o F:\EmpAdds.txt", new string[] { serverName, dataBaseName, TableName + "_FK" + ".sql" }));
                
            }



        }

        public static DataTable ReadExcel(string fileName, string fileExt, string sheetName)
        {
            string conn = string.Empty;
            DataTable dtexcel = new DataTable();

            return GetTablesFromSheet(fileName, fileExt, out conn, dtexcel, sheetName);
        }

        private static bool CheckIsNotNull(string Column, List<string> NotNullColumnList)
        {
            if (NotNullColumnList == null || NotNullColumnList.Count == 0)
                return false;
            return NotNullColumnList.Contains(Column);
        }

        private static DataTable GetTablesFromSheet(string fileName, string fileExt, out string conn, DataTable dtexcel, string sheetName)
        {
            if (fileExt.CompareTo("xls") == 0)
                conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HRD=Yes;IMEX=1';"; //for below excel 2007  
            else
                conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=NO';"; //for above excel 2007  
            using (OleDbConnection con = new OleDbConnection(conn))
            {
                try
                {
                    OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [" + sheetName + "]", con); //here we read data from sheet1  
                    oleAdpt.Fill(dtexcel); //fill excel data into dataTable  
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return dtexcel;
        }

        private static void ScriptfromExcelFiles(string SourceFilepath)
        {
            string strpath = SourceFilepath.Split('.')[0].ToString();
            string strFileext = SourceFilepath.Split('.')[1].ToString();
            string conn = string.Empty;
            if (SourceFilepath.EndsWith("xls"))
                conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + SourceFilepath + ";Extended Properties='Excel 8.0;HRD=Yes;IMEX=1';"; //for below excel 2007  
            else
                conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + SourceFilepath + ";Extended Properties='Excel 12.0;HDR=NO';"; //for above excel 2007  
            String[] excelSheets; //=new string[] { "UserProfile" };
            int k = 0;
            using (OleDbConnection con = new OleDbConnection(conn))
            {
                // Open connection with the database.
                con.Open();
                DataTable dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                excelSheets = new String[dt.Rows.Count];
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[k] = row["TABLE_NAME"].ToString();
                    k++;
                }
            }
            foreach (string sheetName in excelSheets)
            {
                DataTable dataTable = ReadExcel(SourceFilepath, strFileext, sheetName);

                for (int i = 0; i < dataTable.Rows.Count ; ++i)
                    for (int j = 0; j < dataTable.Columns.Count ; ++j)
                    {
                        if(i>2&& dataTable.Columns.Count>j+1 && dataTable.Rows[i-3][j+1].ToString().ToLower().Trim() == "apply delta")
                        {
                            if(dataTable.Rows[i - 3][j + 2].ToString().ToLower().Trim()=="true")
                            {
                                isDelta = true;
                            }
                        }

                        if (dataTable.Rows[i][j].ToString().ToLower().Trim() == "table name")
                        {
                            List<string> AddColumnList, AddDataTypeList, AddNotNullColumnList;
                            AddColumnList = new List<string>();
                            AddDataTypeList = new List<string>();
                            AddNotNullColumnList = new List<string>();
                            
                            commanColumnInfo(dataTable, i, j, AddColumnList, AddDataTypeList, AddNotNullColumnList);
                            commanServerInfo(dataTable, i, j, out dataBaseName, out serverName);
                            string TableName=  newTableCreation(dataTable, i, j, AddColumnList, AddDataTypeList, AddNotNullColumnList);
                        }
                        #region Modified Columns
                        if (dataTable.Rows[i][j].ToString().ToLower().Trim() == "alter table name")
                        {
                            while (dataTable.Rows.Count<i &&  dataTable.Rows[i + 1][j].ToString().Trim() != string.Empty)
                            {
                                if (dataTable.Rows[i + 1][j - 1].ToString().ToLower().Trim() == "add column")
                                {

                                    string tableName = dataTable.Rows[i + 1][j].ToString().Trim();
                                    string columnName = dataTable.Rows[i + 1][j + 1].ToString().Trim();
                                    string dataType = dataTable.Rows[i + 1][j + 2].ToString().Trim();
                                    string isNullColumn = dataTable.Rows[i + 1][j + 3].ToString().Trim();
                                    string script = string.Empty;
                                    if (isNullColumn.ToLower() == "false")
                                        script = string.Format("ALTER TABLE {0} \nADD {1} {2} {3}", tableName, columnName, dataType, " NULL");
                                    else
                                        script = string.Format("ALTER TABLE {0} \nADD {1} {2} {3}", tableName, columnName, dataType, "NOT NULL");
                                    createFile(tableName, ".sql", script);

                                }
                                if (dataTable.Rows[i + 1][j - 1].ToString().ToLower().Trim() == "add not null constraint")
                                {

                                    string tableName = dataTable.Rows[i + 1][j].ToString().Trim();
                                    string columnName = dataTable.Rows[i + 1][j + 1].ToString().Trim();
                                    string dataType = dataTable.Rows[i + 1][j + 2].ToString().Trim();
                                    string isNullColumn = dataTable.Rows[i + 1][j + 3].ToString().Trim();
                                    string script = string.Empty;
                                    script = string.Format("ALTER TABLE {0} \nADD {1} {2} {3}", tableName, columnName, dataType, "NOT NULL");
                                    createFile(tableName, ".sql", script);

                                }
                                if (dataTable.Rows[i + 1][j - 1].ToString().ToLower().Trim() == "add forien key constraint")
                                {

                                    string tableName = dataTable.Rows[i + 1][j].ToString().Trim();
                                    string columnName = dataTable.Rows[i + 1][j + 1].ToString().Trim();
                                    string refTable = dataTable.Rows[i + 1][j + 4].ToString().Trim();
                                    string refColumn = dataTable.Rows[i + 1][j + 5].ToString().Trim();
                                    string constraintName = dataTable.Rows[i + 1][j + 6].ToString().Trim();
                                    string script = string.Empty;
                                    script = string.Format("ALTER TABLE {0} \nADD CONSTRAINT {1} FOREIGN KEY ( {2} ) REFERENCES {3} ( {4} );", tableName, constraintName, columnName, refTable, refColumn);
                                    createFile(tableName, ".sql", script);

                                }
                                if (dataTable.Rows[i + 1][j - 1].ToString().ToLower().Trim() == "add check constraint")
                                {

                                    string tableName = dataTable.Rows[i + 1][j].ToString().Trim();
                                    string columnName = dataTable.Rows[i + 1][j + 1].ToString().Trim();
                                    string constraintName = dataTable.Rows[i + 1][j + 6].ToString().Trim();
                                    string checkCondition = dataTable.Rows[i + 1][j + 7].ToString().Trim();

                                    string script = string.Empty;
                                    script = string.Format("ALTER TABLE {0} \nADD CONSTRAINT {1} CHECK {2};", tableName, constraintName, checkCondition);
                                    createFile(tableName, ".sql", script);

                                }
                                ++i;//Going for next row
                            }
                        }
                        #endregion
                    }
            }
        }

        private static void commanServerInfo(DataTable dataTable, int i, int j, out string dataBaseName, out string serverName)
        {
            if (dataTable.Rows[i - 2][j + 1].ToString().ToLower().Trim() == "server instance")
            {
                string lookupServerInstance = dataTable.Rows[i - 2][j + 2].ToString().ToLower().Trim();
                if (!string.IsNullOrEmpty(lookupServerInstance))
                    for (int row = 0; row < dataTable.Rows.Count - 1; ++row)
                    {
                        for (int column = 0; column < dataTable.Columns.Count; ++column)
                        {
                            if (dataTable.Rows[row][column].ToString().ToLower().Trim() == lookupServerInstance.ToLower() && dataTable.Rows[row + 1][column].ToString().ToLower().Trim() == "servername")
                            {
                                serverName = dataTable.Rows[row + 1][column + 1].ToString();
                                dataBaseName  = dataTable.Rows[row + 2][column + 1].ToString();
                                return;
                            }
                        }
                    }
            }
             dataBaseName = string.Empty;
             serverName = string.Empty;
        }

        private static void commanColumnInfo(DataTable dataTable, int i, int j, List<string> AddColumnList, List<string> AddDataTypeList, List<string> AddNotNullColumnList)
        {
            if (dataTable.Rows[i - 1][j + 2].ToString().ToLower().Trim() == "any default column")
            {

                string lookupAdditionalColums = dataTable.Rows[i][j + 2].ToString().ToLower().Trim();
                if (!string.IsNullOrEmpty(lookupAdditionalColums))
                    getAdditionalColumnInfo(dataTable, lookupAdditionalColums, AddColumnList, AddDataTypeList, AddNotNullColumnList);
            }
        }

        private static void getAdditionalColumnInfo(DataTable dataTable, string lookupAdditionalColums,List<string> ColumnList, List<string> DataTypeList, List<string> NotNullColumnList)
        {
            for (int i = 0; i < dataTable.Rows.Count; ++i)
            {
                for (int j = 0; j < dataTable.Columns.Count; ++j)
                {
                    if (dataTable.Rows[i][j].ToString().ToLower().Trim() == lookupAdditionalColums.ToLower() && dataTable.Rows[i+1][j].ToString().ToLower().Trim() != "is not null(default false)")
                    {
                        getColumnDataTypeList(dataTable, i, j, ColumnList, DataTypeList, NotNullColumnList);
                    }
                }
            }
        }

        private static string newTableCreation(DataTable dataTable, int i, int j, List<String> AddColumnList, List<String> AddDataTypeList, List<String> AddNotNullColumnList)
        {

            string TableName = string.Empty;
            List<string> ColumnList = new List<string>();
            List<string> DataTypeList = new List<string>();
            List<string> NotNullColumnList = new List<string>();
            TableName = dataTable.Rows[i][j + 1].ToString().Trim();
            if (!string.IsNullOrEmpty(TableName))
            {
                getColumnDataTypeList(dataTable, i, j, ColumnList, DataTypeList, NotNullColumnList);
                ColumnList.AddRange(AddColumnList);
                DataTypeList.AddRange(AddDataTypeList);
                NotNullColumnList.AddRange(AddNotNullColumnList);
                extractTable(TableName, ColumnList, DataTypeList, NotNullColumnList);
            }
            return TableName;
        }

        private static void getColumnDataTypeList(DataTable dataTable, int i, int j, List<string> ColumnList, List<string> DataTypeList, List<string> NotNullColumnList)
        {
            for (int c = 1; c < dataTable.Columns.Count - 1; ++c)
            {
                //didn't find any reason for getting below code
                if (i + c == dataTable.Rows.Count )
                    break;
                string column = dataTable.Rows[i + c][j].ToString().Trim();
                if (column.ToLower() == "column name")
                {
                    continue;
                }
                if (string.IsNullOrEmpty(column))
                {
                    break;
                }
                ColumnList.Add(column);
            }
            for (int c = 2; c < dataTable.Columns.Count - 1; ++c)
            {
                //didn't find any reason for getting below code
                if (i + c == dataTable.Rows.Count)
                    break;
                string column = string.Empty;
                column = dataTable.Rows[i + c][j + 1].ToString().Trim();
                if (column.ToLower() == "data type")
                {
                    continue;
                }
                if (string.IsNullOrEmpty(column))
                {
                    break;
                }
                DataTypeList.Add(column);
            }
            for (int c = 3; c < dataTable.Columns.Count - 1; ++c)
            {
                if (i + c == dataTable.Rows.Count)
                    break;
                string column = string.Empty;
                string columnValue = string.Empty;

                column = dataTable.Rows[i + c][j].ToString().Trim();
                //Optional Null Setting value
                //So if no cell is found just return Control with Empty list
                if (dataTable.Columns.Count <= j + 2)
                {
                    break;
                }
                columnValue = dataTable.Rows[i + c][j + 2].ToString().Trim();
                if (column.ToLower() == "Is NOT NULL(Default False)")
                {
                    continue;
                }
                if (string.IsNullOrEmpty(column))
                {
                    break;
                }
                if (columnValue.ToLower() == "true")
                    NotNullColumnList.Add(column);
            }
        }

        #endregion

        static void Main(string[] args)
        {
           
            try
            {
                String SourceFilepath = string.Empty;
                Console.WriteLine(".xls file path: ");

                SourceFilepath = Console.ReadLine();
                if (string.IsNullOrEmpty(SourceFilepath))
                    SourceFilepath = @"G:\New folder\EtoS.xlsx";
                outputPath = Versioning();
                //Script generated from text files
                //  <--Scriptfromtxtfiles();--!>
                //Script generated from Excel File
                ScriptfromExcelFiles(SourceFilepath);
                foreach (string str in lsBatchFile.Where(x=>!x.Contains("_FK")))
                {
                    createFile("Manual", ".bat", str);
                }
                foreach (string str in lsBatchFile.Where(x => x.Contains("_FK")))
                {
                    createFile("Manual", ".bat", str);
                }
                Console.WriteLine("Process completed");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Issue found" + ex.Message);
                Console.ReadLine();
            }
        }
    }
}
