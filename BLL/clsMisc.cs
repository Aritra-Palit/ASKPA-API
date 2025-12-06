
using ASKPA.API.Common;
using ASKPA_API.Model;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ASKPA_API.BLL
{
    public static class clsMisc
    {
        public static async Task<List<clsMiscInfo>> Misc_List(string CompanyConnection,string Type)
        {
            List<clsMiscInfo> mList = new List<clsMiscInfo>();
            DataTable dt = await clsHelperDBAsync.fnDataTableAsync(CompanyConnection, "PRC_Misc_List", Type);
            foreach (DataRow dr in dt.Rows)
            {
                clsMiscInfo obj = new clsMiscInfo();
                obj.IDMisc = clsHelperDBUtility.fnConvert2Long(dr["IDMisc"]);
                obj.Code = dr["Code"].ToString();
                obj.Name = dr["Name"].ToString();
                mList.Add(obj);
            }
            return mList;
        }
        public static async Task<string> Misc_Add_Edit(string CompanyConnection, MiscInfo info)
        {
            return await clsHelperDBAsync.fnDBOperationAsync(CompanyConnection, "PRC_Misc_Add_Edit",
               info.IDMisc, info.Code, info.Name, info.Value, info.Type, info.DisplayOrder, info.SystemYN, info.ActiveYN);
        }
        public static async Task<List<MiscInfoList>> MiscInfo_List(string CompanyConnection)
        {
            List<MiscInfoList> mList = new List<MiscInfoList>();
            DataTable dt = await clsHelperDBAsync.fnDataTableAsync(CompanyConnection, "PRC_MiscInfo_List");
            foreach (DataRow dr in dt.Rows)
            {
                MiscInfoList obj = new MiscInfoList();
                obj.IDMisc = clsHelperDBUtility.fnConvert2Long(dr["IDMisc"]);
                obj.Code = dr["Code"].ToString();
                obj.Name = dr["Name"].ToString();
                obj.Type = dr["MiscType"].ToString();
                mList.Add(obj);
            }
            return mList;
        }
        public static async Task<MiscInfo> Misc_Detail(string ComapanyConnection, long IDMisc)
        {
            MiscInfo obj = new MiscInfo();
            DataTable dt = await clsHelperDBAsync.fnDataTableAsync(ComapanyConnection, "PRC_MiscInfo_Details", IDMisc);
            foreach (DataRow dr in dt.Rows)
            {
                obj.IDMisc = clsHelperDBUtility.fnConvert2Long(dr["IDMisc"]);
                obj.Code = dr["Code"].ToString();
                obj.Name = dr["Name"].ToString();
                obj.Value = clsHelperDBUtility.fnConvert2Long(dr["Value"]);
                obj.Type = dr["MiscType"].ToString();
                obj.DisplayOrder = clsHelperDBUtility.fnConvert2Long(dr["DisplayOrder"]);
                obj.SystemYN = dr["SystemYN"].ToString();
                obj.ActiveYN = dr["ActiveYN"].ToString();
            }
            return obj;

        }
        //public static async Task<List<clsMiscInfo>> Misc_ListAsync(string connectionString, RequestType type)
        //{
        //    var parameters = new List<SqlParameter>
        //    {
        //        new SqlParameter("@Type", type.Type)
        //    };
        //    return await clsHelperDB.DBListAsync<clsMiscInfo>(connectionString, "PRC_CRMServices_Misc", parameters);
        //}
    }
}
