using ASKPA.API.Common;
using ASKPA_API.Model;
using System.Threading.Tasks;

namespace ASKPA_API.BLL
{
    public class clsEnquiry
        {
        public static async Task<string> NewEnquiry(string CompanyConnection, clsEnquiryDetails info)
        {
            return await clsHelperDBAsync.fnDBOperationAsync(CompanyConnection, "PRC_Enquiry",
               info.CustomerName, info.CustomerEmail, info.CustomerMobile, info.EnquirySubject);
        }
    }
}
