using System;
using System.Collections.Generic;

namespace Jague.Common.Database.Bank.Model
{
    public partial class Account
    {
        public Account()
        {
            AccountCurrency = new HashSet<AccountCurrency>();
        }

        public uint Id { get; set; }
        public string Email { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordVerifier { get; set; }
        public string ServerToken { get; set; }
        public string SessionKey { get; set; }
        public DateTime CreateTime { get; set; }

        public virtual ICollection<AccountCurrency> AccountCurrency { get; set; }
    }
}
