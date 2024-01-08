using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Models;

namespace SalesWebMvc.Services
{
    public class SellerService
    {
        private SalesWebMvcContext _context;

        public SellerService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public List<Seller> FindAll()
        {
            return _context.Seller.ToList();
        }

        public Seller findById(int id)
        {
            return _context.Seller.Include(x => x.Department).SingleOrDefault(s => s.Id == id);
        }

        public void Insert(Seller seller)
        {
            _context.Seller.Add(seller);
            _context.SaveChanges();
        }

        public void Delete(Seller seller)
        {
            _context.Seller.Remove(seller);
            _context.SaveChanges();
        }
    }
}
