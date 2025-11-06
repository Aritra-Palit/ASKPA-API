using System;

namespace ASKPA.API.Model
{
    public class clsBusinessInfo
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string AliasName { get; set; } = "";
        public long NoofUsers { get; set; }
        public string DataBaseConnection { get; set; } = "";
        public string StartDate { get; set; } = "";
        public string EndDate { get; set; } = "";
        public string NotificationDate { get; set; } = "";
        public string RenewDate { get; set; } = "";
        public string LastRenewDate { get; set; } = "";
    }
    public class ClsAuditLogInfo
    {
        public string UserCode { get; set; } = "";
        public string UserName { get; set; } = "";
        public string UserEmail { get; set; } = "";
        public string AccessPage { get; set; } = "";
        public string AccessLog { get; set; } = "";
        public string Division { get; set; } = "";
        public string Incoming { get; set; } = "";
        public string Businessid { get; set; } = "";
    }
    public class ClsResultInfo
    {
        public long id { get; set; } = 0;
        public string message { get; set; } = "";
        public Boolean status { get; set; } = false;


    }
    //public class clsLoginResultInfo
    //{
    //    public String empno { get; set; } = "";
    //    public String empemail { get; set; } = "";
    //    public String designationname { get; set; } = "";
    //    public String postname { get; set; } = "";
    //    public String Division { get; set; } = "";
    //    public String designationshortform { get; set; } = "";
    //    public String Empname { get; set; } = "";
    //    public String HQ { get; set; } = "";
    //    public Boolean Truefalse { get; set; } = true;
    //    public long IDCycleGroup { get; set; } = 0;
    //}

}
