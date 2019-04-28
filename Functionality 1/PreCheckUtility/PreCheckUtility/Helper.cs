using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NHunspell;

namespace PreCheckUtility
{
    static class Helper
    {
        /// <summary>
        /// Function to read XML file and will execute query
        /// </summary>
        /// <returns>0 - Success ; 1- Failure</returns>
        public static int ReadXML()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings[Constants.CONNECTION_STRING_SOURCE].ConnectionString;                  //connection string for upstream
                string connectionStringDestination = ConfigurationManager.ConnectionStrings[Constants.CONNECTION_STRING_DESTINATION].ConnectionString; //connection string for maq mart
                int value = 0;

                List<TestCase> testCases = new List<TestCase>();    //List of testcase

                using (XmlReader reader = XmlReader.Create(ConfigurationManager.AppSettings[Constants.XML_FILE_PATH]))
                {
                    DataTable dataTableSource = new DataTable();        //Data Table for Upstream query
                    DataTable dataTableDestination = new DataTable();   //Data Table for Maqmart query
                    int TheshHoldValue = 0;
                    int ExpectedResult = 0;

                    while (reader.Read())
                    {
                        TestCase testCase = new TestCase(); //create testcase for each query
                        try
                        {
                            if (reader.IsStartElement())
                            {
                                string tag = Convert.ToString(reader.Name, CultureInfo.InvariantCulture);
                                if (tag == Constants.TYPE)
                                {
                                    string queryType = reader.ReadString();
                                    switch (queryType)
                                    {
                                        case Constants.DIMENSION:
                                            AddToLogFile(queryType);

                                            //for dimension query read until next Query tag is reached.
                                            while (reader.Read())
                                            {
                                                if (reader.IsStartElement())
                                                {
                                                    tag = Convert.ToString(reader.Name, CultureInfo.InvariantCulture);
                                                    if (tag == Constants.QUERY)
                                                    {
                                                        break;
                                                    }
                                                    string tagContent = reader.ReadString();

                                                    switch (tag)
                                                    {
                                                        case Constants.DESCRIPTION:
                                                            testCase.Name = tagContent;
                                                            break;

                                                        case Constants.ISJOBFAIL:
                                                            testCase.IsJobFail = Convert.ToInt32(tagContent, CultureInfo.InvariantCulture);
                                                            break;

                                                        case Constants.UPSTREAM:
                                                            AddToLogFile(Constants.UPSTREAM_OUTPUT);
                                                            string querySource = tagContent;
                                                            testCase.UpStream = querySource;
                                                            dataTableSource = Connector(connectionString, querySource);

                                                            for (int i = 0; i < dataTableSource.Rows.Count; i++)
                                                            {
                                                                for (int j = 0; j < dataTableSource.Columns.Count; j++)
                                                                {
                                                                    AddToLogFile(Constants.BLANKSPACE + dataTableSource.Rows[i][j]);
                                                                }
                                                            }
                                                            break;

                                                        case Constants.MAQMART:
                                                            AddToLogFile(Constants.MAQMART_OUTPUT);
                                                            string queryDestination = tagContent;
                                                            testCase.MaqMart = queryDestination;
                                                            dataTableDestination = Connector(connectionStringDestination, queryDestination);

                                                            for (int i = 0; i < dataTableDestination.Rows.Count; i++)
                                                            {
                                                                for (int j = 0; j < dataTableDestination.Columns.Count; j++)
                                                                {
                                                                    AddToLogFile(Constants.BLANKSPACE + dataTableDestination.Rows[i][j]);
                                                                }
                                                                AddToLogFile(Constants.BLANKSPACE);
                                                            }

                                                            value = AreTablesTheSame(dataTableSource, dataTableDestination);
                                                            if (value == 1)
                                                            {
                                                                Console.WriteLine(Constants.DIMENSION_NOT_MATCH);
                                                                testCase.Status = Constants.FAIL;
                                                            }
                                                            else
                                                            {
                                                                testCase.Status = Constants.PASS;
                                                            }

                                                            testCases.Add(testCase);
                                                            break;

                                                        case Constants.COMMENTS:
                                                            testCase.Comments = tagContent;
                                                            break;
                                                    }
                                                }
                                            }
                                            break;

                                        case Constants.THRESHOLD_CHECK:
                                            AddToLogFile(queryType);

                                            //for thresholdcheck query read until next Query tag is reached.
                                            while (reader.Read())
                                            {
                                                if (reader.IsStartElement())
                                                {
                                                    tag = Convert.ToString(reader.Name, CultureInfo.InvariantCulture);
                                                    if (tag == Constants.QUERY)
                                                    {
                                                        break;
                                                    }
                                                    string tagContent = reader.ReadString();

                                                    switch (tag)
                                                    {
                                                        case Constants.DESCRIPTION:
                                                            testCase.Name = tagContent;
                                                            break;

                                                        case Constants.ISJOBFAIL:
                                                            testCase.IsJobFail = Convert.ToInt32(tagContent, CultureInfo.InvariantCulture);
                                                            break;

                                                        case Constants.UPSTREAM:
                                                            AddToLogFile(Constants.UPSTREAM_OUTPUT);
                                                            string querySource = tagContent;
                                                            testCase.UpStream = querySource;
                                                            dataTableSource = Connector(connectionString, querySource);

                                                            for (int i = 0; i < dataTableSource.Rows.Count; i++)
                                                            {
                                                                for (int j = 0; j < dataTableSource.Columns.Count; j++)
                                                                {
                                                                    AddToLogFile(Constants.BLANKSPACE + dataTableSource.Rows[i][j]);
                                                                }
                                                            }
                                                            break;

                                                        case Constants.MAQMART:
                                                            AddToLogFile(Constants.MAQMART_OUTPUT);
                                                            string queryDestination = tagContent;
                                                            testCase.MaqMart = queryDestination;
                                                            dataTableDestination = Connector(connectionStringDestination, queryDestination);

                                                            for (int i = 0; i < dataTableDestination.Rows.Count; i++)
                                                            {
                                                                for (int j = 0; j < dataTableDestination.Columns.Count; j++)
                                                                {
                                                                    AddToLogFile(Constants.BLANKSPACE + dataTableDestination.Rows[i][j]);
                                                                }
                                                            }
                                                            break;

                                                        case Constants.THRESHOLD:
                                                            AddToLogFile(Constants.THRESHOLD_VALUE);
                                                            TheshHoldValue = Convert.ToInt32(tagContent, CultureInfo.InvariantCulture);
                                                            AddToLogFile(Convert.ToString(TheshHoldValue, CultureInfo.InvariantCulture));
                                                            value = IsChangesRight(dataTableSource, dataTableDestination, TheshHoldValue);

                                                            if (value == 1)
                                                            {
                                                                AddToLogFile(Constants.EXCEED_CHANGE);
                                                                Console.WriteLine(Constants.EXCEED_CHANGE);
                                                                testCase.Status = Constants.FAIL;
                                                            }
                                                            else
                                                            {
                                                                testCase.Status = Constants.PASS;
                                                            }

                                                            testCases.Add(testCase);
                                                            AddToLogFile(Constants.VALUE + value);
                                                            TheshHoldValue = 0;
                                                            break;

                                                        case Constants.COMMENTS:
                                                            testCase.Comments = tagContent;
                                                            break;
                                                    }
                                                }


                                            }
                                            break;

                                        case Constants.EXPECTED_OUTPUT:
                                            AddToLogFile(queryType);

                                            //for expected output query read until next Query tag is reached.
                                            while (reader.Read())
                                            {
                                                if (reader.IsStartElement())
                                                {
                                                    tag = Convert.ToString(reader.Name, CultureInfo.InvariantCulture);
                                                    if (tag == Constants.QUERY)
                                                    {
                                                        break;
                                                    }
                                                    string tagContent = reader.ReadString();

                                                    switch (tag)
                                                    {
                                                        case Constants.DESCRIPTION:
                                                            testCase.Name = tagContent;
                                                            break;

                                                        case Constants.ISJOBFAIL:
                                                            testCase.IsJobFail = Convert.ToInt32(tagContent, CultureInfo.InvariantCulture);
                                                            break;

                                                        case Constants.UPSTREAM:
                                                            AddToLogFile(Constants.UPSTREAM_OUTPUT);
                                                            string querySource = tagContent;
                                                            testCase.UpStream = querySource;
                                                            dataTableSource = Connector(connectionString, querySource);

                                                            for (int i = 0; i < dataTableSource.Rows.Count; i++)
                                                            {
                                                                for (int j = 0; j < dataTableSource.Columns.Count; j++)
                                                                {

                                                                    AddToLogFile(Constants.BLANKSPACE + dataTableSource.Rows[i][j]);
                                                                }
                                                                AddToLogFile(Constants.BLANKSPACE);
                                                            }
                                                            break;

                                                        case Constants.MAQMART:
                                                            testCase.MaqMart = tagContent;
                                                            break;

                                                        case Constants.EXPECTED_RESULT:
                                                            AddToLogFile(Constants.EXPECTED_RESULT);
                                                            ExpectedResult = Convert.ToInt32(tagContent, CultureInfo.InvariantCulture);
                                                            AddToLogFile(Convert.ToString(ExpectedResult, CultureInfo.InvariantCulture));

                                                            value = IsResultMatching(dataTableSource, ExpectedResult);
                                                            if (value == 1)
                                                            {
                                                                string errorMessage = reader.ReadString();
                                                                Console.WriteLine(errorMessage);
                                                                AddToLogFile(errorMessage);
                                                                testCase.Status = Constants.FAIL;
                                                            }
                                                            else
                                                            {
                                                                testCase.Status = Constants.PASS;
                                                            }
                                                            testCases.Add(testCase);
                                                            AddToLogFile(Constants.VALUE + value);
                                                            TheshHoldValue = 0;
                                                            break;

                                                        case Constants.COMMENTS:
                                                            testCase.Comments = tagContent;
                                                            break;
                                                    }
                                                }
                                            }
                                            break;

                                        case Constants.SPELL_CHECK:
                                            AddToLogFile(queryType);

                                            //for spell check query read until next Query tag is reached.
                                            while (reader.Read())
                                            {
                                                if (reader.IsStartElement())
                                                {
                                                    tag = Convert.ToString(reader.Name);
                                                    if (tag == Constants.QUERY)
                                                    {
                                                        break;
                                                    }
                                                    string tagContent = reader.ReadString();

                                                    switch (tag)
                                                    {
                                                        case Constants.DESCRIPTION:
                                                            testCase.Name = tagContent;
                                                            break;

                                                        case Constants.ISJOBFAIL:
                                                            testCase.IsJobFail = Convert.ToInt32(tagContent);
                                                            break;

                                                        case Constants.UPSTREAM:
                                                            AddToLogFile(Constants.UPSTREAM_OUTPUT);
                                                            string querySource = tagContent;
                                                            testCase.UpStream = querySource;
                                                            dataTableSource = Connector(connectionString, querySource);

                                                            bool statusFlag = true;
                                                            for (int i = 0; i < dataTableSource.Rows.Count; i++)
                                                            {
                                                                for (int j = 0; j < dataTableSource.Columns.Count; j++)
                                                                {
                                                                    bool questionFlag = true;

                                                                    AddToLogFile(Constants.BLANKSPACE + dataTableSource.Rows[i][j]);
                                                                    string question = Convert.ToString(dataTableSource.Rows[i][j], CultureInfo.InvariantCulture);
                                                                    string finalQuestion = "";

                                                                    string[] words = question.Split(' ', '/');

                                                                    foreach (string word in words)
                                                                    {
                                                                        bool correct = CheckSpelling(word);

                                                                        if (!correct)
                                                                        {
                                                                            questionFlag = false;
                                                                            finalQuestion += Constants.UNDERLINE;
                                                                            finalQuestion += word;
                                                                            finalQuestion += " ";
                                                                            finalQuestion += Constants.UNDERLINE_END;
                                                                        }
                                                                        else
                                                                        {
                                                                            finalQuestion += word;
                                                                            finalQuestion += " ";
                                                                        }

                                                                    }
                                                                    if (!questionFlag)
                                                                    {
                                                                        AddToLogFile(Constants.INCORRECT);
                                                                        statusFlag = false;
                                                                        testCase.Comments += Constants.NEW_LINE;
                                                                        testCase.Comments += Constants.LIST_ITEM;
                                                                        testCase.Comments += finalQuestion;
                                                                        testCase.Comments += Constants.LIST_ITEM_END;
                                                                    }
                                                                    else
                                                                    {
                                                                        AddToLogFile(Constants.CORRECT);
                                                                    }
                                                                }
                                                                AddToLogFile(Constants.BLANKSPACE);
                                                            }
                                                            if (statusFlag)
                                                            {
                                                                testCase.Status = Constants.PASS;
                                                            }
                                                            else
                                                            {
                                                                testCase.Status = Constants.WARNING;
                                                            }
                                                            testCases.Add(testCase);
                                                            break;

                                                        case Constants.MAQMART:
                                                            testCase.MaqMart = tagContent;
                                                            break;

                                                        case Constants.COMMENTS:
                                                            testCase.Comments = tagContent;
                                                            break;

                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            Console.WriteLine(Constants.BLANKSPACE);
                            AddToLogFile(Constants.BLANKSPACE);
                        }
                        catch (Exception exception)
                        {
                            Program.loggerDetails.LogException(exception);
                            testCase.Status = Constants.FAIL;
                            testCases.Add(testCase);
                        }
                    }
                }
                ResultMail(testCases);  //send the result of the query execution in mail.
                bool jobFail = false;   //flag to fail job

                //for any testcase if query fails and query is critical then set jobFail flag to true.
                foreach (TestCase testCase in testCases)
                {
                    if (testCase.Status == Constants.FAIL && testCase.IsJobFail == 1)
                    {
                        StringBuilder failedTestCase = new StringBuilder(testCase.Comments);
                        failedTestCase.Append(Constants.BLANKSPACE);
                        failedTestCase.Append(Constants.NEW_LINE);
                        Console.WriteLine(failedTestCase);
                        jobFail = true;
                    }
                }
                if (jobFail)
                {
                    return 1;
                }
            }
            catch (Exception exception)
            {
                Program.loggerDetails.LogException(exception);
                return 1; //Returning Failure
            }
            return 0;
        }

        /// <summary>
        /// Function that sends the result of the query execution.
        /// </summary>
        /// <param name="testCases">List of testCases</param>
        internal static void ResultMail(List<TestCase> testCases)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                using (SmtpClient SmtpServer = new SmtpClient(Constants.MAIL_SMTP))
                {
                    using (KeyVaultClient keyVault = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken)))
                    {
                        // Pass Key Vault Secret URL 
                        var connectionString = keyVault.GetSecretAsync(ConfigurationManager.AppSettings[Constants.SECRET_URI]).GetAwaiter().GetResult();
                        string SsmsPassword = connectionString.Value;
                        mailMessage.IsBodyHtml = true;
                        mailMessage.From = new MailAddress(ConfigurationManager.AppSettings[Constants.FROM_USER_ID]);
                        string[] recipents = ConfigurationManager.AppSettings[Constants.MAIL_RECIPENTS].Split(Constants.SPLIT_COMMA);
                        SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings[Constants.FROM_USER_ID], SsmsPassword);
                        SmtpServer.EnableSsl = true;

                        foreach (string recipent in recipents)
                        {
                            mailMessage.To.Add(recipent.Trim());
                        }

                        mailMessage.CC.Add(ConfigurationManager.AppSettings[Constants.CC_RECIPENT]);
                        mailMessage.Subject = String.Format(CultureInfo.InvariantCulture, Constants.RESULT_MAIL_SUBJECT, Constants.NEEDS_ATTENTION);
                        mailMessage.Priority = MailPriority.High;
                        Attachment attachment = new Attachment(Program.logFilePath);
                        mailMessage.Attachments.Add(attachment);
                        mailMessage.Body = CreateHtmlTemplate(testCases);
                    }
                    SmtpServer.Send(mailMessage);
                }
            }
        }

        /// <summary>
        /// Creates the html view in the mail.
        /// </summary>
        /// <param name="testCases">List of testCases</param>
        /// <returns></returns>
        private static string CreateHtmlTemplate(List<TestCase> testCases)
        {
            StringBuilder body = new StringBuilder();
            body.Clear();
            body.Append(Constants.HTML_HEAD);
            body.Append(String.Format(CultureInfo.InvariantCulture, Constants.HEADER_TEMPLATE, Constants.MAIL_HEADER + DateTime.Now.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)));
            body.Append(Constants.TABLE_HEADERS);

            for (int i = 0; i < testCases.Count; i++)
            {
                body.Append(Constants.TABLE_ROW);

                body.Append(Constants.TABLE_CELL);
                body.Append(i + 1);
                body.Append(Constants.TABLE_CELL_END);

                body.Append(Constants.TABLE_CELL);
                body.Append(testCases[i].Name);
                body.Append(Constants.TABLE_CELL_END);

                body.Append(Constants.TABLE_CELL_QUERY);
                body.Append(testCases[i].UpStream);
                body.Append(Constants.TABLE_CELL_END);

                body.Append(Constants.TABLE_CELL_QUERY);
                body.Append(testCases[i].MaqMart);
                body.Append(Constants.TABLE_CELL_END);

                if (testCases[i].Status == Constants.FAIL)
                {
                    body.Append(Constants.TABLE_CELL_RED);
                    body.Append(testCases[i].Status);
                    body.Append(Constants.TABLE_CELL_END);
                    body.Append(Constants.TABLE_CELL);
                    body.Append(testCases[i].Comments);
                    body.Append(Constants.TABLE_CELL_END);
                }
                else if (testCases[i].Status == Constants.WARNING)
                {
                    body.Append(Constants.TABLE_CELL_ORANGE);
                    body.Append(testCases[i].Status);
                    body.Append(Constants.TABLE_CELL_END);
                    body.Append(Constants.TABLE_CELL);
                    body.Append(testCases[i].Comments);
                    body.Append(Constants.TABLE_CELL_END);
                }
                else
                {
                    body.Append(Constants.TABLE_CELL_GREEN);
                    body.Append(testCases[i].Status);
                    body.Append(Constants.TABLE_CELL_END);
                    body.Append(Constants.TABLE_CELL);
                    body.Append(Constants.BLANKSPACE);
                    body.Append(Constants.TABLE_CELL_END);
                }
                body.Append(Constants.TABLE_ROW_END);
            }
            body.Append(Constants.TABLE_END);
            body.Append(Constants.FOOTER);
            return body.ToString();
        }

        /// <summary>
        /// Connects to Key Vault
        /// </summary>
        /// <param name="authority"></param>
        /// <param name="resource"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority);

            ClientCredential clientCred = new ClientCredential(ConfigurationManager.AppSettings[Constants.CLIENT_ID], DpapiHandler.Decrypt(ConfigurationManager.AppSettings[Constants.CLIENT_SECRET], DataProtectionScope.LocalMachine));
            AuthenticationResult result = authContext.AcquireTokenAsync(resource, clientCred).Result;

            if (result == null)
            {
                throw new InvalidOperationException(Constants.NOT_FOUND);
            }
            return Task.Run(() =>
            {
                return result.AccessToken;
            });
        }

        /// <summary>
        /// Function to compare the tables from upstream and the raw data table
        /// </summary>
        /// <param name="sourceTable">First table</param>
        /// <param name="destinationTable">Second table to compare</param>
        /// <returns>0 - Success ; 1- Failure</returns>
        public static int AreTablesTheSame(DataTable sourceTable, DataTable destinationTable)
        {
            if (sourceTable.Rows.Count != destinationTable.Rows.Count || sourceTable.Columns.Count != destinationTable.Columns.Count)
            {
                Program.loggerDetails.LogInformation(Constants.INFORMATION);
                return 1;
            }
            for (int sourceCounter = 0; sourceCounter < sourceTable.Rows.Count; sourceCounter++)
            {
                for (int destinationCounter = 0; destinationCounter < sourceTable.Columns.Count; destinationCounter++)
                {
                    if (sourceTable.Rows[sourceCounter][destinationCounter].ToString().Trim() != destinationTable.Rows[sourceCounter][destinationCounter].ToString().Trim())
                    {
                        Program.errorValue = sourceTable.Rows[sourceCounter][destinationCounter].ToString(); // The value which causes the precheck to break
                        Program.loggerDetails.LogInformation(Program.errorValue); // Logging the value which broke
                        return 1;  //Returning Failure
                    }
                }
            }
            return 0; //Returning Success
        }

        /// <summary>
        /// Function to compare the tables from upstream and the raw data table with threshhold
        /// </summary>
        /// <param name="sourceTable">First table</param>
        /// <param name="destinationTable">Second table to compare</param>
        /// <param name="threshHoldValue">threshHold Value upto which changes acceptable</param>
        /// <returns>0 - Success ; 1- Failure</returns>
        public static int IsChangesRight(DataTable sourceTable, DataTable destinationTable, int threshHoldValue)
        {
            if (sourceTable.Rows.Count != destinationTable.Rows.Count || sourceTable.Columns.Count != destinationTable.Columns.Count)
            {
                Program.loggerDetails.LogInformation(Constants.INFORMATION);
                return 1;
            }
            for (int sourceCounter = 0; sourceCounter < sourceTable.Rows.Count; sourceCounter++)
            {
                for (int destinationCounter = 0; destinationCounter < sourceTable.Columns.Count; destinationCounter++)
                {
                    int UpstreamCount, MartCount;
                    MartCount = Convert.ToInt32(destinationTable.Rows[sourceCounter][destinationCounter].ToString().Trim(), CultureInfo.InvariantCulture);
                    UpstreamCount = Convert.ToInt32(sourceTable.Rows[sourceCounter][destinationCounter].ToString().Trim(), CultureInfo.InvariantCulture);
                    Double Difference = (UpstreamCount - MartCount) * 100;
                    Double varience = Convert.ToDouble(Difference / MartCount);
                    if (varience > threshHoldValue || varience < -threshHoldValue)
                    {
                        Program.errorValue = sourceTable.Rows[sourceCounter][destinationCounter].ToString(); // The value which causes the precheck to brea
                        Program.loggerDetails.LogInformation(Program.errorValue); // Logging the value which broke
                        return 1;  //Returning Failure
                    }
                }
            }
            return 0; //Returning Success
        }

        /// <summary>
        /// Function to check upstream query output with expected output
        /// </summary>
        /// <param name="sourceTable">First table</param>
        /// <param name="ExpectedResult">ExpectedResult Value which should match with query output</param>
        /// <returns>0 - Success ; 1- Failure</returns>
        public static int IsResultMatching(DataTable sourceTable, int ExpectedResult)
        {
            for (int sourceCounter = 0; sourceCounter < sourceTable.Rows.Count; sourceCounter++)
            {
                int UpstreamCount;
                UpstreamCount = Convert.ToInt32(sourceTable.Rows[sourceCounter][0].ToString().Trim(), CultureInfo.InvariantCulture);
                if (ExpectedResult != UpstreamCount)
                {
                    Program.errorValue = sourceTable.Rows[sourceCounter][0].ToString(); // The value which causes the precheck to brea
                    Program.loggerDetails.LogInformation(Program.errorValue); // Logging the value which broke
                    return 1;  //Returning Failure
                }
            }
            return 0; //Returning Success
        }

        /// <summary>
        /// Used to establish connection for a particular connectionString
        /// </summary>
        /// <param name="connectionString">The connectionstring</param>
        /// <param name="query">Query to retrieve data</param>
        /// <returns></returns>
        public static DataTable Connector(string connectionString, string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand();
                command.CommandType = CommandType.Text; ;
                command.CommandText = query;
                command.Connection = connection;
                connection.Open();
                DataTable dataTable = new DataTable();
                dataTable.Load(command.ExecuteReader());
                return dataTable;
            }

        }

        /// <summary>
        /// This function logs error message to specified text file
        /// </summary>
        /// <param name="errorMessage">Error message to be logged</param>
        public static void AddToLogFile(string errorMessage)
        {
            string filePath = string.Concat(Environment.CurrentDirectory, Constants.LOG_FILE_PATH);
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    sw.WriteLine(string.Format(CultureInfo.InvariantCulture, Constants.LOGGING_DATE_TIME, DateTime.Now, errorMessage));
                    sw.Flush();
                }
            }
        }

        /// <summary>
        /// Method that gets the console and the log file ready for logging
        /// </summary>
        /// <param name="logFilePath">Path to the log file.</param>
        internal static void SetOutputWindow(string logFilePath)
        {
            File.WriteAllText(logFilePath, string.Empty);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
        }

        /// <summary>
        /// Method that checks the spelling of a given word
        /// </summary>
        /// <param name="word"></param>
        /// <returns>true: if spelling is correct, false: if spelling is not correct</returns>
        internal static bool CheckSpelling(string word)
        {
            using (Hunspell hunspell = new Hunspell("en_us.aff", "en_us.dic"))
            {
                bool correct = hunspell.Spell(Regex.Replace(word, @"([^a-zA-Z0-9_]|^\s)", string.Empty));
                if (!correct)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
