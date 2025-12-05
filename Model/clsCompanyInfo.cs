using System;

namespace ASKPA_API.Model
{
    public class clsCompanyInfo
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string AliasName { get; set; } = "";
        public string Address1 { get; set; } = "";
        public string Address2 { get; set; } = "";
        public string State { get; set; } = "";
        public string Country { get; set; } = "";
        public string Pincode { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Mobile { get; set; } = "";
        public string Email { get; set; } = "";
        public string Contactno { get; set; } = "";
        public string ContactPerson { get; set; } = "";
        public string Website { get; set; } = "";
        public string PANNO { get; set; } = "";
        public string CINNO { get; set; } = "";
        public string GSTNO { get; set; } = "";
        public string EntryUser { get; set; } = "";
        public int Active { get; set; } = 0;
        public int Demo { get; set; } = 0;
    }
    public class clsCompanyList
    {
        public long IDCompany { get; set; } = 0;
        public string MerchantCode { get; set; } = "";
    }
    public class clsConfigList
    {
        public int Twostep { get; set; } = 0;
        public long NoofUsers { get; set; } = 0;
        public long OtpTimer { get; set; } = 0;
        public int S_Email { get; set; } = 0;
        public int S_Phone { get; set; } = 0;
        public int S_Whatsapp { get; set; } = 0;
        public string VersionName { get; set; } = "";
        public string FolderName { get; set; } = "";
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateOnly NotificationDate { get; set; }
        public DateOnly RenewDate { get; set; }
        public DateOnly LastRenewDate { get; set; }
        public string DashboardURL { get; set; } = "";
        public string APIURL { get; set; } = "";
    }
}
