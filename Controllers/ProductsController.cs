﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BTVN5.Models;
using Microsoft.Extensions.Hosting;

namespace BTVN5.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductsController(ProductDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        // GET: Products
        public async Task<IActionResult> Index()
        {
            var productDbContext = _context.Products.Include(p => p.Categpry);
            return View(await productDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Categpry)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategpryId"] = new SelectList(_context.Categories, "Id", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Price,Description,Image,CategpryId")] Product product,IFormFile Image)
        {
            
            if (ModelState.IsValid)
            {
                if (Image != null && Image.Length > 0)
                {
                    // Lưu trữ tệp ảnh vào thư mục trên server hoặc bất kỳ nơi lưu trữ khác bạn chọn
                    product.Image = await SaveImage(Image);
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategpryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategpryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            ViewData["CategpryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategpryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Price,Description,Image,CategpryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategpryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategpryId);
            return View(product);
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            // Tùy chỉnh phương thức này để lưu trữ tệp ảnh vào nơi bạn muốn trên máy chủ
            // Ví dụ: thư mục 'wwwroot/images'
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            // Tạo tên file duy nhất
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

            // Đường dẫn đầy đủ đến file trên máy chủ
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Lưu trữ tệp ảnh vào đường dẫn đã chọn
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Trả về đường dẫn của file
            return "/images/" + uniqueFileName; // Đường dẫn trả về có thể sử dụng để hiển thị ảnh trong ứng dụng của bạn
        }
        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Categpry)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

     
    }
}
