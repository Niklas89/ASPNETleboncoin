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
    public class UserController : Controller
    {
        private readonly DbConnect _context;

        public UserController(DbConnect context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Firstname,Lastname,Email,Password,ConfirmPassword,Banned,Admin")] User user)
        {
            if (!ModelState.IsValid) return View(user);
            user.Password = Argon2.Hash(user.Password);
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
// GET: User/Create
        public IActionResult Login(string? redirectTo)
        {
            ViewBag.redirectTo = redirectTo;
            return View();
        }

        // POST: User/Login
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserLogin user,string? redirectTo)
        {
            if (!ModelState.IsValid) return View(user);
            var userDb = _context.Users.Single(userDb => userDb.Email == user.Email);
 
            if(!Argon2.Verify(userDb.Password,user.Password))
            {
                TempData["error"] = "mot de passe ou mail incorrect";
                return View(user);
            }
            HttpContext.Session.SetString("Firstname",userDb.Firstname);
            HttpContext.Session.SetString("Lastname",userDb.Lastname);
            HttpContext.Session.SetString("Email",userDb.Email);
            HttpContext.Session.SetInt32("Id",userDb.Id);
            HttpContext.Session.SetInt32("IsAdmin",userDb.Admin ? 1 : 0);

            if (!string.IsNullOrEmpty(redirectTo))
            {
                return Redirect(redirectTo);
            }
            return RedirectToAction("Index");
            
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Firstname,Lastname,Email,Password,Banned,Admin")] User user)
        {
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
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: User products ordered
        public async Task<IActionResult> UserProductsOrdered()
        {
            int? id = HttpContext.Session.GetInt32("Id");
            IEnumerable<UserProductsOrdered> orderedProducts = from o in _context.Orders 
                                  join u in _context.Users on o.UserId equals id
                                  join p in _context.Products on o.ProductId equals p.Id
                                  select new UserProductsOrdered { OrderVm = o, UserVm = u, ProductVm = p }; 
            

            return View(orderedProducts);
        }

        // GET: List of products published by the connected user
        public async Task<IActionResult> ListPublishedProducts()
        {
            int? id = HttpContext.Session.GetInt32("Id");
            IEnumerable<Product> publishedOrder = _context.Products.Where(p => p.UserId == id).ToList();
            return View(publishedOrder);
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        public IActionResult AddAnnonce()
        {
            return View();
        }

        public IActionResult AdminPage()
        {
            return View(_context.Products.ToList());
        }

        public IActionResult ConnectedClient()
        {
            return View();
        }


    }
}
