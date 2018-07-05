﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoiceAPI.Components.DataContext;
using InvoiceAPI.Components.Entities;
using InvoiceAPI.Components.Helpers;
using InvoiceAPI.Components.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace InvoiceAPI.Components.Services
{
    public class UserRepository : IUserRepository
    {
        private InvoiceContext _context = new InvoiceContext();
        private Encryptor _encryptor = new Encryptor();
        private Random random = new Random();

        public async Task<User> Authenticate(string email, string password)
        {
            //Encrypt password to match 
            string encryptedPassword = _encryptor.Encrypt(password);

            var response = await _context.Users.FirstOrDefaultAsync(q => q.Email.ToLower() == email.ToLower() && q.Password == encryptedPassword);
            return response;
        }

        public async Task<ICollection<User>> GetUsers()
        {
            var response = await _context.Users.ToListAsync();
            return response;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var response = await _context.Users.FirstOrDefaultAsync(q => q.Email.ToLower() == email.ToLower());
            return response;
        }

        public async Task<User> Insert(User user)
        {
            user.Password = _encryptor.Encrypt(user.Password);
            var response = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return response.Entity;
        }

        public async Task<User> Update(User user)
        {
            var userBeforeUpdate = await _context.Users.FindAsync(user.Email);
            if (userBeforeUpdate == null)
            {
                return null;
            }

            // Check if password has changed
            if (userBeforeUpdate.Password != user.Password)
            {
                //Encrypt password before updating
                userBeforeUpdate.Password = _encryptor.Encrypt(user.Password);
            }

            _context.Entry(userBeforeUpdate).CurrentValues.SetValues(user);
            var result = await _context.SaveChangesAsync();

            return result == 1 ? user : null;
        }

        public async Task<bool> Delete(string email)
        {
            User user = await _context.Users.FirstOrDefaultAsync(q => q.Email.ToLower() == email.ToLower());
            _context.Users.Remove(user);

            var result = await _context.SaveChangesAsync();
            return result == 1 ? true : false;
        }

        public async Task<string> ResetPassword(string email)
        {
            // Generate random password
            var tempPassword = this.GeneratePassword();

            // Get user by email
            var user = await _context.Users.FirstOrDefaultAsync(q => q.Email.ToLower() == email.ToLower());

            // Update password
            user.Password = _encryptor.Encrypt(tempPassword);
            await _context.SaveChangesAsync();

            return tempPassword;
        }

        #region Private Methods

        private string GeneratePassword()
        {
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, 10)
                .Select(x => pool[random.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }

        #endregion
    }
}
