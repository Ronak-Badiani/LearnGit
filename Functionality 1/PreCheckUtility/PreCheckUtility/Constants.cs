/// <summary>
/// Class to hold all constant strings  
/// </summary>
namespace PreCheckUtility
{
    internal static class Constants
    {
        internal const string CONNECTION_STRING_SOURCE = "connectionStringSource";
        internal const string DIFFERENCE_COUNT = "differenceCount";
        internal const string CONNECTION_STRING_DESTINATION = "connectionStringDestination";
        internal const string APPLICATION = "Application";
        internal const string QUERY = "Query";
        internal const string TYPE = "Type";
        internal const string FAIL = "Fail";
        internal const string PASS = "Pass";
        internal const string WARNING = "Warning";
        internal const string SPACE = " ";
        internal const int EVENT_ID = 108;
        internal const string ARGUMENTS = "Keys do not match with the upstream";
        internal const string INFORMATION = "Mismatch in number of rows in tables";
        internal const string XML_FILE_PATH = "XMLFilePath";
        internal const string LOG_FILE_PATH = "\\logfile.txt";
        internal const string LOGGING_DATE_TIME = "[{0}]: {1}";

        #region Console Messages
        internal const string UPSTREAM_OUTPUT = "UPSTREAM Query output : ";
        internal const string MAQMART_OUTPUT = "MAQMART Query output : ";
        internal const string THRESHOLD_VALUE = "Threshold  value is : ";
        internal const string BLANKSPACE = " ";
        internal const string VALUE = "value :";
        internal const string DIMENSION_NOT_MATCH = "Dimension Data did not match.";
        internal const string EXCEED_CHANGE = "Data Limit Exceeded.";
        internal const string CORRECT = "Correct";
        internal const string INCORRECT = "Incorrect";
        #endregion

        #region XML Tags
        internal const string DESCRIPTION = "Description";
        internal const string ISJOBFAIL = "IsJobFail";
        internal const string DIMENSION = "Dimension";
        internal const string UPSTREAM = "Upstream";
        internal const string MAQMART = "Maqmart";
        internal const string COMMENTS = "Comments";
        internal const string THRESHOLD_CHECK = "ThreshholdCheck";
        internal const string SPELL_CHECK = "SpellCheck";
        internal const string THRESHOLD = "Threshhold";
        internal const string EXPECTED_OUTPUT = "ExpectedOutput";
        internal const string EXPECTED_RESULT = "ExpectedResult";
        internal const string CHECK_FOR = "CheckFor";
        #endregion

        #region Mail Constants
        internal const string MAIL_SMTP = "smtp.office365.com";
        internal const string MAIL_RECIPENTS = "MailRecipents";
        internal const string CC_RECIPENT = "CCRecipent";
        internal const string TABLE_ROW = "<tr>";
        internal const string NEW_LINE = "<br>";
        internal const string TABLE_ROW_END = "</tr>";
        internal const string TABLE_CELL = "<td>";
        internal const string TABLE_CELL_QUERY = "<td style=\"width:50%\">";
        internal const string TABLE_CELL_END = "</td>";
        internal const string TABLE_CELL_RED = "<td style=\"color: red;\">";
        internal const string TABLE_CELL_GREEN = "<td style=\"color: green;\">";
        internal const string TABLE_CELL_ORANGE = "<td style=\"color: orange;\">";
        internal const string TABLE_END = "</table>";
        internal const string LIST_ITEM = "<li>";
        internal const string LIST_ITEM_END = "</li>";
        internal const string UNDERLINE = "<u style=\"color:red\">";
        internal const string UNDERLINE_END = "</u>";
        internal const string HTML_HEAD = "<head><style>table { border-collapse: collapse; } td, th { border: 1px solid black; text-align: left; padding: 8px; }</style></head>";
        internal const string HEADER_TEMPLATE = "<h2 style=\"text-align:center;font-family:Segoe UI\">{0}</h2>";
        internal const string TABLE_HEADERS = "<table><tr><th>#</th><th>Testcase</th><th>UpStream Query</th><th>MAQMart Query</th><th>Status</th><th>Comments</th></tr>";
        internal const string FOOTER = "<p stylr=\"font-family:Segoe UI;color:#1884D2\">Thanks,<br>Pre-check Utility Tool";
        internal const string LOG_MAIL_BODY = "BVT Tool execution log";
        internal const string LOG_MAIL_SUBJECT = "Execution report for BVT Tool";
        internal const string FAILURE = "[Failure]: ";
        internal const string NEEDS_ATTENTION = "Needs Attention";
        internal const string CLIENT_ID = "ClientID";
        internal const string CLIENT_SECRET = @"ClientSecret";
        internal const string SECRET_URI = @"SecretUrl";
        internal const string FROM_USER_ID = "FromUserID";
        internal const string FROM_USER_PASSWORD = "FromUserPassword"; // [SuppressMessage("Microsoft.Security", "CS001:SecretInline", Justification="FalsePositive")]
        internal const char SPLIT_COMMA = ',';
        internal const string RESULT_MAIL_SUBJECT = "[{0}]: Precheck Utility Result";
        internal const string DATE_FORMAT = "dddd, dd MMMM yyyy";
        internal const string MAIL_HEADER = "Precheck Utility Notification on: ";
        internal const string NOT_FOUND = "Not Found.";
        #endregion
    }
}
