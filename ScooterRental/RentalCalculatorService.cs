using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScooterRental
{
    public class RentalCalculatorService : IRentalCalculator
    {
        public decimal CalculateIncome(IList<RentedScooter> rentedScooters)
        {
            decimal sum = 0;

            foreach (RentedScooter x in rentedScooters)
            {
                sum += CalculateRent(x);
            }

            return sum;
        }

        public decimal CalculateRent(RentedScooter rent)
        {
            DateTime end = GetEndTime(rent);

            if (rent.RentStarted.Day == end.Day)
            {
                if (GetMinutesRented(rent) * rent.PricePerMinute >= 20)
                {
                    return 20.00m;
                }

                return GetMinutesRented(rent) * rent.PricePerMinute;
            }

            return GetPayForStartDay(rent) + GetPayForEndDay(rent) + GetPayForMiddleDays(rent);
        }

        public int GetMinutesRented(RentedScooter rent)
        {
            TimeSpan span;

            if (rent.RentCompleted == null)
            {
                span = DateTime.UtcNow.Subtract(rent.RentStarted);
                return (int)span.TotalMinutes;
            }

            var end = (DateTime)rent.RentCompleted;
            span = end.Subtract(rent.RentStarted);

            return (int)span.TotalMinutes;
        }

        public decimal GetPayForStartDay(RentedScooter rent)
        {
            DateTime startDayEnd = new DateTime(rent.RentStarted.Year, rent.RentStarted.Month, rent.RentStarted.Day, 23, 59, 59);
            TimeSpan startDayTime = startDayEnd.Subtract(rent.RentStarted);
            int startDayMinutes = (int)startDayTime.TotalMinutes;

            if (startDayMinutes * rent.StartPricePerMinute >= 20)
            {
                return 20m;
            }

            return startDayMinutes * rent.PricePerMinute;
        }

        public decimal GetPayForEndDay(RentedScooter rent)
        {
            DateTime end = GetEndTime(rent);
            DateTime endDayStart = new DateTime(end.Year, end.Month, end.Day, 0, 0, 0);
            TimeSpan endDayTime = end.Subtract(endDayStart);
            int endDayMinutes = (int)endDayTime.TotalMinutes;

            if (endDayMinutes * rent.PricePerMinute >= 20)
            {
                return 20.00m;
            }

            return endDayMinutes * rent.PricePerMinute;
        }

        public decimal GetPayForMiddleDays(RentedScooter rent)
        {
            DateTime end = GetEndTime(rent);
            DateTime startDayEnd = new DateTime(rent.RentStarted.Year, rent.RentStarted.Month, rent.RentStarted.Day, 23, 59, 59);
            DateTime endDayStart = new DateTime(end.Year, end.Month, end.Day, 0, 0, 0);
            TimeSpan fullDays = endDayStart.Subtract(startDayEnd);
            int fullDaysInBetween = (int)Math.Round(fullDays.TotalDays);

            return fullDaysInBetween * 20.00m;
        }

        public DateTime GetEndTime(RentedScooter rent)
        {
            if (rent.RentCompleted == null)
            {
                return DateTime.UtcNow;
            }

            return (DateTime)rent.RentCompleted;
        }
    }
}
