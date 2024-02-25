﻿using IMS.CoreBusiness;
using IMS.Plugins.EFCoreSqlServer;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.InMemory
{
    public class ProductEFCoreRepository : IProductRepository
    {
        private readonly IMSContext db;

        public ProductEFCoreRepository(IMSContext db)
        {
            this.db = db;
        }

        public async Task AddProductAsync(Product product)
        {
            this.db.Products.Add(product);

            FlagInventoryUnchanged(product, this.db);

            await this.db.SaveChangesAsync();
        }

        /*
        public async Task<bool> ExistsAsync(Product product)
        {
            return await Task.FromResult(
                _products.Any(x => x.ProductId != product.ProductId &&
                    x.ProductName.Equals(product.ProductName, StringComparison.OrdinalIgnoreCase)));
        }
        */

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await this.db.Products.Include(x => x.ProductInventories)
                .ThenInclude(x => x.Inventory)
                .FirstOrDefaultAsync( x => x.ProductId == productId);
        }
        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            return await this.db.Products.Where(x => x.ProductName.ToLower().IndexOf(name.ToLower()) >= 0).ToListAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            // To prevent different product from having the same name
            var prod = await this.db.Products
                .Include(x => x.ProductInventories)
                .FirstOrDefaultAsync(x => x.ProductId == product.ProductId);

            if (prod != null)
            {
                prod.ProductName = product.ProductName;
                prod.Price = product.Price;
                prod.Quantity = product.Quantity;
                prod.ProductInventories = product.ProductInventories;

                FlagInventoryUnchanged(product, this.db);

                await this.db.SaveChangesAsync();
            }
        }

        private void FlagInventoryUnchanged(Product product, IMSContext db)
        {
            if (product?.ProductInventories != null &&
                product.ProductInventories.Count > 0)
            {
                foreach (var prodInv in product.ProductInventories)
                {
                    if (prodInv.Inventory != null)
                        db.Entry(prodInv.Inventory).State = EntityState.Unchanged;
                }
            }
        }
    }
}
