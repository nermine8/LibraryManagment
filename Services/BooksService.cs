﻿using LibraryManagment.Data;
using LibraryManagment.DTOs.BooksDTOs.Requests;
using LibraryManagment.DTOs.BooksDTOs.Responses;
using LibraryManagment.Interface;
using LibraryManagment.Models;
using Microsoft.EntityFrameworkCore;
namespace LibraryManagment.Services
{
    public class BooksService : IBooksService
    {
        #region Variables+Constroctor
        private readonly ApplicationDBcontext _dbcontext;
        public BooksService(ApplicationDBcontext dbcontext) => _dbcontext = dbcontext;
        #endregion
        #region Get
        #region All
        public async Task<IEnumerable<GetAllBooksResponses>> GetAllAsync()
        {
            List<GetAllBooksResponses> books = await _dbcontext.Books.Include(b => b.BookCategory)
                                                                     .Select(b => new GetAllBooksResponses(b.Title,
                                                                                                           b.Author,
                                                                                                           b.Stock,
                                                                                                           b.BookCategory.CategoryName)).ToListAsync();
            return books;
        }
        #endregion
        #region ById
        public async Task<GetBookByIdResponse> GetByIdAsync(int id)
        {
            Books? book = await _dbcontext.Books.Include(bookSelected => bookSelected.BookCategory)
                                                .FirstOrDefaultAsync(bookSelected => bookSelected.ID == id);
            return book is null
                ? throw new KeyNotFoundException("Not Found")
                : new GetBookByIdResponse(book.Title,
                                          book.Author,
                                          book.Stock,
                                          book.BookCategory.CategoryName);
        }
        #endregion
        #region ByTitle
        public async Task<GetBookByTitleResponse> GetByTitleAsync(string title)
        {
            Books? book = await _dbcontext.Books.Include(bookSelected => bookSelected.BookCategory)
                                                .FirstOrDefaultAsync(bookSelected => bookSelected.Title.ToUpper() == title.ToUpper());
            return book is null
                ? throw new KeyNotFoundException("Not Found")
                : new GetBookByTitleResponse(book.Author,
                                             book.Stock,
                                             book.BookCategory.CategoryName);
        }
        #endregion
        #region ByCategory
        public async Task<IEnumerable<GetBookByCategoryResponse>> GetByCategoryAsync(string categoryName)
        {
            Category category = await _dbcontext.Categories.FirstOrDefaultAsync(categorySelected => categorySelected.CategoryName.ToUpper() == categoryName.ToUpper())
                                                            ?? throw new KeyNotFoundException("Not Found");
            List<Books> books = await _dbcontext.Books.Where(bookSelected => bookSelected.BookCategory.CategoryID == category.CategoryID).ToListAsync();
            return books.Select(b => new GetBookByCategoryResponse(b.Title,
                                                                   b.Author,
                                                                   b.Stock)).ToList();
        }
        #endregion
        #region ByAuther
        public async Task<IEnumerable<GetBookByAuthorResponse>> GetByAuthorAsync(string auther)
        {
            List<Books> book = await _dbcontext.Books.Include(bookSelected => bookSelected.BookCategory)
                                                     .Where(bookSelected => bookSelected.Author.ToUpper() == auther.ToUpper()).ToListAsync();
            if (book.Count == 0)
            {
                throw new KeyNotFoundException("Not Found");
            }
            return book.Select(b => new GetBookByAuthorResponse(b.Title, 
                                                                b.Stock, 
                                                                b.BookCategory.CategoryName)).ToList();

        }
        #endregion
        #endregion
        #region Update
        public async Task<string> UpdateAsync(UpdateBookRequest bookRequest)
        {
            Books book = _dbcontext.Books.Find(bookRequest.Id) ?? throw new KeyNotFoundException("Not Found");
            book.Stock = bookRequest.Stock;
            await _dbcontext.SaveChangesAsync();
            return ("Update Successfully");
        }
        #endregion
        #region Add
        public async Task<string> AddAsync(AddBookRequest bookRequest)
        {
            Category category = await _dbcontext.Categories.FirstOrDefaultAsync(categorySelected => categorySelected.CategoryName.ToUpper() == bookRequest.BookCategory.CategoryName.ToUpper())
                                                           ?? throw new KeyNotFoundException("Category not found");
            _dbcontext.Books.Add(new Books( bookRequest.Title,
                                            bookRequest.Author,
                                            bookRequest.Stock,
                                            category)
            );
            await _dbcontext.SaveChangesAsync();
            return "Book Added Successfully!";
        }
        #endregion
        #region Delete
        public async Task<string> DeleteAsync(string bookName)
        {
            Books book = await _dbcontext.Books.FirstOrDefaultAsync(BookSelected => BookSelected.Title.ToUpper() == bookName.ToUpper()) 
                                               ?? throw new KeyNotFoundException("Not Found");
            _dbcontext.Books.Remove(book);
            await _dbcontext.SaveChangesAsync();
            return ("Deleted Saccussefully");
        }
        #endregion
    }

}
