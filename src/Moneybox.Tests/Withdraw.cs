using Moneybox.App;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using System;
using Xunit;

namespace Moneybox.Tests
{
    public class WithdrawTests
    {

        private readonly Mock<IAccountRepository> mockAccountsRepo;
        private readonly Mock<INotificationService> mockNotificationService;

        public WithdrawTests()
        {
            mockAccountsRepo = new Mock<IAccountRepository>();
            mockNotificationService = new Mock<INotificationService>();

        }

        [Fact]
        public void Throws_If_FromAccount_Insufficient_Funds()
        {

            var fromAccount = new Account { Id = Guid.NewGuid() };
            
            mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);

            var withdraw= new WithdrawMoney(mockAccountsRepo.Object, mockNotificationService.Object);

            Assert.Throws<InvalidOperationException>( () =>
            {
                withdraw.Execute(fromAccount.Id, fromAccount.Balance + 1);
            });

        }


        [Fact]
        public void Notifies_User_If_FromAccount_Funds_Low()
        {
           
            var fromAccount = new Account(paidIn: 1000, withdrawn: 0) { Id = Guid.NewGuid(), User = new User { Email = "fromUser@test.test" } };

        mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);


            var withdraw = new WithdrawMoney(mockAccountsRepo.Object, mockNotificationService.Object);

            withdraw.Execute(fromAccount.Id,  600);

            mockNotificationService.Verify(y => y.NotifyFundsLow(fromAccount.User.Email), Times.Once);
        }

        [Fact]
        public void Calls_Repo_Update_Once()
        {
            var fromAccount = new Account(paidIn: 1000, withdrawn: 0) { Id = Guid.NewGuid(),  User = new User { Email = "fromUser@test.test" } };
          
            mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);

   
            var withdraw = new WithdrawMoney(mockAccountsRepo.Object, mockNotificationService.Object);

            withdraw.Execute(fromAccount.Id, 600);

            mockAccountsRepo.Verify(x =>  x.Update(fromAccount),Times.Once);
        }


        [Fact]
        public void Deducts_FromAccount_Balance()
        {
            var fromAccount = new Account(paidIn: 1000, withdrawn: 0) { Id = Guid.NewGuid(),  User = new User { Email = "fromUser@test.test" } };

            mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);

            var withdraw = new WithdrawMoney(mockAccountsRepo.Object, mockNotificationService.Object);

            withdraw.Execute(fromAccount.Id,  600);

            Assert.Equal(1000-600, fromAccount.Balance );
        }

        [Fact]
        public void Deacreases_FromAccount_Withdrawn()
        {
            var fromAccount = new Account(paidIn: 1000, withdrawn: 0) { Id = Guid.NewGuid(),  User = new User { Email = "fromUser@test.test" } };

            mockAccountsRepo.Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);

            var withdraw = new WithdrawMoney(mockAccountsRepo.Object, mockNotificationService.Object);

            withdraw.Execute(fromAccount.Id, 600);

            Assert.Equal( -600, fromAccount.Withdrawn);
        }



    }
}
