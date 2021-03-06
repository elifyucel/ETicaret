﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Yms.Contracts.Production;
using Yms.Data.Entities;
using Yms.Services.OrderManagement.Abstractions;

namespace Yms.Services.OrderManagement.Concretes
{
    internal class CartService : ICartService
    {
        private readonly DbSet<Basket> table;
        private readonly DbContext context;

        public CartService(DbContext context)
        {
            table = context.Set<Basket>();
            this.context = context;
        }

        public bool Add(Guid userId, Guid productId, byte count)
        {
            table.Add(new Basket
            {
                IsActive = true,
                IsDeleted = false,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                CreatedBy = userId,
                UpdatedBy = userId,
                ProductId = productId,
                Amount = count,
                UserId = userId
            });
            return context.SaveChanges() > 0;
        }


        public IEnumerable<CartDto> GetProductByUserId(Guid id)
        {
            return  table.Include(i => i.Product).Where(p => p.UserId == id).Select(p => new CartDto
            {
                ProductId = p.ProductId,
                Amount = p.Amount,
                Price = p.Product.Price,
                ProductName = p.Product.Name,
                ImageId = p.Product.DocumentId,
                
            }).ToList();
        }

        public bool UpdateCart(Guid userId, Guid productId, byte amount)
        {
            var cartItem = table.FirstOrDefault(f => f.UserId == userId && f.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Amount = amount;
                cartItem.Updated = DateTime.Now;
                context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}

//TODO: Repository Pattern && Generic Repository Pattern && UnitOfWork Pattern