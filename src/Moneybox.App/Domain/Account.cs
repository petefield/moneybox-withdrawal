﻿using System;

namespace Moneybox.App
{
    public class Account
    {
        public event Action<Account> OnFundsLow;
        public event Action<Account> OnApproachingPayInLimit;

        public Account()
        {

        }

        public Account(decimal paidIn, decimal withdrawn)
        {
            this.PaidIn = paidIn;
            this.Withdrawn = withdrawn;
        }


        public const decimal PayInLimit = 4000m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance => this.PaidIn + this.Withdrawn;
            
        public decimal Withdrawn { get; private set; }

        public decimal PaidIn { get; private set; }

        public void Withdraw(decimal amount)
        {
            if (amount < 0) {
                throw new ArgumentOutOfRangeException("amount", "Amount to withdraw must be greater than 0");
            }

            if (amount > this.Balance)
            {
                throw new InvalidOperationException( "Insufficient funds to make transfer");
            }

            this.Withdrawn -= amount;

            if (this.Balance < 500)
            {
                this.OnFundsLow?.Invoke(this);
            }

        }

        public void TransferFrom(Account from, decimal amount)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }

            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Amount to transfer must be greater than 0");
            }

            var paidIn = this.PaidIn + amount;

            if (paidIn > Account.PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            from.Withdraw(amount);

            this.PaidIn = this.PaidIn + amount;

            if (Account.PayInLimit - paidIn < 500m)
            {
                this.OnApproachingPayInLimit?.Invoke(this);
            }
        }
    }
}
