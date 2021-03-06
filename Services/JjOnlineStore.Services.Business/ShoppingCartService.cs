﻿using JjOnlineStore.Common.ViewModels.ShoppingCarts;
using JjOnlineStore.Data.EF;
using JjOnlineStore.Data.Entities;
using JjOnlineStore.Services.Business._Base;
using JjOnlineStore.Services.Core;
using JjOnlineStore.Services.Data.ShoppingCarts;
using JjOnlineStore.Common.ViewModels.CartItems;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace JjOnlineStore.Services.Business
{
    public class ShoppingCartService : BaseService, IShoppingCartService
    {
        public ShoppingCartService(JjOnlineStoreDbContext dbContext, IMapper mapper) 
            : base(dbContext)
        {
            Mapper = mapper;
        }

        protected IMapper Mapper { get; }

        public async Task<CartVm> GetByUserIdAsync(string userId)
        {
            var currentCartId = await GetCurrentCartIdByUserIdAsync(userId);
            return await DbContext
                .Carts
                .Include(c => c.OrderedItems)
                .ThenInclude(c => c.Product)
                .Where(c => c.Id == currentCartId)
                .ProjectTo<CartVm>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task CreateForUserByItsId(string userId)
        {
            var entity = new Cart
            {
                UserId = userId
            };

            await DbContext.Carts.AddAsync(entity);
            await DbContext.SaveChangesAsync();

            var currentUser = await DbContext
                .Users
                .FindAsync(userId);

            currentUser.CartId = entity.Id;

            DbContext.Users.Update(currentUser);
            await DbContext.SaveChangesAsync();
        }

        public async Task<ShoppingCartComponentServiceModel> GetCartComponentModelAsync(string userId)
        {
            var cartId = await GetCurrentCartIdByUserIdAsync(userId);
            var cartItems = await DbContext
                .CartItems
                .Where(ci => ci.CartId == cartId)
                .Include(ci => ci.Product)
                .ToListAsync();

            return new ShoppingCartComponentServiceModel
            {
                CartItems = Mapper.Map<IEnumerable<CartItem>,IEnumerable<CartItemVm>>(cartItems)
            };
        }
    }
}
