using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScooterRental
{
    public class RentalArchive : IRentalArchive
    {
        private readonly IList<RentedScooter> _rentedScooters;

        public RentalArchive(IList<RentedScooter> rentedScooters)
        {
            _rentedScooters = rentedScooters;
        }

        public void AddRent(string id, decimal pricePerMinute, DateTime rentStart)
        {
            RentedScooter newRental = new RentedScooter(id, pricePerMinute, rentStart);
            newRental.StartPricePerMinute = pricePerMinute;
            _rentedScooters.Add(newRental);
        }

        public RentedScooter EndRent(string id, DateTime rentEnd)
        {
            var rental = _rentedScooters.SingleOrDefault(x => x.Id == id && !x.RentCompleted.HasValue);
            rental.RentCompleted = rentEnd;
            return rental;
        }

        public IList<RentedScooter> GetRentedScootersRecords()
        {
            return _rentedScooters;
        }
    }
}
