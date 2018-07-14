using System;

namespace Moneybox.App
{
    public class Account
    {
        public event Action<Account> OnFundsLow;
        public event Action<Account> OnApproachingPayInLimit;

        public const decimal PayInLimit = 4000m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public void Withdraw(decimal amount)
        {
            if (amount < 0) {
                throw new ArgumentOutOfRangeException("amount", "Amount to withdraw must be greater than 0");
            }

            if (amount > this.Balance)
            {
                throw new InvalidOperationException( "Insufficient funds to make transfer");
            }

            this.Balance -= amount;
            this.Withdrawn -= amount;

            if (this.Balance < 500)
            {
                this.OnFundsLow?.Invoke(this);
            }

        }

        public void TransferFrom(Account from, decimal amount)
        {
            var paidIn = this.PaidIn + amount;

            if (paidIn > Account.PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            from.Withdraw(amount);

            this.Balance = this.Balance + amount;
            this.PaidIn = this.PaidIn + amount;

            if (Account.PayInLimit - paidIn < 500m)
            {
                this.OnApproachingPayInLimit?.Invoke(this);
            }
        }
    }
}
