using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScooterRental
{
    public class ScooterNotRentedException : Exception
    {
        public ScooterNotRentedException() :base("Can not end rent. Scooter is currently not being rented.")
        {

        }
    }
}
