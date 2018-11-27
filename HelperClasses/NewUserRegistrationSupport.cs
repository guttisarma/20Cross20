using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperClasses
{
    public class NewUserRegistrationSupport
    {
        public string ID { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        [DisplayName("Full Address")]
        public string FullAddress
        {
            get
            {
                return Address1 + " " + Address2 + " " + Address3 + " .";
            }
        }
        public string AddressProofType { get; set; }
        public string AddressID { get; set; }
        public DateTime? Dob { get; set; }
        [DisplayName("Date of Birth")]
        public String DateofBirth
        {
            get
            {
                if (Dob == null)
                {
                    return "";
                }
                return ((DateTime)Dob).ToString("dd/MM/yyyy");
            }
        }


    }
}
