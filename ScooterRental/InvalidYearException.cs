using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScooterRental
{
    public class InvalidYearException : Exception
    {
        public InvalidYearException() :base("Invalid year input. Year must be positive and not greater than current year.")
        {

        }
    }
}
