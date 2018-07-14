using Moneybox.App;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using System;
using Xunit;

namespace Moneybox.Tests
{
    public class TransferTests
    {

        private readonly Mock<IAccountRepository> mockAccountsRepo;
        private readonly Mock<INotificationService> mockNotificationService;

        public TransferTests()
        {
            mockAccountsRepo = new Mock<IAccountRepository>();
            mockNotificationService = new Mock<INotificationService>();

        }

        [Fact]
        public void Throw_Exception_If_FromAccount_Insufficient_Funds()
        {

            var fromAccount = new Account { Id = Guid.NewGuid(), Balance = 0 };
            var toAccount = new Account { Id = Guid.NewGuid(), Balance = 0 };
            
            mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);

            mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
             .Returns(fromAccount);

            var transfer = new TransferMoney(mockAccountsRepo.Object, mockNotificationService.Object);

            Assert.Throws<InvalidOperationException>( () =>
            {
                transfer.Execute(fromAccount.Id, toAccount.Id, fromAccount.Balance + 1);
            });

        }
    }
}
