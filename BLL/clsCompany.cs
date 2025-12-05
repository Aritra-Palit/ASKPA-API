using ASKPA.API.Common;
using ASKPA_API.Model;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Threading.Tasks;

namespace ASKPA_API.BLL
{
    public class clsCompany
    {
        public static async Task<string> NewCompany(string CompanyConnection, clsCompanyInfo info)
        {
            return await clsHelperDBAsync.fnDBOperationAsync(CompanyConnection, "PRC_CompanyOnboarding",
               info.Code, info.Name, info.AliasName, info.Address1, info.Address2, info.State, info.Country, 
               info.Pincode, info.Phone, info.Mobile, info.Email, info.Contactno, info.ContactPerson, info.Website, 
               info.PANNO, info.CINNO,  info.GSTNO, info.EntryUser, info.Active, info.Demo);
        }

        public static async Task<List<clsCompanyList>> Company_List(string CompanyConnection)
        {
            List<clsCompanyList> mList = new List<clsCompanyList>();
            DataTable dt = await clsHelperDBAsync.fnDataTableAsync(CompanyConnection, "PRC_CompanyList");
            foreach (DataRow dr in dt.Rows)
            {
                clsCompanyList obj = new clsCompanyList();
                obj.IDCompany = clsHelperDBUtility.fnConvert2Long(dr["IDCompany"]);
                obj.MerchantCode = dr["MerchantCode"].ToString();
                mList.Add(obj);
            }
            return mList;
        }
        public static async Task<List<clsConfigList>> Config_List(string CompanyConnection, int IDCompany)
        {
            List<clsConfigList> mList = new List<clsConfigList>();
            DataTable dt = await clsHelperDBAsync.fnDataTableAsync(CompanyConnection, "PRC_ConfigList", IDCompany);
            foreach (DataRow dr in dt.Rows)
            {
                clsConfigList obj = new clsConfigList();
                obj.Twostep = clsHelperDBUtility.fnConvert2Int(dr["Twostep"]);
                obj.NoofUsers = clsHelperDBUtility.fnConvert2Long(dr["NoofUsers"]);
                obj.OtpTimer = clsHelperDBUtility.fnConvert2Long(dr["OtpTimer"]);
                obj.S_Email = clsHelperDBUtility.fnConvert2Int(dr["S_Email"]);
                obj.S_Phone = clsHelperDBUtility.fnConvert2Int(dr["S_Phone"]);
                obj.S_Whatsapp = clsHelperDBUtility.fnConvert2Int(dr["S_Whatsapp"]);
                obj.VersionName = dr["VersionName"].ToString();
                obj.FolderName = dr["FolderName"].ToString();
                obj.StartDate = DateOnly.FromDateTime(Convert.ToDateTime(dr["StartDate"]));
                obj.EndDate = DateOnly.FromDateTime(Convert.ToDateTime(dr["EndDate"]));
                obj.NotificationDate = DateOnly.FromDateTime(Convert.ToDateTime(dr["NotificationDate"]));
                obj.RenewDate = DateOnly.FromDateTime(Convert.ToDateTime(dr["RenewDate"]));
                obj.LastRenewDate = DateOnly.FromDateTime(Convert.ToDateTime(dr["LastRenewDate"]));
                obj.DashboardURL = dr["DashboardURL"].ToString();
                obj.APIURL = dr["APIURL"].ToString();
                mList.Add(obj);
            }
            return mList;
        }

    }
}
