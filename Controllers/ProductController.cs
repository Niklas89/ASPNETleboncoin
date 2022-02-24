#nullable disable
using MetadataExtractor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lebonanimal.Datas;
using lebonanimal.Models;

namespace lebonanimal.Controllers
{
    public class ProductController : Controller
    {
        private readonly DbConnect _context;
        private readonly IWebHostEnvironment _he;

        public ProductController(DbConnect context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _he = hostEnvironment;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Firstname")))
            {
                TempData["error"] = "Vous devez vous connecter pour ajouter un produit";
                return RedirectToAction("Login", "User", 
                new {redirectTo=$"/{ControllerContext.RouteData.Values["Controller"]}/{ControllerContext.RouteData.Values["Action"]}" });
            }
            ViewBag.Category = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Price,ImgPath,Description,Certificat")] Product product, List<IFormFile> imgPath, List<IFormFile> certificat)
        {
            
            if (HttpContext.Session.GetInt32("Id") == null)
            {
                TempData["error"] = "Vous devez vous connecter pour ajouter un produit";
                return RedirectToAction("Login","User", 
                        new {redirectTo=$"/{ControllerContext.RouteData.Values["Controller"]}/{ControllerContext.RouteData.Values["Action"]}" });
            }
            if (!int.TryParse(Request.Form["Category.Id"], out var categoryId))
            {
                TempData["error"] = "Categorie invalide";
                return View();
            }
            ViewBag.Category = new SelectList(_context.Categories, "Id", "Name");// in case error cateogry is still filled
            // pour les photos 
            var pathPhotos = Path.Combine(_he.WebRootPath, "files/photos");
            if (!System.IO.Directory.Exists(pathPhotos))
            {
                System.IO.Directory.CreateDirectory(pathPhotos);
            }
        

            var uploadedFiles = new List<string>();
            foreach (var postedFile in imgPath)
            {
                var fileName = Path.Combine(pathPhotos, $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{postedFile.FileName}");

                await using var stream = new FileStream(fileName, FileMode.Create);
                await postedFile.CopyToAsync(stream);
                uploadedFiles.Add(fileName);
                product.ImgPath = fileName;

                if (new FileInfo(fileName).Length > 1_200_000)
                {
                    System.IO.File.Delete(fileName);
                    TempData["error"] = "fichier trop gros 1.2Mo max";
                    return View();
                }
                if (!postedFile.ContentType.Contains("jpeg") && !postedFile.ContentType.Contains("png"))
                {
                    System.IO.File.Delete(fileName);
                    TempData["error"] = "nous acceptons seulement les jpeg ou les png";
                    return View();
                }
                stream.Close();
                var directories = ImageMetadataReader.ReadMetadata(fileName);
                var file = directories[0];
                var width = 0;
                var height = 0;
                
                if (postedFile.ContentType.Contains("jpeg"))
                {
                    int.TryParse(file.Tags[3].Description?.Split(" ")[0], out  width);
                    int.TryParse(file.Tags[2].Description?.Split(" ")[0], out  height);
                }
                else
                {
                    int.TryParse(file.Tags[0].Description?.Split(" ")[0], out  width);
                    int.TryParse(file.Tags[1].Description?.Split(" ")[0], out  height);
                }

                if (width >1920 || height > 1500)
                {
                    System.IO.File.Delete(fileName);
                    TempData["error"] = "fichier trop grand en dimension 1920*1500 max";
                    return View();
                }
                //string fileName = Path.GetFileName(product.ImgPath);
            }

            // pour les certificats
            var pathCertificates = Path.Combine(_he.WebRootPath, "files/certificates");
            if (!System.IO.Directory.Exists(pathCertificates))
            {
                System.IO.Directory.CreateDirectory(pathCertificates);
            }

            var uploadedCerts = new List<string>();
            foreach (var postedFile in certificat)
            {
                if (!postedFile.ContentType.Contains("pdf"))
                {
                    TempData["error"] = "le certificat n'est pas un pdf";
                    return View();
                }   
                var fileName =  Path.Combine(pathCertificates,  DateTime.Now.ToFileTime() +  postedFile.FileName);
                await using var stream = new FileStream(fileName, FileMode.Create);
                await postedFile.CopyToAsync(stream);
                if (new FileInfo(fileName).Length > 1_200_000)
                {
                  System.IO.File.Delete(fileName);
                  TempData["error"] = "fichier trop grand en dimension 1920*1500 max";
                  return View();
                }
                uploadedCerts.Add(fileName);
                product.Certificat = fileName;
            }

            product.User = _context.Users.Find(HttpContext.Session.GetInt32("Id"));
            product.Category = _context.Categories.Find(categoryId);
            //if (ModelState.IsValid)
            //{
                _context.Add(product);
                await _context.SaveChangesAsync();
                  return RedirectToAction("Index");
            //}
            return View();
        }

        // GET: Product/Edit/5
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
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Price,ImgPath,Description,Certificat,Enabled,Deleted")] Product product)
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
            return View(product);
        }

        // GET: Product/Buy/5
        public async Task<IActionResult> Buy(int? id)
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
            return View(product);
        }

        // POST: Product/Buy/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            try
            {
 
                var prod = _context.Products.SingleOrDefault(x => x.Id==product.Id); 
                _context.Update(prod);

                Order order = new Order();
                order.UserId = HttpContext.Session.GetInt32("Id").Value;
                order.ProductId = product.Id;
                _context.Add(order);

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
            return RedirectToAction("Index");
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
