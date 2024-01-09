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

        public async Task<IActionResult> Index()
        {
            var list = await _sellerService.FindAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            return await ViewCreateAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Valida contra ataque de CSSR
        public async Task<IActionResult> CreateAsync(Seller seller)
        {
            if (!ModelState.IsValid)
            {
                return await ViewCreateAsync(seller);
            }
            await _sellerService.InsertAsync(seller);
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> ViewCreateAsync(Seller seller = null)
        {
            var departments = await _departmentService.FindAllAsync();
            var viewModel = new SellerFormViewModel { Departments = departments, Seller = seller };
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            return View(await _sellerService.findByIdAsync(id));
        }

        public async Task<IActionResult> Delete(int id)
        {
            return View(await _sellerService.findByIdAsync(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Seller seller)
        {
            try
            {
                await _sellerService.DeleteAsync(seller);
                return RedirectToAction(nameof(Index));
            }catch(IntegrityException e)
            {
                return RedirectError(e.Message);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return RedirectError("Id not provided.");
            }
            var seller = await _sellerService.findByIdAsync(id.Value);
            if(seller == null)
            {
                return RedirectError("Id not fond!!!!");
            }
            var departments = await _departmentService.FindAllAsync();
            var viewModel = new SellerFormViewModel { Departments = departments, Seller = seller };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seller seller)
        {
            if (id != seller.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return await ViewCreateAsync(seller);
            }

            try
            {
                await _sellerService.UpdateAsync(seller);
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
