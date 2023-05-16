using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScooterRental
{
    public interface IRentalCalculator
    {
        decimal CalculateRent(RentedScooter rent);

        decimal CalculateIncome(IList<RentedScooter> rentedScooters);
    }
}
