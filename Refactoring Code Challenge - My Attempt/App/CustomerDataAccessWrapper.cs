namespace App
{
    public interface ICustomerDataAccessWrapper
    {
        void AddCustomer(Customer customer);
    }

    public class CustomerDataAccessWrapper: ICustomerDataAccessWrapper
    {
        public void AddCustomer(Customer customer)
        {
            CustomerDataAccess.AddCustomer(customer);
        }
    }
}
