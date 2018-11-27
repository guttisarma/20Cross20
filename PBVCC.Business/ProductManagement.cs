using PBVCC.DAL;
using PBVCC.DAL.AppData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperClasses;

namespace PBVCC.Business
{
    //8989
    public class ProductManagement
    {
        public void CreateProduct(NewProductViewModel product, int CurrentUserID)
        {
            GenericRepository<Product> ProductRepository;
            GenericRepository<DAL.AppData.ProductType> ProductTypeRepository;
            GenericRepository<Document> DocumentRepository;
            GenericRepository<DocumentType> DocumentTypeRepository;
            GenericRepository<User> UserRepository;

            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    DocumentRepository = unitOfWork.GetRepoInstance<Document>();
                    ProductRepository = unitOfWork.GetRepoInstance<Product>();
                    ProductTypeRepository = unitOfWork.GetRepoInstance<DAL.AppData.ProductType>();
                    DocumentTypeRepository = unitOfWork.GetRepoInstance<DocumentType>();
                    UserRepository = unitOfWork.GetRepoInstance<User>();
                    Product prdct = new Product();

                    prdct.ProductName = product.ProductName;
                    prdct.Code = "";//need to generate
                    prdct.Description = product.Description;
                    prdct.Price = Convert.ToDecimal(product.Price);
                    //owner
                    //prdct.Owner = UserRepository.GetByID(CurrentUserID);
                    //803 taking input from User not hard corded one
                    prdct.ProductType = ProductTypeRepository.GetByID((long)EProductType.Inventatory);
                    prdct.Quanity = Convert.ToInt32(product.Quantity);
                    ProductRepository.Insert(prdct);
                    foreach (DocumentViewModel Dviewmodel in product.Documents)
                    {
                        Document dcment = new Document();
                        dcment.ServerPath = Dviewmodel.DocumentPath;
                        dcment.FileExtension = Dviewmodel.MIMEType;
                        dcment.DocumentType = DocumentTypeRepository.GetByID((long)EDocumentType.ProfilePhoto);
                        dcment.Products.Add(prdct);
                        DocumentRepository.Insert(dcment);
                    }
                    unitOfWork.SaveChanges();

                }


            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
            }
        }

        public List<NewProductViewModel> GetCurrentUserProductList(int CurrentUserID,int CurrentPage,int PageSize,out long TotalPages)
        {
            GenericRepository<Product> ProductRepository;
            GenericRepository<Document> DocumentRepository;
            GenericRepository<UserDetail> UserDetailsRepository;
            TotalPages = 0;

            List<NewProductViewModel> productResult = new List<NewProductViewModel>();
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    DocumentRepository = unitOfWork.GetRepoInstance<Document>();
                    ProductRepository = unitOfWork.GetRepoInstance<Product>();
                    UserDetailsRepository = unitOfWork.GetRepoInstance<UserDetail>();



                    IQueryable<Product> lsProd = ProductRepository.GetAllExpressions(/*x => x.User.UserId == CurrentUserID*/);
                    TotalPages = lsProd.Count<Product>();
                    foreach (Product p in lsProd)
                    {
                        NewProductViewModel productVeiwModel = new NewProductViewModel();
                        productVeiwModel.Id = p.ProductPID.ToString();
                        productVeiwModel.Description = p.Description;
                        productVeiwModel.AuctionDate = p.AuctionDate == null ? "" : p.AuctionDate.Value.ToString("dddd, dd MMMM yyyy HH:mm:ss");
                        //productVeiwModel.Documents.Add(p.Document);
                        byte[] byteData = System.IO.File.ReadAllBytes(p.Document.ServerPath);
                        //Convert byte arry to base64string   
                        string imreBase64Data = Convert.ToBase64String(byteData);
                        productVeiwModel.ImageData = new StringBuilder("data:image/png;base64," + imreBase64Data);
                        productVeiwModel.Price = p.Price.ToString();
                        productVeiwModel.ProductName = p.ProductName;
                        productVeiwModel.Quantity = p.Quanity.ToString();
                        productResult.Add(productVeiwModel);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
            }
            return productResult;
        }

        public NewProductViewModel GetProductbyId(int id)
        {
            GenericRepository<Product> ProductRepository;
            GenericRepository<Document> DocumentRepository;
            NewProductViewModel productVeiwModel = new NewProductViewModel();

            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    DocumentRepository = unitOfWork.GetRepoInstance<Document>();
                    ProductRepository = unitOfWork.GetRepoInstance<Product>();
                    Product lsProd = ProductRepository.GetByID(id);
                    productVeiwModel.Id = lsProd.ProductPID.ToString();
                    productVeiwModel.Description = lsProd.Description;
                    //productVeiwModel.Documents.Add(p.Document);
                    productVeiwModel.AuctionDate = lsProd.AuctionDate == null ? "" : lsProd.AuctionDate.Value.ToString("dddd dd MMMM yyyy HH:mm:ss");
                    byte[] byteData = System.IO.File.ReadAllBytes(lsProd.Document.ServerPath);
                    //Convert byte arry to base64string   
                    string imreBase64Data = Convert.ToBase64String(byteData);
                    productVeiwModel.ImageData = new StringBuilder("data:image/png;base64," + imreBase64Data);
                    productVeiwModel.Price = lsProd.Price.ToString();
                    productVeiwModel.ProductName = lsProd.ProductName;
                    productVeiwModel.Quantity = lsProd.Quanity.ToString();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
            }
            return productVeiwModel;

        }

        public List<NewProductViewModel> GetOnlineProducts(int CurrentPage, int PageSize, out long TotalPages)
        {
            TotalPages = 0;
            GenericRepository<Product> ProductRepository;
            List<NewProductViewModel> productResult;
            productResult = new List<NewProductViewModel>();
            try
            {
                using (UnitOfWork unitOfWork = new UnitOfWork())
                {
                    ProductRepository = unitOfWork.GetRepoInstance<Product>();
                    IQueryable<Product> lsProd = ProductRepository.GetAllExpressions(x=>x.ispublic==true);
                    TotalPages = lsProd.Count<Product>();
                    
                    foreach (Product p in lsProd)
                    {
                        NewProductViewModel productVeiwModel = new NewProductViewModel();
                        productVeiwModel.Id = p.ProductPID.ToString();
                        productVeiwModel.Description = p.Description;
                        productVeiwModel.AuctionDate = p.AuctionDate == null ? "" : p.AuctionDate.Value.ToString("dddd, dd MMMM yyyy HH:mm:ss");
                        //productVeiwModel.Documents.Add(p.Document);
                        byte[] byteData = System.IO.File.ReadAllBytes(p.Document.ServerPath);
                        //Convert byte arry to base64string   
                        string imreBase64Data = Convert.ToBase64String(byteData);
                        productVeiwModel.ImageData = new StringBuilder("data:image/png;base64," + imreBase64Data);
                        productVeiwModel.Price = p.Price.ToString();
                        productVeiwModel.ProductName = p.ProductName;
                        productVeiwModel.ProductCode = p.Code;
                        productVeiwModel.Quantity = p.Quanity.ToString();
                        productResult.Add(productVeiwModel);

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
            }
            return productResult;

        }
    }
}
