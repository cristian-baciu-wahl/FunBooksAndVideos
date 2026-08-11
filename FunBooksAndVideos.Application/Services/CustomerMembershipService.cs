using FunBooksAndVideos.Application.Interfaces;
using FunBooksAndVideos.Domain;
using System.Collections.Concurrent;

namespace FunBooksAndVideos.Application.Services
{
    public class CustomerMembershipService : ICustomerMembershipService
    {
        private readonly ConcurrentDictionary<int, Customer> _customers;

        public CustomerMembershipService()
        {
            _customers = new ConcurrentDictionary<int, Customer>();
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            var customer = new Customer(4567890, "John Doe", "john.doe@example.com");
            _customers.TryAdd(customer.Id, customer);
        }

        public void ActivateMembership(int customerId, MembershipType membershipType)
        {
            if (!_customers.TryGetValue(customerId, out var customer))
            {
                throw new ArgumentException($"Customer with ID {customerId} not found");
            }

            if (!customer.IsActive)
            {
                throw new InvalidOperationException($"Customer {customerId} is not active");
            }

            // Check if membership already exists and deactivate it if needed
            var existingMembership = customer.Memberships.FirstOrDefault(m => m.Type == membershipType);
            if (existingMembership != null)
            {
                existingMembership.IsActive = true;
                existingMembership.ActivationDate = DateTime.UtcNow;
            }
            else
            {
                customer.Memberships.Add(new Membership(membershipType));
            }

            // For Premium membership, activate both Book and Video clubs
            if (membershipType == MembershipType.Premium)
            {
                ActivateMembership(customerId, MembershipType.BookClub);
                ActivateMembership(customerId, MembershipType.VideoClub);
            }
        }

        public bool HasActiveMembership(int customerId, MembershipType membershipType)
        {
            if (!_customers.TryGetValue(customerId, out var customer))
                return false;

            return customer.Memberships.Any(m => m.Type == membershipType && m.IsActive);
        }

        public bool CustomerExists(int customerId)
        {
            return _customers.ContainsKey(customerId);
        }

        public Customer? GetCustomer(int customerId)
        {
            _customers.TryGetValue(customerId, out var customer);
            return customer;
        }
    }
}