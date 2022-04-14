using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoListMVCG.Data;
using ToDoListMVCG.Models.ViewModels.Users;

namespace ToDoListMVCG.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private UserManager<AppUser> _userManager;
        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        // GET: UsersController
        public ActionResult Index()
        {
            List<AppUserViewModel> users = _userManager.Users
                .Select(item => new AppUserViewModel()
                {
                    Id = item.Id,
                    UserName = item.UserName,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email,
                    RoleName = _userManager
                    .GetRolesAsync(item)
                    .Result
                    .FirstOrDefault()
                })
                .ToList();
            return View(users);
        }

        // GET: UsersController/Details/5
        public ActionResult Details(string id)
        {
            AppUser user = _userManager.FindByIdAsync(id).Result;

            return View(new AppUserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleName = _userManager
                    .GetRolesAsync(user)
                    .Result
                    .FirstOrDefault()
            });
        }

        // GET: UsersController/Create
        public ActionResult Create()
        {
            return View(new CreateAppUserViewModel());
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] CreateAppUserViewModel model)
        {
            try
            {
                AppUser user = new AppUser()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    CreatedAt = DateTime.Now,
                    CreatedById = _userManager
                        .GetUserAsync(User)
                        .Result
                        .Id,
                    EmailConfirmed = true
                };
                IdentityResult result = _userManager.CreateAsync(user, model.Password).Result;
                if (result.Succeeded)
                {
                    if (model.RoleName != "")
                    {
                        _userManager.AddToRoleAsync(user, model.RoleName).Wait();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Edit/5
        public ActionResult Edit(string id)
        {
            AppUser user = _userManager.FindByIdAsync(id).Result;

            return View(new CreateAppUserViewModel()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleName = _userManager
                    .GetRolesAsync(user)
                    .Result
                    .FirstOrDefault()
            });
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, CreateAppUserViewModel model)
        {
            try
            {
                var user = _userManager.FindByIdAsync(id).Result;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.ModifiedAt = DateTime.Now;
                user.ModifiedById = _userManager
                        .GetUserAsync(User)
                        .Result
                        .Id;
                if(model.Password != "")
                {
                    _userManager.RemovePasswordAsync(user).Wait();
                    _userManager.AddPasswordAsync(user, model.Password).Wait();
                }
                IdentityResult result = _userManager.UpdateAsync(user).Result;
                if(result.Succeeded)
                {
                    if (model.RoleName != "")
                    {
                        _userManager.RemoveFromRoleAsync(user, "Admin").Wait();
                        _userManager.RemoveFromRoleAsync(user, "RegularUser").Wait();
                        _userManager.AddToRoleAsync(user, model.RoleName).Wait();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Delete/5
        public ActionResult Delete(string id)
        {
            AppUser user = _userManager.FindByIdAsync(id).Result;

            return View(new AppUserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleName = _userManager
                    .GetRolesAsync(user)
                    .Result
                    .FirstOrDefault()
            });
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, IFormCollection collection)
        {
            try
            {
                IdentityResult result = _userManager.DeleteAsync(
                _userManager.FindByIdAsync(id).Result
                    ).Result;

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
