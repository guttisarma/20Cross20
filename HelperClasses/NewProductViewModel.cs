using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperClasses
{
    public enum EProductType {Inventatory=1,Stock,Marketing,Others};
    public enum EDocumentType { License=1, Guarantee, Contract, Diploma, Surety, Warrant, Service, Others, ProfilePhoto };

    public class DocumentViewModel
    {
        public string DocumentPath { get; set; }
        public string MIMEType { get; set; }

    }
    public class NewProductViewModel
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string ProductType { get; set; }
        public string AuctionDate { get; set; }
        public string Description { get; set; }
        public StringBuilder ImageData { get; set; }
        public List<DocumentViewModel> Documents { get; set; }

    }
}
