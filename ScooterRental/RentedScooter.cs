using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScooterRental
{
    public class RentedScooter
    {
        public string Id { get; }
        public decimal PricePerMinute { get; }
        public decimal StartPricePerMinute { get; set; }
        public DateTime RentStarted { get; }
        public DateTime? RentCompleted { get; set; }

        public RentedScooter(string id, decimal pricePerMinute, DateTime rentStarted)
        {
            Id = id;
            PricePerMinute = pricePerMinute;
            RentStarted = rentStarted;
        }
    }
}
