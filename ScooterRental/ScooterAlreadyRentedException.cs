using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScooterRental
{
    public class ScooterAlreadyRentedException : Exception
    {
        public ScooterAlreadyRentedException() : base("Scooter not available at this moment. It is already being rented right now.")
        {

        }
    }
}
