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
    
    public partial class UserAuctionInfo
    {
        public long UserAuctionInfoPID { get; set; }
        public Nullable<long> UserDetailPID { get; set; }
        public Nullable<long> ManagerUserDetailPID { get; set; }
        public Nullable<long> ProductPID { get; set; }
        public Nullable<decimal> AuctionAmount { get; set; }
        public Nullable<System.DateTime> Timestamp { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<long> CreatedUserPID { get; set; }
        public Nullable<long> UpdatedUserPID { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual UserDetail UserDetail { get; set; }
        public virtual UserDetail UserDetail1 { get; set; }
    }
}
