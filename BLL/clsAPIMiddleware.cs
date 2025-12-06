using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using ASKPA.API.BLL;

namespace ASKPA.BLL
{
    public class APIMiddleware
    {
        private readonly RequestDelegate _next;
          
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public APIMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DBAdmin");
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            string url = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
            string method = context.Request.Method;
            string userAgent = context.Request.Headers["User-Agent"].ToString();
            string ip = context.Connection.RemoteIpAddress?.ToString();
            string businessId = context.Request.Query["HexKey"];

            if (string.IsNullOrWhiteSpace(businessId) && context.Request.HasFormContentType)
            {
                var form = await context.Request.ReadFormAsync();
                businessId = form["HexKey"];
            }
            

            if (!string.IsNullOrWhiteSpace(businessId))
            {
                var objComp = await clsBusiness.Business_DetailAsync(_connectionString, businessId);
                context.Items["CompanyConnection"] = objComp?.DataBaseConnection ?? string.Empty;
                context.Items["BusinessId"] = businessId ?? string.Empty;

            }
            else
            {
                context.Items["AdminConnection"] = _connectionString as String;
                businessId = " ";
            }

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        await conn.OpenAsync();
                        using (SqlCommand cmd = new SqlCommand("PRC_LogRequest", conn))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Url", url);
                            cmd.Parameters.AddWithValue("@HttpMethod", method);
                            cmd.Parameters.AddWithValue("@UserAgent", userAgent ?? "");
                            cmd.Parameters.AddWithValue("@IPAddress", ip ?? "");
                            cmd.Parameters.AddWithValue("@Execution", stopwatch.ElapsedMilliseconds);
                            cmd.Parameters.AddWithValue("@Company", businessId);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch (Exception)
                {
                    //Console.WriteLine("Logging error: " + logEx.Message);

                }
            }
        }
    }
}
