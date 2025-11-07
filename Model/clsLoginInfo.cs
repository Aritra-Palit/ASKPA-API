using DocumentFormat.OpenXml.Math;
using System;
namespace ASKPA.API.Model
{
    //public class clsLogindataInfo
    //{
    //    public string ColumnData { get; set; }
    //    public string UserID { get; set; } = "";
    //}
    
    //public class clsLoginConditionAritra
    //{
    //    public bool Result { get; set; }
    //}
    //public class clsLoginData
    //{
    //    public string result { get; set; } = "";
    //    public long IDUser { get; set; } = 0;
    //    public string Name { get; set; } = "";
    //    public string Gender { get; set; } = "";
    //    public string Email { get; set; } = "";
    //    public string Mobile { get; set; } = "";
    //    public string HexKey { get; set; } = "";
    //    public string Username { get; set; } = "";
    //}
    public class clsLoginInfo
    {
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
    }
    public class clsLoginResultInfo
    {
        public string Result { get; set; } = "";
        public long IDUser { get; set; } = 0;
        public string UserFullName { get; set; } = "";
        public string UserName { get; set; } = "";
        public string UserType { get; set; } = "";
        public string UserGender { get; set; } = "";
        public string UserEmail { get; set; } = "";
        public string UserMobile { get; set; } = "";
        public long IDCompany { get; set; } = 0;
        public string CompanyCode { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public string CompanyAddress { get; set; } = "";
        public string CompanyMobile { get; set; } = "";
        public string CompanyEmail { get; set; } = "";
        public string DashboardURL { get; set; } = "";
        public string APIURL { get; set; } = "";
        public string CompanyHexKey { get; set; } = "";

    }
}
