using DocumentFormat.OpenXml.InkML;
using ASKPA.API.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ASKPA.API.Common
{
    public static class clsHelperDB
    {
        public static async Task<List<T>> DBListAsync<T>(
            string connectionString,
            string storedProcedureName,
            List<SqlParameter> parameters) where T : class
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();
                optionsBuilder.UseSqlServer(connectionString);

                await using var context = new AppDBContext(optionsBuilder.Options);
                await context.Database.OpenConnectionAsync();

                try
                {
                    // Build SQL with parameters
                    string paramNames = string.Join(", ", parameters.ConvertAll(p => p.ParameterName));
                    string sql = $"EXEC {storedProcedureName} {paramNames}";

                    // Execute and map to entity
                    return await context.Set<T>().FromSqlRaw(sql, parameters.ToArray()).ToListAsync();
                }
                finally
                {
                    await context.Database.CloseConnectionAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ExecuteStoredProcedureAsync error: {ex.Message}");
                return new List<T>();
            }
        }

        public static async Task<int> DBOperationAsync(
            string connectionString,
            string storedProcedureName,
            List<SqlParameter> parameters,
            DataTable tableParam = null,
            string tableParamName = null,
            string tableTypeName = null,
            DataSet dataSetParam = null,
            Dictionary<string, string> dataSetTableTypes = null
        )
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();
                optionsBuilder.UseSqlServer(connectionString);

                await using var context = new AppDBContext(optionsBuilder.Options);
                await context.Database.OpenConnectionAsync();

                try
                {
                    if (tableParam != null && !string.IsNullOrEmpty(tableParamName) && !string.IsNullOrEmpty(tableTypeName))
                    {
                        parameters.Add(new SqlParameter(tableParamName, tableParam)
                        {
                            SqlDbType = SqlDbType.Structured,
                            TypeName = tableTypeName
                        });
                    }
                    if (dataSetParam != null && dataSetTableTypes != null)
                    {
                        foreach (DataTable dt in dataSetParam.Tables)
                        {
                            if (dataSetTableTypes.TryGetValue(dt.TableName, out string typeName))
                            {
                                parameters.Add(new SqlParameter("@" + dt.TableName, dt)
                                {
                                    SqlDbType = SqlDbType.Structured,
                                    TypeName = typeName
                                });
                            }
                        }
                    }

                    string paramNames = string.Join(", ", parameters.ConvertAll(p => p.ParameterName));
                    string sql = $"EXEC {storedProcedureName} {paramNames}";

                    return await context.Database.ExecuteSqlRawAsync(sql, parameters.ToArray());
                }
                finally
                {
                    await context.Database.CloseConnectionAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ExecuteNonQueryAsync error: {ex.Message}");
                return -1;
            }
        }



        //public static async Task<string> DBOperationAsync(string ConnectionString, string SPName , List<SqlParameter> SQLParameters)
        //{
        //    try
        //    {
        //        var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();
        //        optionsBuilder.UseSqlServer(ConnectionString);
        //        await using var context = new AppDBContext(optionsBuilder.Options);
        //        await context.Database.OpenConnectionAsync();
        //        string paramNames = string.Join(", ", SQLParameters.ConvertAll(p => p.ParameterName));
        //        string sql = $"EXEC {SPName} {paramNames}";
        //        return await context.Database.ExecuteSqlRawAsync(sql, SQLParameters.ToArray());
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"ExecuteNonQueryAsync error: {ex.Message}");
        //        return -1;
        //    }
        //}
        //public static List<SqlParameter> GetStoredProcParameters(string connectionString, string storedProcedureName, Dictionary<string, object> parameterValues)
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        conn.Open();

        //        // Fetch all parameters for the stored procedure
        //        SqlCommandBuilder.DeriveParameters(cmd);

        //        var parameters = new List<SqlParameter>();

        //        foreach (SqlParameter p in cmd.Parameters)
        //        {
        //            if (p.ParameterName == "@RETURN_VALUE")
        //                continue; // skip return value param

        //            // Try to assign value from dictionary
        //            if (parameterValues.ContainsKey(p.ParameterName))
        //            {
        //                p.Value = parameterValues[p.ParameterName] ?? DBNull.Value;
        //            }
        //            else
        //            {
        //                p.Value = DBNull.Value; // default if not provided
        //            }

        //            parameters.Add(p);
        //        }

        //        return parameters;
        //    }
        //}


        public static List<SqlParameter> GetParameters(string ConnectionString, string SPName)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(SPName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlCommandBuilder.DeriveParameters(cmd);
                var parameters = new List<SqlParameter>();
                foreach (SqlParameter para in cmd.Parameters)
                {
                    if (para.ParameterName != "@RETURN_VALUE")// skip return value param
                    {
                        parameters.Add(new SqlParameter(para.ParameterName, para.DbType));

                    }
                }
                return parameters;
            }
        }


    }
    public static class clsHelperDBDecrypt
    {
        public static string DecryptWithRSA(string base64Data, RSA rsa)
        {
            try
            {
                var encryptedBytes = Convert.FromBase64String(base64Data);
                var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Invalid Base64 input: {ex.Message}");
                return null;
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine($"Decryption error: {ex.Message}");
                return null;
            }
        }
    }
    public class clsHelperDBAPI
    {
        private string APIBaseURL { get; set; } = "";
        public clsHelperDBAPI(string _APIBaseURL)
        {
            APIBaseURL = _APIBaseURL;
        }
        public HttpResponseMessage APIPost(string APIMethod, string contents)
        {
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(APIBaseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = client.PostAsync(APIMethod, new StringContent(contents, Encoding.UTF8, "application/json")).Result;
            }
            return response;
        }
        public HttpResponseMessage APIPost(string APIMethod)
        {
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(APIBaseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = client.PostAsync(APIMethod, new StringContent("", Encoding.UTF8, "application/json")).Result;
            }
            return response;
        }
        public HttpResponseMessage APIGet(string APIMethod)
        {
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(APIBaseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = client.GetAsync(APIMethod).Result;
            }
            return response;
        }
    }
    public class clsHelperDBUtility
    {
        public enum WeekDays
        {
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
            Sunday = 7
        }
        private class clsMonths
        {
            public int ID { get; set; } = 0;
            public string name { get; set; } = "";
        }
        public static String MonthsName(int Monthno)
        {
            List<clsMonths> mlist = new List<clsMonths>();
            String[] MonthsName = { "JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY", "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER", "NOVEMBER", "DECEMBER" };
            for (int index = 1; index <= 12; index++)
            {
                clsMonths obj = new clsMonths();
                obj.ID = index;
                obj.name = MonthsName[index - 1];
                mlist.Add(obj);
            }
            return mlist[Monthno - 1].name.ToString();
        }
        public static String[] CurrentMonthsYear()
        {
            int year = System.DateTime.Now.Year;
            String[] MonthsName = { "JANUARY" + "-" + year.ToString(),
                                            "FEBRUARY" + "-" + year.ToString(),
                                            "MARCH" + "-" + year.ToString(),
                                            "APRIL" + "-" + year.ToString(),
                                            "MAY" + "-" + year.ToString(),
                                            "JUNE" + "-" + year.ToString(),
                                            "JULY" + "-" + year.ToString(),
                                            "AUGUST" + "-" + year.ToString(),
                                            "SEPTEMBER" + "-" + year.ToString(),
                                            "OCTOBER" + "-" + year.ToString(),
                                            "NOVEMBER" + "-" + year.ToString(),
                                            "DECEMBER"  + "-" + year.ToString()
                                    };
            return MonthsName;
        }
        public static string ConvertAmountToINR(double dblAmount, bool boolUseShortFormat = false) //string strAmount
        {
            string strFormattedAmount = "";
            if (boolUseShortFormat == false)
            {
                strFormattedAmount = dblAmount.ToString("#,0.00", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
            }
            else
            {
                string strAmt = "", strAmtPart1 = "", strAmtPart2 = "";
                double dblAmtPart1 = 0, dblAmtPart2 = 0;

                // Displays 123,45,67,890   
                if (dblAmount < 1000)
                    strFormattedAmount = dblAmount.ToString("#,0.00", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                // Displays 123,45,68K
                else if (dblAmount >= 1000 && dblAmount < 100000)
                    strFormattedAmount = dblAmount.ToString("#,#,K", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));//InvariantCulture

                // Displays 123,5L
                else if (dblAmount >= 100000 && dblAmount < 10000000)
                {
                    strAmt = dblAmount.ToString();
                    strAmtPart1 = strAmt.Substring(0, (strAmt.Length - 5));
                    strAmtPart2 = strAmt.Substring((strAmt.Length - 5), 5);

                    dblAmtPart1 = Convert.ToDouble(strAmtPart1);
                    dblAmtPart2 = Convert.ToDouble(strAmtPart2);

                    if (dblAmtPart2 > 55999)
                    {
                        dblAmtPart1 = dblAmtPart1 + 1;
                    }

                    strAmtPart1 = dblAmtPart1.ToString("#,#", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                    strFormattedAmount = strAmtPart1 + "L";
                }
                // Displays 123C
                else if (dblAmount >= 10000000)
                {
                    strAmt = dblAmount.ToString();
                    strAmtPart1 = strAmt.Substring(0, (strAmt.Length - 7));
                    strAmtPart2 = strAmt.Substring((strAmt.Length - 7), 7);

                    dblAmtPart1 = Convert.ToDouble(strAmtPart1);
                    dblAmtPart2 = Convert.ToDouble(strAmtPart2);

                    if (dblAmtPart2 > 5599999)
                    {
                        dblAmtPart1 = dblAmtPart1 + 1;
                    }

                    strAmtPart1 = dblAmtPart1.ToString("#,#", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));

                    strFormattedAmount = strAmtPart1 + "C";
                }
            }

            return strFormattedAmount;
        }
        public static long fnConvert2Long(object pText)
        {
            try
            {
                if (pText == null || pText == String.Empty || pText == "{}")
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(pText);
                }
            }
            catch
            {
                return 0;
            }
        }
        public static int fnConvert2Int(object pText)
        {
            try
            {
                if (pText == null || pText == string.Empty || pText == "{}")
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt16(pText);
                }
            }
            catch
            {
                return 0;
            }
        }
        public static decimal fnConvert3Decimal(object pText)
        {
            try
            {
                if (pText == null || pText == "" || pText == "{}")
                {
                    return 0;
                }
                else
                {
                    //return Convert.ToDecimal(pText).ToString("0.00");
                    return Convert.ToDecimal(Convert.ToDecimal(pText).ToString("0.000"));
                }
            }
            catch
            {
                return 0;
            }
        }
        public static decimal fnConvert2Decimal(object pText)
        {
            try
            {
                if (pText == null || pText == "" || pText == "{}")
                {
                    return 0;
                }
                else
                {
                    //return Convert.ToDecimal(pText).ToString("0.00");
                    return Convert.ToDecimal(Convert.ToDecimal(pText).ToString("0.00"));
                }
            }
            catch
            {
                return 0;
            }
        }
        public static double fnConvert2Double(object pText)
        {
            try
            {
                if (pText == null || pText.ToString() == "" || pText.ToString() == "{}")
                {
                    return 0;
                }
                else
                {
                    //return Convert.ToDecimal(pText).ToString("0.00");
                    return Convert.ToDouble(pText);
                }
            }
            catch
            {
                return 0;
            }
        }
        public static string fnConvert2String(String pText)
        {
            try
            {
                if (String.IsNullOrEmpty(pText))
                {
                    return "";
                }
                else
                {
                    return Convert.ToString(pText);
                }
            }
            catch
            {
                return "";
            }
        }
        public static String fnConvert2PascalWithSpace(String pText)
        {
            String mText = pText;
            if (mText != "")
            {
                mText = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(mText.ToLower());
            }
            return mText;
        }
        public static String fnConvert2StartCapital(String pText)
        {
            String mText = pText;
            if (mText != "")
            {
                return char.ToUpper(mText[0]) + pText.Substring(1).ToLower();
            }
            return mText;
        }
        public static bool fnIsNumeric(string pText)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(pText);
        }
        public static string fnAutoNumber()
        {
            String mAutoNo = System.DateTime.Now.ToString("ddmmyy");
            return mAutoNo;
        }
        public static String fnAutoNumber(int pdigit)
        {
            Random generator = new Random();
            String mAutoNo = Convert.ToString(generator.Next(0, 1000000).ToString("D6"));
            return mAutoNo;
        }
        public static String fnAutoNumber8()
        {
            Random generator = new Random();
            String mAutoNo = Convert.ToString(generator.Next(0, 100000000).ToString("D8"));
            return mAutoNo;
        }
        public static string SplitQueryString(string QueryString)
        {

            string strReq = "";
            try
            {

                strReq = QueryString;
                strReq = strReq.Substring(strReq.IndexOf('?') + 1);

                return strReq;
            }
            catch (Exception ex)
            {
                strReq = ex.Message.ToString();
                return strReq;
            }
            finally { }
        }
        public static string SplitQueryString(string QueryString, char SplitType)
        {

            string strReq = "";
            try
            {

                strReq = QueryString;
                strReq = strReq.Substring(strReq.IndexOf(SplitType) + 1);

                return strReq;
            }
            catch (Exception ex)
            {
                strReq = ex.Message.ToString();
                return strReq;
            }
            finally { }
        }
        public static Boolean fnConvert2Boolean(String pText)
        {
            try
            {
                if (String.IsNullOrEmpty(pText))
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(pText);
                }
            }
            catch
            {
                return false;
            }
        }
        public Boolean fnValidEmail(String pEmail)
        {
            String mPattern = "^[a-zA-Z0-9][-\\._a-zA-Z0-9]*@[a-zA-Z0-9][-\\.a-zA-Z0-9]*\\.(com|edu|info|gov|int|mil|net|org|biz|name|museum|coop|aero|pro|tv|[a-zA-Z]{2})$";
            System.Text.RegularExpressions.Regex mCheck = new System.Text.RegularExpressions.Regex(mPattern, RegexOptions.IgnorePatternWhitespace);
            Boolean mValid = false;
            if (String.IsNullOrEmpty(pEmail) == true)
            {
                mValid = true;
            }
            else
            {
                mValid = mCheck.IsMatch(pEmail);
            }
            return mValid;
        }
        public static decimal[] fnRoundoff(decimal netamt)
        {
            decimal[] myarray = new decimal[2];
            decimal roundoff = 0;
            decimal double_result = (decimal)((netamt - (decimal)((long)netamt)));
            if (double_result != 0)
            {
                if (double_result >= (decimal)(0.5))
                {
                    roundoff = (1 - double_result);
                }
                else
                {
                    roundoff = -(double_result);
                }
            }
            else
            {
                roundoff = 0;
            }
            myarray[0] = roundoff;
            myarray[1] = (netamt + roundoff);
            return myarray;
        }
        public static decimal fnCeilingFloor(Decimal decvalue)
        {
            decimal trnvalue = 0;
            int mint = (int)(decvalue);
            float floatpart = ((float)decvalue - mint);
            if (floatpart >= 0.5)
            {
                trnvalue = Math.Ceiling(decvalue);
            }
            else
            {
                trnvalue = Math.Floor(decvalue);
            }

            return trnvalue;
        }
        public static String fnAutoNumber4()
        {
            Random generator = new Random();
            String mAutoNo = Convert.ToString(generator.Next(0, 1000000).ToString("D6"));
            return mAutoNo;
        }
    }
    public class clsHelperDBEmail
    {
        public static string fnSendEmail(string SMTPServer, Boolean EnableSSL_TrueFalse, string Username, string Password, int SMTP_Port, string mfromEmail, string mToEmail, string mSubject, string mBody)
        {
            string EmailStatus = "";
            try
            {
                using (MailMessage mm = new MailMessage())
                {
                    mm.From = new MailAddress(mfromEmail);
                    mm.To.Add(mToEmail);
                    mm.Subject = mSubject;
                    mm.Body = mBody;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = SMTPServer;
                    smtp.EnableSsl = EnableSSL_TrueFalse;
                    NetworkCredential NetworkCred = new NetworkCredential(Username, Password, SMTPServer);

                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = SMTP_Port;
                    smtp.Send(mm);
                    EmailStatus = "";
                    return EmailStatus;
                }
            }
            catch (Exception ex)
            {
                EmailStatus = ex.Message.ToString();
                return EmailStatus;
            }
            finally { }
        }
        //public static string EmailHelper(String calledFrom, String smtpServer, int smtpPort, bool SSL, String password, String fromEmail, String toEmail, String emailSubject, String emailBody, String attachmentPath, String attachmentName)
        //{
        //    string error = "";
        //    try
        //    {
        //        NetworkCredential login = new NetworkCredential(fromEmail, password);
        //        SmtpClient smtp = new SmtpClient(smtpServer);
        //        smtp.Port = smtpPort;
        //        smtp.EnableSsl = SSL;
        //        smtp.Credentials = login;

        //        MailMessage msg = new MailMessage { From = new MailAddress(fromEmail) };
        //        msg.To.Add(new MailAddress(toEmail));
        //        msg.Subject = emailSubject;
        //        msg.Body = emailBody;

        //        System.Net.Mail.Attachment attachment;
        //        String Path = HttpContext.Current.Server.MapPath(attachmentPath);
        //        attachment = new System.Net.Mail.Attachment(Path);
        //        msg.Attachments.Add(attachment);

        //        msg.IsBodyHtml = false;
        //        msg.Priority = MailPriority.Normal;
        //        msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
        //        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        //        smtp.Send(msg);
        //        error = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        error = "Issue";
        //        clsDatabase.fnErrorLog(calledFrom, ex.Message.ToString());
        //    }
        //    return error;
        //}

    }
    public class clsHelperDBSync
    {

        public static string DBConnection(String CString)
        {
            string result = "";
            SqlConnection mCon = new SqlConnection(CString);
            try
            {
                mCon.Open();
                result = "SUCCESS";
            }
            catch (SqlException err)
            {
                result = err.Message;
                fnErrorLog("apiMETHOD:DBConnection", err);
            }
            finally
            {
                mCon.Close();
            }
            return result;

        }
        public static String fnDBOperation(String mConnection, String pSPName, params object[] pParaValue)
        {
            SqlConnection mCon = new SqlConnection(mConnection);
            string Result = "";
            string errSourceName = "";
            try
            {
                mCon.Open();
                SqlCommand cmd = new SqlCommand(pSPName, mCon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlCommandBuilder.DeriveParameters(cmd);
                cmd.Parameters.RemoveAt(0);
                SqlParameter[] pParaName = new SqlParameter[cmd.Parameters.Count];
                cmd.Parameters.CopyTo(pParaName, 0);
                cmd.Parameters.Clear();
                Result = fnAddParaValue(cmd, pParaName, pParaValue);
                if (Result == "") // Parameter value passed
                {
                    return Convert.ToString(cmd.ExecuteScalar());
                }
                else
                {
                    return Result;
                }
            }
            catch (Exception e)
            {
                Result = fnError(e);
                // Logging
                errSourceName = "DB Connection: " + mConnection + Environment.NewLine + "API Method:" + pSPName;
                fnErrorLog(errSourceName, e);
                return Result;
            }
            finally
            {
                mCon.Close();
            }
        }
        private static string fnAddParaValue(SqlCommand pCom, SqlParameter[] pParaName, object[] pParaValue)
        {
            if ((pCom == null))
            {
                return "SQL Command initialization issue...";
            }
            if (((pParaName == null) | (pParaValue == null)))
            {
                return "SQL Command Parameter initialization issue...";
            }
            if ((pParaName.Length != pParaValue.Length))
            {
                return "SQL Command Parameter length size issue...";
            }
            for (int index = 0; index <= pParaName.Length - 1; index++)
            {

                pCom.Parameters.AddWithValue(pParaName[index].ParameterName, pParaValue[index]);
            }
            return "";
        }
        public static DataSet fnDataSet(String mConnection, String pSPName, params object[] pParaValue)
        {
            SqlConnection mCon = new SqlConnection(mConnection);
            DataSet DS = new DataSet("Data");
            string Result = "";
            string errSourceName = "";
            try
            {
                mCon.Open();
                SqlCommand mCom = new SqlCommand(pSPName, mCon);
                mCom.CommandType = CommandType.StoredProcedure;
                SqlCommandBuilder.DeriveParameters(mCom);
                mCom.Parameters.RemoveAt(0);
                SqlParameter[] pParaName = new SqlParameter[mCom.Parameters.Count];
                mCom.Parameters.CopyTo(pParaName, 0);
                mCom.Parameters.Clear();
                if ((pParaName.Length > 0))
                {
                    for (int index = 0; index <= pParaName.Length - 1; index++)
                    {
                        mCom.Parameters.AddWithValue(pParaName[index].ParameterName, pParaValue[index]);
                    }
                }
                SqlDataAdapter DAP = new SqlDataAdapter(mCom);
                DAP.Fill(DS);
                return DS;
            }
            catch (Exception e)
            {
                Result = fnError(e);
                // Logging
                errSourceName = "DB Connection: " + mConnection + Environment.NewLine + "API Method:" + pSPName;
                fnErrorLog(errSourceName, e);
                return DS;
            }
            finally
            {
                mCon.Close();
            }
        }
        public static DataTable fnDataTable(String mConnection, String pSPName)
        {
            SqlConnection mCon = new SqlConnection(mConnection);
            DataTable DT = new DataTable("Data");
            string Result = "";
            string errSourceName = "";
            try
            {
                mCon.Open();
                SqlCommand mCom = new SqlCommand(pSPName, mCon);
                mCom.CommandType = CommandType.StoredProcedure;
                DT.Load(mCom.ExecuteReader());
                return DT;
            }
            catch (Exception e)
            {
                Result = fnError(e);
                // Logging
                errSourceName = "DB Connection: " + mConnection + Environment.NewLine + "API Method:" + pSPName;
                fnErrorLog(errSourceName, e);
                return DT;
            }
            finally
            {
                mCon.Close();
            }
        }
        public static DataTable fnDataTable(String mConnection, String pSPName, params object[] pParaValue)
        {
            SqlConnection mCon = new SqlConnection(mConnection);
            DataTable DT = new DataTable("Data");
            string Result = "";
            string errSourceName = "";
            try
            {
                mCon.Open();
                SqlCommand mCom = new SqlCommand(pSPName, mCon);
                mCom.CommandType = CommandType.StoredProcedure;
                SqlCommandBuilder.DeriveParameters(mCom);
                mCom.Parameters.RemoveAt(0);
                SqlParameter[] pParaName = new SqlParameter[mCom.Parameters.Count];
                mCom.Parameters.CopyTo(pParaName, 0);
                mCom.Parameters.Clear();
                if ((pParaName.Length > 0))
                {
                    for (int index = 0; index <= pParaName.Length - 1; index++)
                    {
                        mCom.Parameters.AddWithValue(pParaName[index].ParameterName, pParaValue[index]);
                    }
                }
                DT.Load(mCom.ExecuteReader());
                return DT;
            }
            catch (Exception e)
            {
                Result = fnError(e);
                // Logging
                errSourceName = "DB Connection: " + mConnection + Environment.NewLine + "API Method:" + pSPName;
                fnErrorLog(errSourceName, e);
                return DT;
            }
            finally
            {
                mCon.Close();
            }
        }
        public static string fnError(Exception pError)
        {
            String mMessage = "";
            mMessage += "Error Message : " + pError.Message.ToString() + Environment.NewLine;
            return mMessage;
        }

        //public static void fnErrorLog(String SPName, String Result)
        //{
        //    String path = Path.Combine(Directory.GetCurrentDirectory().ToString(), "Errros");
        //    String mError = "Date & Time : " + DateTime.Now + Environment.NewLine +
        //                    "SP Name :  " + SPName + Environment.NewLine +
        //                    "Message :  " + Result + Environment.NewLine +
        //                    "----------END--------- " + Environment.NewLine;
        //    try
        //    {
        //        if (File.Exists(path) == false)
        //        {
        //            File.Create(path);
        //        }
        //        File.WriteAllText(path, mError);
        //    }
        //    catch (Exception) { }
        //    finally { }
        //}
        public static void fnErrorLog(String SourceName, Exception Err)
        {
            String path = Path.Combine(Directory.GetCurrentDirectory().ToString(), "wwwroot\\logFiles", "Site.log");
            String mError = "Date & Time : " + DateTime.Now + Environment.NewLine +
                            "Source Name :  " + SourceName + Environment.NewLine +
                            "Message :  " + Err.Message.ToString() + Environment.NewLine +
                            "----------END--------- " + Environment.NewLine;
            try
            {
                if (File.Exists(path) == false)
                {
                    File.Create(path);
                }
                File.WriteAllText(path, mError);
            }
            catch (Exception) { }
            finally { }
        }
    }
    public class clsHelperDBAsync
    {
        public static async Task<string> DBConnection(String CString)
        {
            string result = "";
            SqlConnection mCon = new SqlConnection(CString);
            try
            {
                await mCon.OpenAsync();
                result = "SUCCESS";
            }
            catch (SqlException err)
            {
                result = err.Message;
            }
            finally
            {
                await mCon.CloseAsync();
            }
            return result;

        }
        public static async Task<string> fnDBOperationAsync(String mConnection, String pSPName, params object[] pParaValue)
        {
            using SqlConnection mCon = new SqlConnection(mConnection);
            var mResult = "";
            try
            {
                await mCon.OpenAsync();
                SqlCommand cmd = new SqlCommand(pSPName, mCon);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlCommandBuilder.DeriveParameters(cmd);
                cmd.Parameters.RemoveAt(0);
                SqlParameter[] pParaName = new SqlParameter[cmd.Parameters.Count];
                cmd.Parameters.CopyTo(pParaName, 0);
                cmd.Parameters.Clear();
                mResult = fnAddParaValue(cmd, pParaName, pParaValue);
                if (mResult == "") // Parameter value passed
                {
                    var Result = await cmd.ExecuteScalarAsync();
                    return Convert.ToString(Result);
                }
                else
                {
                    return mResult;
                }
            }
            catch (Exception e)
            {
                mResult = fnError(e);
                return mResult;
            }
            finally
            {
                mCon.Close();
            }
        }
        private static string fnAddParaValue(SqlCommand pCom, SqlParameter[] pParaName, object[] pParaValue)
        {
            if (pParaName.Length != pParaValue.Length)
            {
                return "Parameter count mismatch...";
            }
            for (int index = 0; index <= pParaName.Length - 1; index++)
            {
                pCom.Parameters.AddWithValue(pParaName[index].ParameterName, pParaValue[index]);
            }
            return "";
        }
        public static async Task<DataSet> fnDataSetAsync(string mConnection, string pSPName, params object[] pParaValue)
        {
            SqlConnection mCon = new SqlConnection(mConnection);
            var DS = new DataSet("Data");
            String Result = "";
            try
            {
                await mCon.OpenAsync();
                SqlCommand mCom = new SqlCommand(pSPName, mCon);
                mCom.CommandType = CommandType.StoredProcedure;
                SqlCommandBuilder.DeriveParameters(mCom);
                mCom.Parameters.RemoveAt(0);
                SqlParameter[] pParaName = new SqlParameter[mCom.Parameters.Count];
                mCom.Parameters.CopyTo(pParaName, 0);
                mCom.Parameters.Clear();
                if ((pParaName.Length > 0))
                {
                    for (int index = 0; index <= pParaName.Length - 1; index++)
                    {
                        mCom.Parameters.AddWithValue(pParaName[index].ParameterName, pParaValue[index]);
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter(mCom);
                da.Fill(DS);
                return DS;
            }
            catch (Exception e)
            {
                Result = fnError(e);
                return DS;
            }
            finally
            {
                mCon.Close();
            }
        }
        public static async Task<DataTable> fnDataTableAsync(String mConnection, String pSPName)
        {
            SqlConnection mCon = new SqlConnection(mConnection);
            DataTable DT = new DataTable("Data");
            String mResult = "";
            try
            {
                await mCon.OpenAsync();
                SqlCommand mCom = new SqlCommand(pSPName, mCon);
                mCom.CommandType = CommandType.StoredProcedure;
                await using var reader = await mCom.ExecuteReaderAsync();
                DT.Load(reader, LoadOption.PreserveChanges); // Load reader into datatable 
                return DT;
            }
            catch (Exception e)
            {
                mResult = fnError(e);
                return DT;
            }
            finally
            {
                mCon.Close();
            }
        }
        public static async Task<DataTable> fnDataTableAsync(String mConnection, String pSPName, params object[] pParaValue)
        {
            SqlConnection mCon = new SqlConnection(mConnection);
            DataTable DT = new DataTable("Data");
            String mResult = "";
            try
            {
                await mCon.OpenAsync();
                SqlCommand mCom = new SqlCommand(pSPName, mCon);
                mCom.CommandType = CommandType.StoredProcedure;
                SqlCommandBuilder.DeriveParameters(mCom);
                mCom.Parameters.RemoveAt(0);
                SqlParameter[] pParaName = new SqlParameter[mCom.Parameters.Count];
                mCom.Parameters.CopyTo(pParaName, 0);
                mCom.Parameters.Clear();
                if ((pParaName.Length > 0))
                {
                    for (int index = 0; index <= pParaName.Length - 1; index++)
                    {
                        mCom.Parameters.AddWithValue(pParaName[index].ParameterName, pParaValue[index]);
                    }
                }
                await using var reader = await mCom.ExecuteReaderAsync();
                DT.Load(reader, LoadOption.PreserveChanges); // Load reader into datatable 
                return DT;
            }
            catch (Exception e)
            {
                mResult = fnError(e);
                return DT;
            }
            finally
            {
                mCon.Close();
            }
        }
        public static string fnError(Exception pError)
        {
            String mMessage = "";
            mMessage += "Error Message : " + pError.Message.ToString() + Environment.NewLine;
            return mMessage;
        }
        public static async Task fnErrorLog(String SPName, String Result)
        {
            String path = Path.Combine(Directory.GetCurrentDirectory().ToString(), "Errros");
            String mError = "Date & Time : " + DateTime.Now + Environment.NewLine +
                            "SP Name :  " + SPName + Environment.NewLine +
                            "Message :  " + Result + Environment.NewLine +
                            "----------END--------- " + Environment.NewLine;
            try
            {
                if (File.Exists(path) == false)
                {
                    File.Create(path);
                }
                await File.AppendAllTextAsync(path, mError);
            }
            catch (Exception) { }
            finally { }
        }
    }
}

