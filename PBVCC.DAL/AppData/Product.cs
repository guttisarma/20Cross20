//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PBVCC.DAL.AppData
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.UserAuctionInfoes = new HashSet<UserAuctionInfo>();
        }
    
        public long ProductPID { get; set; }
        public Nullable<long> ProductTypePID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Nullable<long> RatingPID { get; set; }
        public int Quanity { get; set; }
        public long DocumentPID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public Nullable<System.DateTime> AuctionDate { get; set; }
        public Nullable<long> OwnerPID { get; set; }
        public Nullable<decimal> EachPiecePrice { get; set; }
        public Nullable<bool> ispublic { get; set; }
        public Nullable<int> assignedOwner { get; set; }
        public Nullable<long> NotePid { get; set; }
        public Nullable<long> ProductState { get; set; }
        public Nullable<long> salesTypePid { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<long> CreatedUserPID { get; set; }
        public Nullable<long> UpdatedUserPID { get; set; }
    
        public virtual Document Document { get; set; }
        public virtual Note Note { get; set; }
        public virtual UserDetail UserDetail { get; set; }
        public virtual UserDetail UserDetail1 { get; set; }
        public virtual ProductType ProductType { get; set; }
        public virtual Rating Rating { get; set; }
        public virtual saleType saleType { get; set; }
        public virtual UserDetail UserDetail2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserAuctionInfo> UserAuctionInfoes { get; set; }
    }
}