using ASKPA.API.Common;
using ASKPA_API.Model;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
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

        public static async Task<List<clsConfigList>> Company_List(string CompanyConnection)
        {
            List<clsConfigList> mList = new List<clsConfigList>();
            DataTable dt = await clsHelperDBAsync.fnDataTableAsync(CompanyConnection, "PRC_CompanyList");
            foreach (DataRow dr in dt.Rows)
            {
                clsConfigList obj = new clsConfigList();
                obj.IDCompany = clsHelperDBUtility.fnConvert2Long(dr["IDCompany"]);
                obj.MerchantCode = dr["MerchantCode"].ToString();
                mList.Add(obj);
            }
            return mList;
        }

    }
}
