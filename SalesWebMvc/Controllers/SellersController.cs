using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Controllers
{
    public class SellersController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        public object SellerFormViewmodel { get; private set; }

        public SellersController(SellerService sellerService, DepartmentService departmentService)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        public IActionResult Index()
        {
            var list = _sellerService.FindAll();
            return View(list);
        }

        public IActionResult Create()
        {
            return ViewCreate();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Valida contra ataque de CSSR
        public IActionResult Create(Seller seller)
        {
            if (!ModelState.IsValid)
            {
                return ViewCreate(seller);
            }
            _sellerService.Insert(seller);
            return RedirectToAction(nameof(Index));
        }

        private IActionResult ViewCreate(Seller seller = null)
        {
            var departments = _departmentService.FindAll();
            var viewModel = new SellerFormViewModel { Departments = departments, Seller = seller };
            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            return View(_sellerService.findById(id));
        }

        public IActionResult Delete(int id)
        {
            return View(_sellerService.findById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Seller seller)
        {
            _sellerService.Delete(seller);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if(id == null)
            {
                return RedirectError("Id not provided.");
            }
            var seller = _sellerService.findById(id.Value);
            if(seller == null)
            {
                return RedirectError("Id not fond!!!!");
            }
            var departments = _departmentService.FindAll();
            var viewModel = new SellerFormViewModel { Departments = departments, Seller = seller };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Seller seller)
        {
            if (id != seller.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return ViewCreate(seller);
            }

            try
            {
                _sellerService.Update(seller);
                return RedirectToAction(nameof(Index));
            }
            catch (DbConcurrencyException)
            {
                return BadRequest();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        private IActionResult RedirectError(string msg)
        {
            return RedirectToAction(nameof(Error),new { message = msg});
        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(viewModel);
        }
    }
}
