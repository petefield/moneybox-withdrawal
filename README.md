# Moneybox Money Withdrawal

The solution contains a .NET core library (Moneybox.App) which is structured into the following 3 folders:

* Domain - this contains the domain models for a user and an account, and a notification service.
* Features - this contains two operations, one which is implemented (transfer money) and another which isn't (withdraw money)
* DataAccess - this contains a repository for retrieving and saving an account (and the nested user it belongs to)

## The task

The task is to implement a money withdrawal in the WithdrawMoney.Execute(...) method in the features folder. For consistency, the logic should be the same as the TransferMoney.Execute(...) method i.e. notifications for low funds and exceptions where the operation is not possible. 

As part of this process however, you should look to refactor some of the code in the TransferMoney.Execute(...) method into the domain models, and make these models less susceptible to misuse. We're looking to make our domain models rich in behaviour and much more than just plain old objects, however we don't want any data persistance operations (i.e. data access repositories) to bleed into our domain. This should simplify the task of implementing WidthdrawMoney.Execute(...).

## Guidlines

* You should spend no more than 1 hour on this task, although there is no time limit
* You should fork or copy this repository into your own public repository (Gihub, BitBucket etc.) before you do your work
* Your solution must compile and run first time
* You should not alter the notification service or the the account repository interfaces
* You may add unit/integration tests using a test framework (and/or mocking framework) of your choice
* You may edit this README.md if you want to give more details around your work (e.g. why you have done something a particular way, or anything else you would look to do but didn't have time)

Once you have completed your work, send us a link to your public repository.

Good luck!

## Comments from PF

* Refactored the transferMoney and withdrawMoney features to use new methods (TransferFrom and Withdraw) on the Account Domain model.
* Made balance a readonly property. Should only be able to adjust the balance of an account by paying money in or taking money out (?)
* Could make balance a computed property PaidIn + Withdrawn.  
* Should also make PaidIn and Withdrawn readonly properties.
* ~~Haven't done the above three changes as would mean I have to introduce a constructor to set the values of the accounts being used for testing and I ran out of time.~~
* The above was bugging me so I went back and did it anyway. PaidIn and Withdrawn are now readonly prperties and balance is now a computed property
