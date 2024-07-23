﻿using LibraryManagment.Data;
using LibraryManagment.DTOs.CategoriesDTOs;
using LibraryManagment.Interface;
using LibraryManagment.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagment.Services
{
    public class CategoriesService : ICategoriesService
    {
        #region Variable + Constroctor
        private readonly ApplicationDBcontext _dbcontext;
        public CategoriesService(ApplicationDBcontext dbcontext) => _dbcontext = dbcontext;
        #endregion

        #region Get
        #region All
        public async Task<IEnumerable<GetAllCategoriesResponse>> GetAllAsync(int pageNumber = 1,int pageSize=10)
        {
            IEnumerable<GetAllCategoriesResponse> categories = await _dbcontext.Categories.Include(book => book.Books)
                                                                                           .Skip((pageNumber - 1) * pageSize)
                                                                                           .Take(pageSize)
                                                                                           .Select(categorySelected => new GetAllCategoriesResponse(categorySelected.CategoryName,
                                                                                                                                                    categorySelected.Books.Select(bookSelected => bookSelected.Title).ToList())).ToListAsync();

            return categories;
        }
        #endregion

        #region ByName
        public async Task<GetAllCategoriesResponse> GetByNameeAsync(string categoryName)
        {
            Category? category = await _dbcontext.Categories.Include(book => book.Books)
                                                            .FirstOrDefaultAsync(categorySelected => categorySelected.CategoryName.ToUpper() == categoryName.ToUpper()) ?? throw new KeyNotFoundException("Not Found");
            return new GetAllCategoriesResponse(category.CategoryName,
                                                category.Books.Select(bookSelected => bookSelected.Title).ToList());

        }
        #endregion
        #endregion

        #region Add
        public async Task<bool> AddAsync(AddCategoryRequest category)
        {
            _dbcontext.Categories.Add(new Category(category.CategoryName));
            await _dbcontext.SaveChangesAsync();
            return true;
        }
        #endregion

        #region Delete
        public async Task<bool> DeleteAsync(string categoryName)
        {
            var category = await _dbcontext.Categories.FirstOrDefaultAsync(categorySelected => categorySelected.CategoryName.ToUpper() == categoryName.ToUpper())
                                                      ?? throw new KeyNotFoundException("Not Found");
            _dbcontext.Categories.Remove(category);
            await _dbcontext.SaveChangesAsync();
            return (true);
        }
        #endregion
    }
}
