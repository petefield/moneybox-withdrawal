﻿using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var from = this.accountRepository.GetAccountById(fromAccountId);
            var to = this.accountRepository.GetAccountById(toAccountId);

            from.OnFundsLow += account => this.notificationService.NotifyFundsLow(account.User.Email);
            to.OnApproachingPayInLimit += account => this.notificationService.NotifyApproachingPayInLimit(account.User.Email);

            to.TransferFrom(from, amount); 

            this.accountRepository.Update(from);
            this.accountRepository.Update(to);
        }
    }
}
