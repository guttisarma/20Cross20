using PBVCC.DAL.AppData;
using PBVCC.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PBVCC.Models;
using System.Linq.Expressions;
using PBVCC.Business;
using HelperClasses;

namespace PBVCC.Models.Administrator
{
    public class NewUserRegistrationHelper
    {
        public void SaveNewUserDetails(NewUserRegistrationSupport NewUserDeatils)
        {
            GenericRepository<Phone> PhoneRepository;
            GenericRepository<Email> EmailRepository;
            GenericRepository<Address> AddressRepository;
            GenericRepository<UserDetailAddress> UserDetailAddressRepository;
            GenericRepository<UserDetail> UserDetailRepository;
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    try
                    {
                        #region Phone
                        PhoneRepository = unitOfWork.GetRepoInstance<Phone>();
                        Phone phone = new Phone();
                        phone.Number = NewUserDeatils.PhoneNumber;
                        PhoneRepository.Insert(phone);
                        #endregion

                        #region Email
                        EmailRepository = unitOfWork.GetRepoInstance<Email>();
                        Email email = new Email();
                        email.ID = NewUserDeatils.Email;
                        EmailRepository.Insert(email);
                        #endregion

                        #region Address
                        AddressRepository = unitOfWork.GetRepoInstance<Address>();
                        Address Add = new Address();
                        Add.Address1 = NewUserDeatils.Address1;
                        Add.Address2 = NewUserDeatils.Address2;
                        Add.Address3 = NewUserDeatils.Address3;
                        Add.Phone = phone;
                        Add.Email = email;
                        AddressRepository.Insert(Add);
                        #endregion

                        #region User Address Details
                        UserDetailAddressRepository = unitOfWork.GetRepoInstance<UserDetailAddress>();
                        UserDetailAddress UDA = new UserDetailAddress();
                        UDA.Address = Add;
                        UserDetailAddressRepository.Insert(UDA);
                        #endregion

                        #region User Details
                        UserDetailRepository = unitOfWork.GetRepoInstance<UserDetail>();
                        UserDetail newUser = new UserDetail();
                        newUser.FirstName = NewUserDeatils.FirstName;
                        newUser.LastName = NewUserDeatils.LastName;
                        newUser.MiddleName = NewUserDeatils.MiddleName;
                        newUser.DateofBirth = NewUserDeatils.Dob;
                        UDA.UserDetail = newUser;
                        //newUser.UserDetailAddress.=UDA;
                        UserDetailRepository.Insert(newUser);
                        #endregion

                        unitOfWork.SaveChanges();

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("System Stack :: " + ex.StackTrace + " System Exception Message :: " + ex.Message);
                    }
                }
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
            }
        
        }
        
        public void ApprovedUser(string Description, string Id)
        {
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    GenericRepository<UserDetail> uDetails;
                    GenericRepository<UserDetailAddress> uDetailsAddress;
                    long id = Convert.ToInt64(Id);
                    uDetailsAddress = unitOfWork.GetRepoInstance<UserDetailAddress>();
                    Expression<Func<UserDetailAddress, object>> parameter1 = v => v.UserDetail;
                    Expression<Func<UserDetailAddress, object>> parameter2 = v => v.Address;
                    Expression<Func<UserDetailAddress, object>> parameter3 = v => v.Address.Phone;
                    Expression<Func<UserDetailAddress, object>> parameter4 = v => v.Address.Email;
                    Expression<Func<UserDetailAddress, object>>[] parameterArray = new Expression<Func<UserDetailAddress, object>>[] { parameter1, parameter2, parameter3, parameter4 };
                    IEnumerable<UserDetailAddress> ListUser = uDetailsAddress.GetAllExpressions(x => (x.UserDetail.IsApproved == null && x.UserDetailPID == id), null, naProperties: parameterArray);
                    string EmailID = ListUser.FirstOrDefault<UserDetailAddress>().Address.Email.ID;

                    if (!isAlreadyExists(unitOfWork, EmailID))
                    {
                        //check for the emil is not already exists

                        uDetails = unitOfWork.GetRepoInstance<UserDetail>();

                        UserDetail ApprovedUser = uDetails.GetByID(Convert.ToInt64(Id));
                        ApprovedUser.IsActive = true;
                        ApprovedUser.IsApproved = true;
                        ApprovedUser.ApproveReason = Description;
                        ApprovedUser.ApprovedDate = DateTime.Now;

                        string strPassword = GenrateRandomPassword();
                        //User will be created here
                        GenericRepository<User> ApprvedUserRepository;
                        ApprvedUserRepository = unitOfWork.GetRepoInstance<User>();

                        User user = new User();
                        user.FirstName = ApprovedUser.FirstName;
                        user.Email = EmailID;
                        user.ActivationCode = Guid.NewGuid();
                        user.IsActive = true;
                        user.LastName = ApprovedUser.LastName;
                        user.Password = Security.Encrypt(strPassword);
                        GenericRepository<Role> rolesrepo = unitOfWork.GetRepoInstance<Role>();
                        //user.Roles = rolesrepo.GetAllExpressions(x => x.RoleId == 2, null, null, null).ToList<Role>();//Customer Role i being assigned
                        string CrypticInfo = new string((Convert.ToInt64(DateTime.Now.ToString("yyyy")) + ApprovedUser.UserdetailPID).ToString().Substring(0, 3).Reverse().ToArray());
                        user.Username = ApprovedUser.FirstName.Substring(0, 4).ToUpper() + CrypticInfo;
                        CrypticInfo = string.Empty;
                        CrypticInfo = new string((Convert.ToInt64(((DateTime)ApprovedUser.DateofBirth).ToString("yyyy")) + ApprovedUser.UserdetailPID).ToString().Substring(0, 3).Reverse().ToArray());
                        user.UserCode = ApprovedUser.FirstName.Substring(0, 4).ToUpper() + CrypticInfo;
                        ApprovedUser.User = user;
                        ApprvedUserRepository.Insert(user);
                        

                        unitOfWork.SaveChanges();
                        //Password will be generated here
                        //SendEmail(ToAddress,UserName,Password);
                        //Mail will be send from here

                    }
                }
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
            }
        }

        private bool isAlreadyExists(UnitOfWork unitOfWork,string EmailID)
        {
            try
            {
                TradeBulkEntities db = new TradeBulkEntities();
                var varible = from x in db.Users select x;
                var v2 = from x in db.UserDetails select x;


                GenericRepository<User> ApprvedUserRepository;
                ApprvedUserRepository = unitOfWork.GetRepoInstance<User>();


                if (ApprvedUserRepository.GetAllExpressions(x => x.Email == EmailID).Count<User>() == 0)
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
            }
                return true;
        }

        private string GenrateRandomPassword()
        {
            string Password = string.Empty;
            try
            {
                Random random = new Random();
                const string chars = ")(*&^%$#@!ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                Password = new string(Enumerable.Repeat(chars, 9).Select(s => s[random.Next(s.Length)]).ToArray());
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
            }
            return Password;
        }
    }
}