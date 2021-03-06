﻿using System;
using System.Linq;
using System.Threading.Tasks;
using JjOnlineStore.Common.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JjOnlineStore.Data.EF;
using JjOnlineStore.Data.Entities;
using JjOnlineStore.Services.Core.Admin;

namespace JjOnlineStore.Web.Areas.Admin.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// 
    /// </summary>
    public class AdminProductsController : BaseAdminController
    {
        private readonly JjOnlineStoreDbContext _context;
        private readonly IAdminProductsService _adminProductsService;
        private readonly IAdminCategoryService _adminCategoryService;

        public AdminProductsController(
            JjOnlineStoreDbContext context, 
            IAdminProductsService adminProductsService,
            IAdminCategoryService adminCategoryService)
        {
            _context = context;
            _adminProductsService = adminProductsService;
            _adminCategoryService = adminCategoryService;
        }

        /// GET: Admin/AdminProducts
        /// <summary>
        /// Returns all products without deleted.
        /// </summary>
        public async Task<IActionResult> Index()
            => View(await _adminProductsService.AllWithoutDeletedAsync());

        // GET: Admin/AdminProducts/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/AdminProducts/Create
        public async Task<IActionResult> Create()
        {
            ViewData["Categories"] = new SelectList(await _adminCategoryService.AllAsync(), "Id", "Name");
            return View();
        }

        // POST: Admin/AdminProducts/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,Base64Image,IsAvailable,Size,Color,Type,Details,CategoryId,FormImages")] ProductViewModel product)
        {
            await _adminProductsService.CreateAsync(product);
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/AdminProducts/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Admin/AdminProducts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Description,Price,Base64Image,IsAvailable,Size,Color,Type,Details,CategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }
            product.ModifiedOn = DateTime.UtcNow;
            _context.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
