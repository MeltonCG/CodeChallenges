using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App
{
    public class CustomerService
    {
        private readonly ICompanyRepository companyRepository;
        private readonly ICustomerCreditService customerCreditService;
        private readonly ICustomerDataAccessWrapper customerDataAccess;

        public CustomerService()
        {
            this.companyRepository = new CompanyRepository();
            this.customerCreditService = new CustomerCreditServiceClient();
            this.customerDataAccess = new CustomerDataAccessWrapper();
        }

        public CustomerService(ICompanyRepository companyRepository,
                               ICustomerCreditService customerCreditService, 
                               ICustomerDataAccessWrapper customerDataAccess)
        {
            this.companyRepository = companyRepository;
            this.customerCreditService = customerCreditService;
            this.customerDataAccess = customerDataAccess;
        }

        public bool AddCustomer(string firstName, string surname, string email, DateTime dateOfBirth, int companyId)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(surname))
            {
                return false;
            }

            // Given more time a method to validate an email address rather than just checking it contains @ and . at any point
            // in the string
            if (string.IsNullOrEmpty(email) || !email.Contains("@") || !email.Contains("."))
            {
                return false;
            }

            if (dateOfBirth.Date > DateTime.Now.AddYears(-21).Date)
            {
                return false;
            }

            var company = companyRepository.GetById(companyId);

            var customer = new Customer
                               {
                                   Company = company,
                                   DateOfBirth = dateOfBirth,
                                   EmailAddress = email,
                                   Firstname = firstName,
                                   Surname = surname
                               };

            if (company.Name == "VeryImportantClient")
            {
                // Skip credit check
                customer.HasCreditLimit = false;
            }
            else if (company.Name == "ImportantClient")
            {
                // Do credit check and double credit limit
                customer.HasCreditLimit = true;
                var creditLimit = customerCreditService.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth).Result;
                creditLimit *= 2;
                customer.CreditLimit = creditLimit;
            }
            else
            {
                // Do credit check
                customer.HasCreditLimit = true;
                var creditLimit = customerCreditService.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth).Result;
                customer.CreditLimit = creditLimit;
            }

            if (customer.HasCreditLimit && customer.CreditLimit < 500)
            {
                return false;
            }

            customerDataAccess.AddCustomer(customer);

            return true;
        }
    }
}
