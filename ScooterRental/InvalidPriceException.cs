using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScooterRental
{
    public class InvalidPriceException : Exception
    {
        public InvalidPriceException() : base("Provided price not valid. Only positive price allowed.")
        {

        }
    }
}
