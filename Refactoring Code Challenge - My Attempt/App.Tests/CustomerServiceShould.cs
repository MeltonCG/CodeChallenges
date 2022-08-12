using AutoFixture;
using Moq;
using NUnit.Framework;
using System;

namespace App.Tests
{
    [TestFixture]
    public sealed class CustomerServiceShould
    {
        private static Fixture fixture;
        private static Mock<ICompanyRepository> companyRepo;
        private static Mock<ICustomerCreditService> creditService;
        private static Mock<ICustomerDataAccessWrapper> customerDataAccessWrapper;

        public CustomerServiceShould()
        {
            fixture = new Fixture();
            companyRepo = new Mock<ICompanyRepository>();
            creditService = new Mock<ICustomerCreditService>();
            customerDataAccessWrapper = new Mock<ICustomerDataAccessWrapper>();
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("Joe", true)]
        public void AddCustomer_FirstName(string firstName, bool expectedResult)
        {
            // Arrange
            string surname = "Bloggs";
            string email = "Joe.Bloggs@adomain.com";
            DateTime dateOfBirth = new DateTime(1980, 3, 27);
            int companyId = 123;
            var company = fixture.Create<Company>();
            
            MockSetupForBasicValidation();

            // Act
            var instance = new CustomerService(companyRepo.Object, creditService.Object, customerDataAccessWrapper.Object);
            var result = instance.AddCustomer(firstName, surname, email, dateOfBirth, companyId);


            // Assert
            Assert.AreEqual(expectedResult, result);
        }
              

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("Bloggs", true)]
        public void AddCustomer_Surname(string surname, bool expectedResult)
        {
            // Arrange
            string firstName = "Joe";
            string email = "Joe.Bloggs@adomain.com";
            DateTime dateOfBirth = new DateTime(1980, 3, 27);
            int companyId = 123;


            MockSetupForBasicValidation();

            // Act
            var instance = new CustomerService(companyRepo.Object, creditService.Object, customerDataAccessWrapper.Object);
            var result = instance.AddCustomer(firstName, surname, email, dateOfBirth, companyId);


            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("Joe.Bloggs invalid email", false)]
        [TestCase("Joe.Bloggs@adomain.com", true)]
        public void AddCustomer_Email(string email, bool expectedResult)
        {
            // Arrange
            string firstName = "Joe";
            string surname = "Bloggs";
            DateTime dateOfBirth = new DateTime(1980, 3, 27);
            int companyId = 123;


            MockSetupForBasicValidation();

            // Act
            var instance = new CustomerService(companyRepo.Object, creditService.Object, customerDataAccessWrapper.Object);
            var result = instance.AddCustomer(firstName, surname, email, dateOfBirth, companyId);


            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase(16, false)]
        [TestCase(18, false)]
        [TestCase(20, false)]
        [TestCase(20, false)]
        [TestCase(21, true)]
        [TestCase(25, true)]
        public void AddCustomer_AgeCheck(int customerAge, bool expectedResult)
        {
            // Arrange
            string firstName = "Joe";
            string surname = "Bloggs";
            string email = "Joe.Bloggs@adomain.com";
            DateTime dateOfBirth = DateTime.Now.AddYears(customerAge * -1).Date;
            int companyId = 123;


            MockSetupForBasicValidation();

            // Act
            var instance = new CustomerService(companyRepo.Object, creditService.Object, customerDataAccessWrapper.Object);
            var result = instance.AddCustomer(firstName, surname, email, dateOfBirth, companyId);


            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        // Ran out of time at this point.
        // Going forward I would have created unit tests for VeryImportantClient & ImportantClient
        // checking that the combinations of HasCreditLimit & CreditLimit return the right outcome
        // e.g. VeryImportantClient has credit limit < 500 but still passes as HasCreditLimit = false

        private static void MockSetupForBasicValidation()
        {
            companyRepo.Setup(s => s.GetById(It.IsAny<int>()))
                            .Returns(fixture.Create<Company>());
            creditService.Setup(s => s.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()).Result)
                .Returns(1000);
        }
    }
}
