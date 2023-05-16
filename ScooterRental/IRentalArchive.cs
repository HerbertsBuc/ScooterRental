using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScooterRental
{
    public interface IRentalArchive
    {
        void AddRent(string id, decimal pricePerMinute, DateTime rentStart);

        RentedScooter EndRent(string id, DateTime rentEnd);

        IList<RentedScooter> GetRentedScootersRecords();
    }
}
