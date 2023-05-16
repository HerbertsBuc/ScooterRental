using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScooterRental
{
    public class ScooterService : IScooterService
    {
        static void Main() { }

        private readonly List<Scooter> _scooters;

        public ScooterService(List<Scooter> scooters)
        {
            _scooters = scooters;
        }
        public void AddScooter(string id, decimal pricePerMinute)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new ScooterIdNotProvidedException();
            }

            if (pricePerMinute <= 0)
            {
                throw new InvalidPriceException();
            }

            var scooter = new Scooter(id, pricePerMinute);
            _scooters.Add(scooter);
        }

        public Scooter GetScooterById(string scooterId)
        {
            if (String.IsNullOrEmpty(scooterId))
            {
                throw new ScooterIdNotProvidedException();
            }

            var scooter = _scooters.SingleOrDefault(x => x.Id == scooterId);
            if (scooter != null)
            {
                return scooter;
            }
            
            throw new ScooterNotFoundException();
            
        }

        public IList<Scooter> GetScooters()
        {
            return _scooters.ToList();
        }

        public void RemoveScooter(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new ScooterIdNotProvidedException();
            }

            var scooter = _scooters.SingleOrDefault(x => x.Id == id);
            if (scooter != null)
            {
                _scooters.Remove(scooter);
            }
            else
            {
                throw new ScooterIdNotProvidedException();
            }
        }
    }
}
