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

        public void TransferFrom(Account from, decimal amount)
        {

            var fromBalance = from.Balance - amount;
            if (fromBalance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }

            if (fromBalance < 500m)
            {
                from.OnFundsLow?.Invoke(from);
              //  this.notificationService.NotifyFundsLow(from.User.Email);
            }

            var paidIn = this.PaidIn + amount;
            if (paidIn > Account.PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            if (Account.PayInLimit - paidIn < 500m)
            {
                this.OnApproachingPayInLimit?.Invoke(this);
             //   this.notificationService.NotifyApproachingPayInLimit(to.User.Email);
            }

            from.Balance = from.Balance - amount;
            from.Withdrawn = from.Withdrawn - amount;

            this.Balance = this.Balance + amount;
            this.PaidIn = this.PaidIn + amount;

        }
    }
}
