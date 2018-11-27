using HelperClasses;
using PBVCC.DAL;
using PBVCC.DAL.AppData;
using PBVCC.Models.Administrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PBVCC.Business
{
   public class ManageAdminHelper
    {
        public List<NewUserRegistrationSupport> GetNewUser()
        {
            List<NewUserRegistrationSupport> ListNewUsers = new List<NewUserRegistrationSupport>();

            GenericRepository<UserDetailAddress> UserDetailAddressRepository;
            try
            {
               
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {

                    UserDetailAddressRepository = unitOfWork.GetRepoInstance<UserDetailAddress>();

                    Expression<Func<UserDetailAddress, object>> parameter1 = v => v.UserDetail;
                    Expression<Func<UserDetailAddress, object>> parameter2 = v => v.Address;
                    Expression<Func<UserDetailAddress, object>> parameter3 = v => v.Address.Phone;
                    Expression<Func<UserDetailAddress, object>> parameter4 = v => v.Address.Email;
                    Expression<Func<UserDetailAddress, object>>[] parameterArray = new Expression<Func<UserDetailAddress, object>>[] { parameter1, parameter2, parameter3, parameter4 };
                    IEnumerable<UserDetailAddress> ListUser = UserDetailAddressRepository.GetAllExpressions(x => x.UserDetail.IsApproved == null, null, naProperties: parameterArray);
                    foreach (var User in ListUser)
                    {
                        NewUserRegistrationSupport NUser = new NewUserRegistrationSupport();
                        NUser.Address1 = User.Address.Address1;
                        NUser.Address2 = User.Address.Address2;
                        NUser.Address3 = User.Address.Address3;
                        NUser.Email = User.Address.Email.ID;
                        NUser.ID = User.UserDetailPID.ToString();
                        NUser.PhoneNumber = User.Address.Phone.Number;
                        NUser.FirstName = User.UserDetail.FirstName;
                        NUser.MiddleName = User.UserDetail.MiddleName;
                        NUser.LastName = User.UserDetail.LastName;
                        NUser.Dob = User.UserDetail.DateofBirth;//User.UserDetail.DateofBirth.Value.ToString("dd/MM/yyyy");
                        ListNewUsers.Add(NUser);

                    }
                }
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
            }

            return ListNewUsers;
        }

        public List<NewUserRegistrationSupport> GetAllApprovedUsers()
        {
            List<NewUserRegistrationSupport> ListNewUsers = new List<NewUserRegistrationSupport>();

            GenericRepository<UserDetailAddress> UserDetailAddressRepository;
            try
            {

                using (UnitOfWork unitOfWork = new UnitOfWork())
                {

                    UserDetailAddressRepository = unitOfWork.GetRepoInstance<UserDetailAddress>();

                    Expression<Func<UserDetailAddress, object>> parameter1 = v => v.UserDetail;
                    Expression<Func<UserDetailAddress, object>> parameter2 = v => v.Address;
                    Expression<Func<UserDetailAddress, object>> parameter3 = v => v.Address.Phone;
                    Expression<Func<UserDetailAddress, object>> parameter4 = v => v.Address.Email;
                    Expression<Func<UserDetailAddress, object>>[] parameterArray = new Expression<Func<UserDetailAddress, object>>[] { parameter1, parameter2, parameter3, parameter4 };
                    IEnumerable<UserDetailAddress> ListUser = UserDetailAddressRepository.GetAllExpressions(x => x.UserDetail.IsApproved == true, null, naProperties: parameterArray);
                    foreach (var User in ListUser)
                    {
                        NewUserRegistrationSupport NUser = new NewUserRegistrationSupport();
                        NUser.Address1 = User.Address.Address1;
                        NUser.Address2 = User.Address.Address2;
                        NUser.Address3 = User.Address.Address3;
                        NUser.Email = User.Address.Email.ID;
                        NUser.ID = User.UserDetailPID.ToString();
                        NUser.PhoneNumber = User.Address.Phone.Number;
                        NUser.FirstName = User.UserDetail.FirstName;
                        NUser.MiddleName = User.UserDetail.MiddleName;
                        NUser.LastName = User.UserDetail.LastName;
                        NUser.Dob = User.UserDetail.DateofBirth;//User.UserDetail.DateofBirth.Value.ToString("dd/MM/yyyy");
                        ListNewUsers.Add(NUser);

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
            }

            return ListNewUsers;
        }

        public long getCurrentUserPID(string Code)
        {
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    GenericRepository<UserDetail> UserDetail;
                    UserDetail = unitOfWork.GetRepoInstance<UserDetail>();
                    UserDetail user = UserDetail.GetAllExpressions(x => x.UserCode == Code, null, null).FirstOrDefault<UserDetail>();
                    return user.UserdetailPID;
                }
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
            }
            return 0;
        }
    }
}
