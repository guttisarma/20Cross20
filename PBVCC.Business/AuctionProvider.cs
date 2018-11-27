using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBVCC.DAL;
using PBVCC.DAL.AppData;
using PBVCC.Models.Administrator;
using System.Web;
using HelperClasses;

namespace PBVCC.Business
{
    public class AuctionProvider
    {
        public DateTime LastUpdated;
        public List<CurrentAuction> GetIntialAuction(int CurrentUserPID)
        {
            // int CurrentUserPID = ((PBVCC.Authe_AuthoATION.CustomPrincipal)HttpContext.User).UserId;
            List<CurrentAuction> lscAuction = new List<CurrentAuction>();
            GenericRepository<UserAuctionInfo> UserAuctionInfoRepository;
            GenericRepository<UserDetail> UserDetailRepository;
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                UserAuctionInfoRepository = unitOfWork.GetRepoInstance<UserAuctionInfo>();
                UserDetailRepository = unitOfWork.GetRepoInstance<UserDetail>();
                UserDetail cUser= UserDetailRepository.GetAllExpressions(x => x.UserPID == CurrentUserPID).FirstOrDefault<UserDetail>();
                IEnumerable<UserAuctionInfo> currentUserAuction = UserAuctionInfoRepository.GetAllExpressions(x => x.ManagerUserDetailPID == cUser.ManagerPID, null, null, null);
                //CurrentUserPID = 11;
                //currentUserAuction = UserAuctionInfoRepository.GetAllExpressions(x => x.UserDetail.UserPID == CurrentUserPID, null, null, null);
                foreach (UserAuctionInfo usrAuction in currentUserAuction)
                {
                    lscAuction.Add(new CurrentAuction() { UserName = usrAuction.UserDetail.FirstName, UserAmount = usrAuction.AuctionAmount.ToString(), Timestamp = usrAuction.Timestamp.ToString() });
                }
            }
            //var something=from x in lscAuction  select max
            LastUpdated =Convert.ToDateTime( lscAuction.Max(x => x.Timestamp));
            return lscAuction;
        }
        public void submitAuctionAmont(CurrentAuction prvidedDetails)
        {
            GenericRepository<UserAuctionInfo> UserAuctionInfoRepository;

            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                UserAuctionInfoRepository = unitOfWork.GetRepoInstance<UserAuctionInfo>();
                UserAuctionInfo saveUserAuctionInfo = new UserAuctionInfo();
                saveUserAuctionInfo.AuctionAmount = Convert.ToDecimal(prvidedDetails.UserAmount);
                saveUserAuctionInfo.Timestamp = DateTime.Now;
                saveUserAuctionInfo.Product = new Product(); //Here we need current product
                saveUserAuctionInfo.UserDetail = new UserDetail();//current UserID
                saveUserAuctionInfo.UserDetail1 = new UserDetail();//current User Manger 
                UserAuctionInfoRepository.Insert(saveUserAuctionInfo);
                unitOfWork.SaveChanges();
            }

        }
        public bool isAnyUpdate(int CurrentUserPID)
        {
            GenericRepository<UserAuctionInfo> UserAuctionInfoRepository;
            GenericRepository<UserDetail> UserDetailRepository;
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                UserAuctionInfoRepository = unitOfWork.GetRepoInstance<UserAuctionInfo>();
                UserDetailRepository = unitOfWork.GetRepoInstance<UserDetail>();
                UserDetail cUser = UserDetailRepository.GetAllExpressions(x => x.UserPID == CurrentUserPID).FirstOrDefault<UserDetail>();
                IEnumerable<UserAuctionInfo> currentUserAuction = UserAuctionInfoRepository.GetAllExpressions(x => x.ManagerUserDetailPID == cUser.ManagerPID && x.Timestamp > LastUpdated, null, null, null);
                if (currentUserAuction.Count<UserAuctionInfo>() > 0)
                    return true;
            }

            return false;
        }
    }
}
