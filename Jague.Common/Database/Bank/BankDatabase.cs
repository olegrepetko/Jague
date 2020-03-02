using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Jague.Common.Database.Bank.Model;
using Jague.Common.Cryptography;

namespace Jague.Common.Database.Bank
{
    public static class BankDatabase
    {
        public static async Task Save(Action<BankContext> action)
        {
            using (var context = new BankContext())
            {
                action.Invoke(context);
                await context.SaveChangesAsync();
            }
        }

        public static async Task<Account> GetAccountAsync(string email)
        {
            using (var context = new BankContext())
                return await context.Account.SingleOrDefaultAsync(a => a.Email == email);
        }

        public static async Task<Account> GetAccountAsync(string email, Guid guid)
        {
            string serverToken = guid.ToByteArray().ToHexString();
            using (var context = new BankContext())
                return await context.Account.SingleOrDefaultAsync(a => a.Email == email && a.ServerToken == serverToken);
        }

        public static async Task<Account> GetAccountAsync(string email, byte[] sessionKeyBytes)
        {
            string sessionKey = BitConverter.ToString(sessionKeyBytes).Replace("-", "");
            using (var context = new BankContext())
                return await context.Account.SingleOrDefaultAsync(a => a.Email == email && a.SessionKey == sessionKey);
        }

        public static void CreateAccount(string email, string password)
        {
            using (var context = new BankContext())
            {
                byte[] s = RandomProvider.GetBytes(16u);
                byte[] v = SecureRemotePasswordProvider.GenerateVerifier(s, email, password);

                context.Account.Add(new Account
                {
                    Email = email,
                    PasswordSalt = s.ToHexString(),
                    PasswordVerifier = v.ToHexString()
                });

                context.SaveChanges();
            }
        }

        public static bool DeleteAccount(string email)
        {
            using (var context = new BankContext())
            {
                Account account = context.Account.SingleOrDefault(a => a.Email == email);
                if (account == null)
                    return false;

                context.Account.Remove(account);
                return context.SaveChanges() > 0;
            }
        }

        public static async Task UpdateAccountServerToken(Account account, Guid guid)
        {
            account.ServerToken = guid.ToByteArray().ToHexString();
            using (var context = new BankContext())
            {
                EntityEntry<Account> entity = context.Attach(account);
                entity.Property(p => p.ServerToken).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public static async Task UpdateAccountSessionKey(Account account, byte[] sessionKeyBytes)
        {
            account.SessionKey = BitConverter.ToString(sessionKeyBytes).Replace("-", "");
            using (var context = new BankContext())
            {
                EntityEntry<Account> entity = context.Attach(account);
                entity.Property(p => p.SessionKey).IsModified = true;
                await context.SaveChangesAsync();
            }
        }

        public static ImmutableList<ServerMessage> GetServerMessages()
        {
            using (var context = new BankContext())
                return context.ServerMessage
                    .AsNoTracking()
                    .ToImmutableList();
        }
    }
}
