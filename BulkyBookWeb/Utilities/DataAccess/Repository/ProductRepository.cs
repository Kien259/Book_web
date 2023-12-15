﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db): base(db)
        {
            _db = db;

        }
        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

		public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
		{
			return await _db.Products
				.Where(p => p.Title.Contains(searchTerm) || p.Author.Contains(searchTerm))
				.ToListAsync();
		}

		public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(x => x.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Price = obj.Price;
                objFromDb.Price100 = obj.Price100;
                objFromDb.Description = obj.Description;    
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Author = obj.Author;
                objFromDb.CoverTypeId = obj.CoverTypeId;
                if(obj.ImageUrl != null) objFromDb.ImageUrl = obj.ImageUrl;
            }
        }
    }
}
