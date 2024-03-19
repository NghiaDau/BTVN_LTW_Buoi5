using System;
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
            ViewBag.categories = _context.Categories.ToList();
            var productDbContext = _context.Products.Include(p => p.Categpry);
            return View(await productDbContext.ToListAsync());
        }

        public async Task<IActionResult> ProductCategory(int Id)
        {
            ViewBag.categories = _context.Categories.ToList();
            var produtcs = _context.Products.ToList();
            Dictionary<string, int> categoryCounts = new Dictionary<string, int>();
            foreach (var category in ViewBag.Categories)
            {
                int count = produtcs.Count(s => s.Categpry?.CategoryName == category.CategoryName);
                categoryCounts[category.CategoryName] = count;
            }
            ViewBag.categoryCounts = categoryCounts;
            var productDbContext = _context.Products.Include(p => p.Categpry)
                .Where(p => p.Categpry.Id == Id);
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
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Price,Description,Image,CategpryId")] Product product, IFormFile Image)
        {

            if (ModelState.IsValid)
            {
                if (Image != null && Image.Length > 0)
                {
                    int maxId;
                    if (_context.Products.Count() == 0)
                    {
                        maxId = 0;
                    }
                    else
                    {
                        maxId = _context.Products.Max(p => p.Id) + 1;

                    }
                    // Lưu trữ tệp ảnh vào thư mục trên server hoặc bất kỳ nơi lưu trữ khác bạn chọn
                    product.Image = await SaveImage(Image, maxId);
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

            ViewBag.Price = product.Price;
            ViewData["CategpryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategpryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Price,Description,Image,CategpryId")] Product product, IFormFile Image)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (Image != null && Image.Length > 0)
                    {
                        // Lưu trữ tệp ảnh vào thư mục trên server hoặc bất kỳ nơi lưu trữ khác bạn chọn
                        product.Image = await SaveImage(Image, product.Id);
                    }
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

        private async Task<string> SaveImage(IFormFile file, int id)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            var uniqueFileName = id + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
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
