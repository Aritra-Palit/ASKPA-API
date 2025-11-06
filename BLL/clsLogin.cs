using ASKPA.API.Common;
using ASKPA.API.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ASKPA.API.BLL
{
    public static class clsLogin
    {
        //public static async Task<clsLoginData> LoginCheckAsync(string connectionString, clsLoginInfo info)
        //{
        //    var obj = new clsLoginData();
        //    try
        //    {
        //        var parameters = new List<SqlParameter>
        //        {
        //            new SqlParameter("@DataValue", info.ColumnData ?? ""),
        //            new SqlParameter("@UserID", info.UserID ?? "")
        //        };

        //        var result = await clsHelperDB.DBListAsync<clsLoginData>(connectionString, "PRC_CRM_Logging_Data_Check", parameters);
        //        var row = result.FirstOrDefault();
        //        if (row != null && row.result == "Success")
        //        {
        //            obj.result = "Success";
        //            obj.IDUser = row.IDUser;
        //            obj.Name = row.Name;
        //            obj.Gender = row.Gender;
        //            obj.Email = row.Email;
        //            obj.Mobile = row.Mobile;
        //            obj.HexKey = row.HexKey;
        //        }
        //        else
        //        {
        //            obj.result = "Failure";
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        obj.result = "Failure";
        //    }

        //    return obj;
        //}
        public static async Task<clsLoginResultInfo> Login_Validation(string AdminConnection, clsLoginInfo info)
        {
            clsLoginResultInfo obj = new clsLoginResultInfo();
            DataTable dt = await clsHelperDBAsync.fnDataTableAsync(AdminConnection, "PRC_Login_Validation", info.UserName, info.Password);
            foreach (DataRow dr in dt.Rows)
            {
                obj.IDUser = clsHelperDBUtility.fnConvert2Long(dr["IDUser"]);
                obj.UserFullName = dr["UserFullName"].ToString();
                obj.UserName = dr["UserName"].ToString();
                obj.UserType= dr["UserType"].ToString();
                obj.UserMobile = dr["UserMobile"].ToString();
                obj.UserGender= dr["UserGender"].ToString();
                obj.UserEmail= dr["UserEmail"].ToString();
                obj.IDCompany = clsHelperDBUtility.fnConvert2Long(dr["IDCompany"]);
                obj.CompanyCode = dr["CompanyCode"].ToString();
                obj.CompanyName = dr["CompanyName"].ToString(); 
                obj.CompanyAddress = dr["CompanyAddress"].ToString();
                obj.CompanyMobile = dr["CompanyMobile"].ToString();
                obj.CompanyEmail = dr["CompanyEmail"].ToString();
                obj.CompanyHexKey = dr["CompanyHexKey"].ToString();
                obj.Result = "Success";
            }
            return obj;
        }
    }
}
