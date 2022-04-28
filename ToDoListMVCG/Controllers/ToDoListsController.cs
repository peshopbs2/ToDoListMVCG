using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDoListMVCG.Data;
using ToDoListMVCG.Models.ViewModels.ToDoLists;

namespace ToDoListMVCG.Controllers
{
    [Authorize(Roles="Admin")]
    public class ToDoListsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ToDoListsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ToDoLists
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var applicationDbContext = _context.ToDoList.Include(t => t.CreatedBy).Include(t => t.ModifiedBy);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var applicationDbContext = _context.ToDoList
                    .Where(item => item.CreatedBy.Id== currentUser.Id || item.SharedWith.Any(shared => shared.User.Id == currentUser.Id))
                    .Include(t => t.CreatedBy).Include(t => t.ModifiedBy);
                return View(await applicationDbContext.ToListAsync());
            }
        }

        // GET: ToDoLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoList = await _context.ToDoList
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoList == null)
            {
                return NotFound();
            }

            return View(toDoList);
        }

        // GET: ToDoLists/Create
        public IActionResult Create()
        {
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ToDoLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Id")] ToDoList toDoList)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                toDoList.CreatedById = currentUser.Id;
                toDoList.CreatedAt = DateTime.Now;
                _context.Add(toDoList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", toDoList.CreatedById);
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id", toDoList.ModifiedById);
            return View(toDoList);
        }

        // GET: ToDoLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoList = await _context.ToDoList.FindAsync(id);
            if (toDoList == null)
            {
                return NotFound();
            }
            ViewData["CreatedAt"] = toDoList.CreatedAt;
            ViewData["ModifiedAt"] = toDoList.ModifiedAt;
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", toDoList.CreatedById);
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id", toDoList.ModifiedById);
            return View(toDoList);
        }

        // POST: ToDoLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Id,CreatedById,CreatedAt")] ToDoList toDoList)
        {
            if (id != toDoList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    toDoList.ModifiedAt = DateTime.Now;
                    toDoList.ModifiedById = currentUser.Id;
                    _context.Update(toDoList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoListExists(toDoList.Id))
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
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", toDoList.CreatedById);
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id", toDoList.ModifiedById);
            return View(toDoList);
        }

        public async Task<IActionResult> Share(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoList = await _context.ToDoList.FindAsync(id);
            if (toDoList == null)
            {
                return NotFound();
            }

            var shareList = new List<string>();
            if (toDoList.SharedWith.Count > 0)
            {
                shareList = toDoList.SharedWith
                    .Select(item => item.UserId).ToList();
            }

            return View(new ToDoListShareViewModel()
            {
                Id = toDoList.Id,
                Title = toDoList.Title,
                CreatedAt = toDoList.CreatedAt,
                CreatedById = toDoList.CreatedById,
                ModifiedAt = toDoList.ModifiedAt,
                ModifiedById = toDoList.ModifiedById,
                SharedWith = _context.Users
                .Select(item => new SelectListItem()
                {
                    Value = item.Id,
                    Text = item.UserName
                })

                .ToList()
            });
        }

        // POST: ToDoLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Share(int id, ToDoListShareViewModel model)
        {
            var toDoList = await _context.ToDoList.FindAsync(id);

            if (model.Id != toDoList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    toDoList.ModifiedAt = DateTime.Now;
                    toDoList.ModifiedById = currentUser.Id;

                    var sharedWith = model.SharedWithIds;

                    toDoList.SharedWith.Clear();
                    await _context.SaveChangesAsync();

                    toDoList.SharedWith = new List<Share>();
                    foreach (var sharedId in sharedWith)
                    {
                        toDoList.SharedWith.Add(new Data.Share()
                        {
                            ToDoListId = toDoList.Id,
                            UserId = sharedId
                        });
                    }
                    _context.Update(toDoList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoListExists(toDoList.Id))
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
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", toDoList.CreatedById);
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id", toDoList.ModifiedById);
            ViewData["SharedWith"] = new MultiSelectList(_context.Users, "Id", "UserName");
            return View(toDoList);
        }

        // GET: ToDoLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoList = await _context.ToDoList
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoList == null)
            {
                return NotFound();
            }

            return View(toDoList);
        }

        // POST: ToDoLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toDoList = await _context.ToDoList.FindAsync(id);
            var currentUser = await _userManager.GetUserAsync(User);

            if(toDoList.CreatedById == currentUser.Id)
            {
                //it's ok to delete your own todo list!
                toDoList.SharedWith.Clear();
                await _context.SaveChangesAsync();

                _context.ToDoList.Remove(toDoList);
                await _context.SaveChangesAsync();
            } else
            {
                //it's not ok to delete someone's else's todo list
                var share = toDoList
                    .SharedWith
                    .Where(item => item.UserId == currentUser.Id)
                    .FirstOrDefault();
                toDoList.SharedWith.Remove(share);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoListExists(int id)
        {
            return _context.ToDoList.Any(e => e.Id == id);
        }
    }
}
