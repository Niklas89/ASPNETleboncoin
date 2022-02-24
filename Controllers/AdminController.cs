#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lebonanimal.Datas;
using lebonanimal.Models;

namespace lebonanimal.Controllers
{
    public class AdminController : Controller
    {
        private readonly DbConnect _context;

        public AdminController(DbConnect context)
        {
            _context = context;
        }
        // GET: User
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: User
        public async Task<IActionResult> ModifyUser()
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            return View("User/ModifyUser", await _context.Users.ToListAsync());
        }


        // GET: User/Create
        public IActionResult ModifyUsers()
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            return View("User/ModifyUser");
        }
        // GET: User/Details/5
        public async Task<IActionResult> DetailsUser(int? id)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View("User/DetailsUser", user);
        }

        // GET: User/Create
        public IActionResult CreateUser()
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            return View("User/CreateUser");
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("Id,Firstname,Lastname,Email,Password,ConfirmPassword,Banned,Admin")] User user)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            if (!ModelState.IsValid) return View(user);
            user.Password = Argon2.Hash(user.Password);
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("User/ModifyUser");
        }

        // GET: User/Edit/5
        public async Task<IActionResult> EditUser(int? id)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("User/EditUser", user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, [Bind("Id,Firstname,Lastname,Email,Password,Banned,Admin")] User user)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View("User/EditUser", user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> DeleteUser(int? id)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View("User/DeleteUser", user);
        }

        // GET: Product
        public async Task<IActionResult> UnbanUser()
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            return View("User/UnbanUser", await _context.Users.Where(x => x.Banned == true).ToListAsync());
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnbanUser(int id)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);

            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Banned = false;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Redirect("/Admin/UnbanUser");
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }



        // GET: Product
        public async Task<IActionResult> IndexProduct()
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            return View("Product/IndexProduct", await _context.Products.ToListAsync());
        }

        // GET: Product
        public async Task<IActionResult> DeletedProduct()
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            return View("Product/DeletedProduct", await _context.Products.Where(x => x.Deleted == true).ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> DetailsProduct(int? id)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
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

            return View("Product/DetailsProduct", product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> EditProduct(int? id)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View("Product/EditProduct", product);
        }


        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, [Bind("Id,Title,Price,ImgPath,Description,Certificat,Enabled,Deleted")] Product product)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
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
            return View("Product/EditProduct", product);
        }

        // GET: Product
        public async Task<IActionResult> ApproveProduct()
        {
            if ( HttpContext.Session.GetInt32("IsAdmin") is null or 0 )
            {
                return NotFound();
            }
            return View("Product/ApproveProduct", await _context.Products.Where(x=>x.Deleted == false && x.Enabled==false).ToListAsync());
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveProduct(int id)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.Enabled = true;
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
            }
            return Redirect("/Admin/ApproveProduct");
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisapproveProduct(int id)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.Deleted = true;
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
            }
            return Redirect("/Admin/ApproveProduct");
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (HttpContext.Session.GetInt32("IsAdmin") is null or 0)
            {
                return NotFound();
            }
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

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
