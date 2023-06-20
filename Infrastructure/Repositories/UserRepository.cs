﻿using Domain;
using DomainServices.Interfaces.Repositories;
using Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) => _context = context;

        public async Task<User> GetUserByName(string username) => await _context.User.Where(user => user.Name == username).FirstOrDefaultAsync();

        public async Task<bool> CreateUser(User user)
        {
            try
            {
                await _context.User.AddAsync(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddSatoshi(string username, string satoshi)
        {
            try
            {
                var user = await _context.User.SingleOrDefaultAsync(u => u.Name == username);
                if (user == null) { return false; }
                user.Satoshi += satoshi;
                await _context.SaveChangesAsync();

                var Updated = await _context.User.SingleOrDefaultAsync(u => u.Name == username);
                return true;

            }catch (Exception e) { 
                return false;
            }
        }
    }
}
