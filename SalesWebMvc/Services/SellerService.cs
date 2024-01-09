using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Models;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Services
{
    public class SellerService
    {
        private SalesWebMvcContext _context;

        public SellerService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public async Task<List<Seller>> FindAllAsync()
        {
            return await _context.Seller.ToListAsync();
        }

        public async Task<Seller> findByIdAsync(int id)
        {
            return await _context.Seller.Include(x => x.Department).SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task InsertAsync(Seller seller)
        {
            _context.Seller.Add(seller);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Seller seller)
        {
            try
            {
                _context.Seller.Remove(seller);
                await _context.SaveChangesAsync();
            }catch(DbUpdateException e)
            {
                throw new IntegrityException(e.Message);
            }
        }

        public async Task UpdateAsync(Seller seller)
        {
            bool foundId = await _context.Seller.AnyAsync(s => s.Id == seller.Id);
            if (!foundId)
            {
                throw new NotFoundException("Id not found.");
            }

            try
            {
                _context.Update(seller);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
    }
}
