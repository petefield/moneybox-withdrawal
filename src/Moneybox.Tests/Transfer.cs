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
        public void Throws_If_FromAccount_Insufficient_Funds()
        {

            var fromAccount = new Account { Id = Guid.NewGuid(), Balance = 0 };
            var toAccount = new Account { Id = Guid.NewGuid(), Balance = 0 };
            
            mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);

            mockAccountsRepo.Setup(x => x.GetAccountById(toAccount.Id))
                .Returns(toAccount);

            var transfer = new TransferMoney(mockAccountsRepo.Object, mockNotificationService.Object);

            Assert.Throws<InvalidOperationException>( () =>
            {
                transfer.Execute(fromAccount.Id, toAccount.Id, fromAccount.Balance + 1);
            });

        }

        [Fact]
        public void Throws_If_ToAccount_PayInLimit_Reached()
        {

            var fromAccount = new Account { Id = Guid.NewGuid(), Balance = 0 };
            var toAccount = new Account { Id = Guid.NewGuid(), Balance = Account.PayInLimit +1 };

            mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);

            mockAccountsRepo.Setup(x => x.GetAccountById(toAccount.Id))
                .Returns(toAccount);

            var transfer = new TransferMoney(mockAccountsRepo.Object, mockNotificationService.Object);

            Assert.Throws<InvalidOperationException>(() =>
            {
                transfer.Execute(fromAccount.Id, toAccount.Id,  Account.PayInLimit +1);
            });

        }

        [Fact]
        public void Notifies_User_If_FromAccount_Funds_Low()
        {
           
            var fromAccount = new Account { Id = Guid.NewGuid(), Balance = 1000, User = new User { Email = "fromUser@test.test" } };
            var toAccount = new Account { Id = Guid.NewGuid(), Balance = 0, User = new User { Email = "toUser@test.test" } };

        mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);

            mockAccountsRepo.Setup(x => x.GetAccountById(toAccount.Id))
                .Returns(toAccount);

            var transfer = new TransferMoney(mockAccountsRepo.Object, mockNotificationService.Object);

            transfer.Execute(fromAccount.Id, toAccount.Id, 600);

            mockNotificationService.Verify(y => y.NotifyFundsLow(fromAccount.User.Email), Times.Once);
        }

        [Fact]
        public void Notifies_User_If_ToAccount_PayinLimit_Approaching()
        {
            var fromAccount = new Account { Id = Guid.NewGuid(), Balance = 1000, User = new User { Email = "fromUser@test.test" } };
            var toAccount = new Account { Id = Guid.NewGuid(),PaidIn = 3000, Balance = 3000, User = new User { Email = "toUser@test.test" } };

            mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);

            mockAccountsRepo.Setup(x => x.GetAccountById(toAccount.Id))
                .Returns(toAccount);

            var transfer = new TransferMoney(mockAccountsRepo.Object, mockNotificationService.Object);

            transfer.Execute(fromAccount.Id, toAccount.Id, 600);

            mockNotificationService.Verify(y => y.NotifyApproachingPayInLimit(toAccount.User.Email), Times.Once);
        }

        [Fact]
        public void Calls_Repo_Update_1_for_Each_Account()
        {
            var fromAccount = new Account { Id = Guid.NewGuid(), Balance = 1000, User = new User { Email = "fromUser@test.test" } };
            var toAccount = new Account { Id = Guid.NewGuid(), PaidIn = 3000, Balance = 3000, User = new User { Email = "toUser@test.test" } };

            mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);

            mockAccountsRepo.Setup(x => x.GetAccountById(toAccount.Id))
                .Returns(toAccount);

            var transfer = new TransferMoney(mockAccountsRepo.Object, mockNotificationService.Object);

            transfer.Execute(fromAccount.Id, toAccount.Id, 600);

            mockAccountsRepo.Verify(x =>  x.Update(fromAccount),Times.Once);
            mockAccountsRepo.Verify(x => x.Update(toAccount), Times.Once);
        }


        [Fact]
        public void Doesnt_Alter_TotalBalance()
        {
            var fromAccount = new Account { Id = Guid.NewGuid(), Balance = 1000, User = new User { Email = "fromUser@test.test" } };
            var toAccount = new Account { Id = Guid.NewGuid(), PaidIn = 3000, Balance = 3000, User = new User { Email = "toUser@test.test" } };

            mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);

            mockAccountsRepo.Setup(x => x.GetAccountById(toAccount.Id))
                .Returns(toAccount);

            var transfer = new TransferMoney(mockAccountsRepo.Object, mockNotificationService.Object);

            transfer.Execute(fromAccount.Id, toAccount.Id, 600);

            Assert.Equal(4000, fromAccount.Balance + toAccount.Balance);
        }

        [Fact]
        public void Deducts_FromAccount_Balance()
        {
            var fromAccount = new Account { Id = Guid.NewGuid(), Balance = 1000, User = new User { Email = "fromUser@test.test" } };
            var toAccount = new Account { Id = Guid.NewGuid(), PaidIn = 3000, Balance = 3000, User = new User { Email = "toUser@test.test" } };

            mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);

            mockAccountsRepo.Setup(x => x.GetAccountById(toAccount.Id))
                .Returns(toAccount);

            var transfer = new TransferMoney(mockAccountsRepo.Object, mockNotificationService.Object);

            transfer.Execute(fromAccount.Id, toAccount.Id, 600);

            Assert.Equal(1000-600, fromAccount.Balance );
        }

        [Fact]
        public void Increases_ToAccount_Balance()
        {
            var fromAccount = new Account { Id = Guid.NewGuid(), Balance = 1000, User = new User { Email = "fromUser@test.test" } };
            var toAccount = new Account { Id = Guid.NewGuid(), PaidIn = 3000, Balance = 3000, User = new User { Email = "toUser@test.test" } };

            mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);

            mockAccountsRepo.Setup(x => x.GetAccountById(toAccount.Id))
                .Returns(toAccount);

            var transfer = new TransferMoney(mockAccountsRepo.Object, mockNotificationService.Object);

            transfer.Execute(fromAccount.Id, toAccount.Id, 600);

            Assert.Equal(3000 + 600, toAccount.Balance);
        }
    }
}
