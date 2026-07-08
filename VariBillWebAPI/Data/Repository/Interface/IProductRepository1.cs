
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using VariBillWebAPI.Data.Entities;

//namespace VariBillWebAPI.Data.Repository.Interface;

//public interface IProductRepository : IRepository1<Product>
//{
//    Task<IEnumerable<Product>> GetAllProductsAsync();//(string excludeProfileBID);

//    //Task<Profile> GetProfileByBIDAsync(string profileBID);

//    //IQueryable<Profile> GetQueryableProfileByBID(string profileBID);

//    //Task<List<Profile>> GetAllNonMasterProfilesAsync(string excludeProfileBID);
//    //Task<List<Profile>> GetProfilesByBIDOrNameAsync(string bidOrName, string excludeProfileBID);
//    //Task<List<Profile>> GetProductsByCategoryNameAsync(string categoryName);
//    //Task<List<Profile>> GetServicesByCategoryNameAsync(string categoryName);

//    ////TODO(NB):should Task be used all the way to repo level or can repo return only value type needed?
//    //Task<bool> HasFoldersAsync(string profileGUID);

//    Task<IReadOnlyList<Product>> GetAllWithProductTypeAsync();
//    Task<Product?> GetByIdWithProductTypeAsync(Guid id);

//}

