using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASKPA.API.Common;
using ASKPA.API.Model;

namespace ASKPA.API.BLL
{
    public static class clsBusiness
    {
        public static async Task<clsBusinessInfo> Business_DetailAsync(string connectionString, string businessId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@BusinessId", businessId ?? "")
                };
                var result = await clsHelperDB.DBListAsync<clsBusinessInfo>(connectionString, "CRM_Services_Business_Detail", parameters);
                return result.FirstOrDefault() ?? new clsBusinessInfo();
            }
            catch (Exception)
            {
                return new clsBusinessInfo();
            }
        }
    }
}
