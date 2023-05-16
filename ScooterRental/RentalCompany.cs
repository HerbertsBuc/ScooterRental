using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScooterRental
{
    public class RentalCompany : IRentalCompany
    {
        private readonly IScooterService _scooterService;
        private readonly IRentalCalculator _rentalCalculator;
        private readonly IRentalArchive _rentedScooters;
        private readonly RentalCalculatorService _rentalCalculatorService;

        public string Name { get; }

        public RentalCompany(string name, IScooterService scooterService, IRentalCalculator rentalCalculator, IRentalArchive rentedScooters, RentalCalculatorService rentalCalculatorService)
        {
            Name = name;
            _scooterService = scooterService;
            _rentalCalculator = rentalCalculator;
            _rentedScooters = rentedScooters;
            _rentalCalculatorService = rentalCalculatorService;
        }

        public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
        {
            if (year != null && (year <= 0 || year > DateTime.UtcNow.Year))
            {
                throw new InvalidYearException();
            }

            if (year == null && includeNotCompletedRentals == true)
            {
                return _rentalCalculator.CalculateIncome(_rentedScooters.GetRentedScootersRecords());
            }

            if (year == null && includeNotCompletedRentals == false)
            {
                return CalculateIncomeCompletedRents();
            }

            if (year != null && includeNotCompletedRentals == false)
            {
                return CalculateYearIncomeCompletedRents(year);
            }

            return CalculateYearIncomeAllRents(year);
        }

        private decimal CalculateIncomeCompletedRents()
        {
            decimal sum = 0;

            foreach (RentedScooter x in _rentedScooters.GetRentedScootersRecords())
            {
                if (x.RentCompleted != null)
                {
                    sum += _rentalCalculator.CalculateRent(x);
                }
            }
            return sum;
        }

        private decimal CalculateYearIncomeCompletedRents(int? year)
        {
            decimal sum = 0;

            foreach (RentedScooter x in _rentedScooters.GetRentedScootersRecords())
            {
                if (x.RentCompleted != null)
                {
                    DateTime endTime = _rentalCalculatorService.GetEndTime(x);

                    if (endTime.Year == year)
                    {
                        sum += _rentalCalculator.CalculateRent(x);
                    }
                }
            }
            return sum;
        }

        private decimal CalculateYearIncomeAllRents(int? year)
        {
            decimal sum = 0;

            foreach (RentedScooter x in _rentedScooters.GetRentedScootersRecords())
            {
                DateTime endTime = _rentalCalculatorService.GetEndTime(x);

                if (endTime.Year == year)
                {
                    sum += _rentalCalculator.CalculateRent(x);
                }
            }

            return sum;
        }

        public decimal EndRent(string id)
        {
            var scooter = _scooterService.GetScooterById(id);

            if (scooter.IsRented == false)
            {
                throw new ScooterNotRentedException();
            }

            var rental = _rentedScooters.EndRent(scooter.Id, DateTime.UtcNow);

            scooter.IsRented = false;
            return _rentalCalculator.CalculateRent(rental);
        }

        public void StartRent(string id)
        {
            var scooter = _scooterService.GetScooterById(id);

            if (scooter.IsRented == true)
            {
                throw new ScooterAlreadyRentedException();
            }

            _rentedScooters.AddRent(scooter.Id, scooter.PricePerMinute, DateTime.UtcNow);
            scooter.IsRented = true;
        }
    }
}
