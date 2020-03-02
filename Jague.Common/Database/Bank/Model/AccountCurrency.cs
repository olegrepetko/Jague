﻿namespace Jague.Common.Database.Bank.Model
{
    public partial class AccountCurrency
    {
        public uint Id { get; set; }
        public byte CurrencyId { get; set; }
        public ulong Amount { get; set; }

        public virtual Account IdNavigation { get; set; }
    }
}
