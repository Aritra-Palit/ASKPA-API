using System;
namespace ASKPA_API.Model
{
    public class clsMiscInfo
    {
        public long IDMisc { get; set; } = 0;
        public String Code { get; set; } = "";
        public String Name { get; set; } = "";
    }
    public class RequestType
    {
        public String Type { get; set; } = "";
    }
    public class MiscInfo
    {
        public long IDMisc { get; set; } = 0;
        public String Code { get; set; } = "";
        public String Name { get; set; } = "";
        public long Value { get; set; } = 0;
        public String Type { get; set; } = "";
        public long DisplayOrder { get; set; } = 0;
        public String SystemYN { get; set; } = "";
        public String ActiveYN { get; set; } = "";
    }
    public class MiscInfoList
    {
        public long IDMisc { get; set; } = 0;
        public String Code { get; set; } = "";
        public String Name { get; set; } = "";
        public String Type { get; set; } = "";
    }
}
