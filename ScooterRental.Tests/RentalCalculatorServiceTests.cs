using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace ScooterRental.Tests
{
    public class RentalCalculatorServiceTests
    {
        private RentalCalculatorService _rentalCalculator;
        private RentedScooter _rentedScooter;
        private IList<RentedScooter> _rentedScooters;

        [SetUp]

        public void Setup()
        {
            _rentedScooter = new RentedScooter("1", 1m, DateTime.UtcNow);
            _rentedScooters = new List<RentedScooter>();
            _rentalCalculator = new RentalCalculatorService();
        }

        [Test]

        public void GetEndTime_ScooterIsReturned_EndTimeIsTimeOfReturn()
        {
            _rentedScooter.RentCompleted = DateTime.UtcNow.AddMinutes(15);
            TimeSpan timeRented = _rentalCalculator.GetEndTime(_rentedScooter).Subtract(_rentedScooter.RentStarted);
            timeRented.Minutes.Should().Be(15);
        }

        [Test]
        public void GetEndTime_NotReturnedYet_ReturnsCurrentTime()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 1m, DateTime.UtcNow.AddMinutes(-20));
            TimeSpan timeRented = _rentalCalculator.GetEndTime(scooterNotReturned)
                .Subtract(scooterNotReturned.RentStarted);
            timeRented.Minutes.Should().Be(20);
        }

        [Test]
        public void GetMinutesRented_ProvidedReturnTime_ReturnsMinutesRented()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 1m, DateTime.UtcNow.AddMinutes(-200));
            scooterNotReturned.RentCompleted = DateTime.UtcNow.AddMinutes(-10);
            _rentalCalculator.GetMinutesRented(scooterNotReturned).Should().Be(190);
        }

        [Test]
        public void GetMinutesRented_ScooterNotReturnedYet_ReturnsMinutesRented()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 1m, DateTime.UtcNow.AddMinutes(-200));
            _rentalCalculator.GetMinutesRented(scooterNotReturned).Should().Be(200);
        }

        [Test]
        public void GetPayForStartDay_ProvidedReturnTime_ReturnsPayForFirstDay()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 1m, new DateTime(2023, 4, 3, 23, 48, 02));
            scooterNotReturned.RentCompleted = new DateTime(2023, 4, 4, 17, 48, 02);
            _rentalCalculator.GetPayForStartDay(scooterNotReturned).Should().Be(11);
        }

        [Test]
        public void GetPayForStartDay_NotProvidedReturnTime_ReturnsPayForFirstDay()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 1m, new DateTime(2023, 4, 2, 23, 47, 02));
            _rentalCalculator.GetPayForStartDay(scooterNotReturned).Should().Be(12);
        }

        [Test]
        public void GetPayForStartDay_MinutesExceedDailyPay_ReturnsFullDayFare()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 1m, new DateTime(2023, 4, 3, 13, 48, 02));
            scooterNotReturned.RentCompleted = new DateTime(2023, 4, 4, 17, 48, 02);
            scooterNotReturned.StartPricePerMinute = scooterNotReturned.PricePerMinute;
            _rentalCalculator.GetPayForStartDay(scooterNotReturned).Should().Be(20);
        }

        [Test]
        public void GetPayForEndDay_ProvidedReturnTime_ReturnsPayForLastDay()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 1m, new DateTime(2023, 4, 3, 23, 48, 02));
            scooterNotReturned.RentCompleted = new DateTime(2023, 4, 4, 0, 13, 02);
            _rentalCalculator.GetPayForEndDay(scooterNotReturned).Should().Be(13);
        }

        [Test]
        public void GetPayForEndDay_NotProvidedReturnTime_ReturnsPayForLastDay()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 0.01m, DateTime.UtcNow.AddDays(-2));
            TimeSpan minutesToday = DateTime.UtcNow.Subtract(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0));
            decimal totalPay = (decimal)minutesToday.TotalMinutes * scooterNotReturned.PricePerMinute;
            Math.Round(_rentalCalculator.GetPayForEndDay(scooterNotReturned), 1).Should().Be(Math.Round(totalPay, 1));
        }

        [Test]
        public void GetPayForEndDay_MinutesExceedDailyPay_ReturnsFullDayFare()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 1m, new DateTime(2023, 4, 1, 13, 48, 02));
            scooterNotReturned.RentCompleted = new DateTime(2023, 4, 4, 13, 48, 02);
            scooterNotReturned.StartPricePerMinute = scooterNotReturned.PricePerMinute;
            _rentalCalculator.GetPayForEndDay(scooterNotReturned).Should().Be(20);
        }

        [Test]
        public void GetPayForMiddleDays_ProvidedReturnTime_ReturnsPayForMiddleDays()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 1m, new DateTime(2023, 4, 1, 23, 48, 02));
            scooterNotReturned.RentCompleted = new DateTime(2023, 4, 4, 0, 13, 02);
            _rentalCalculator.GetPayForMiddleDays(scooterNotReturned).Should().Be(40);
        }

        [Test]
        public void GetPayForMiddleDays_NotProvidedReturnTime_ReturnsPayForLastDay()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 1m, DateTime.UtcNow.AddDays(-5));
            _rentalCalculator.GetPayForMiddleDays(scooterNotReturned).Should().Be(80);
        }

        [Test]
        public void GetPayForMiddleDays_RentSpanWithoutMiddleDays_ReturnsPriceZero()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 1m, new DateTime(2023, 4, 1, 13, 48, 02));
            scooterNotReturned.RentCompleted = new DateTime(2023, 4, 2, 13, 48, 02);
            _rentalCalculator.GetPayForMiddleDays(scooterNotReturned).Should().Be(0);
        }

        [Test]
        public void CalculateRent_ProvidedLongRentTime_ReturnsFullRentAmount()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 1m, new DateTime(2023, 4, 1, 23, 48, 02));
            scooterNotReturned.RentCompleted = new DateTime(2023, 4, 4, 0, 13, 02);
            _rentalCalculator.CalculateRent(scooterNotReturned).Should().Be(64);
        }

        [Test]
        public void CalculateRent_ReturnOnSameDayNotFullTime_ReturnsLessThanTwenty()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 1m, new DateTime(2023, 4, 1, 23, 48, 02));
            scooterNotReturned.RentCompleted = new DateTime(2023, 4, 1, 23, 58, 14);
            _rentalCalculator.CalculateRent(scooterNotReturned).Should().Be(10);
        }

        [Test]
        public void CalculateRent_ReturnOnSameDayLongRent_ReturnsFullDayFare()
        {
            RentedScooter scooterNotReturned = new RentedScooter("1", 1m, new DateTime(2023, 4, 1, 13, 15, 02));
            scooterNotReturned.RentCompleted = new DateTime(2023, 4, 1, 23, 58, 14);
            _rentalCalculator.CalculateRent(scooterNotReturned).Should().Be(20);
        }

        [Test]
        public void CalculateIncome_ProvidedLisOfRents_ReturnsTotalIncome()
        {
            _rentedScooters.Add(new RentedScooter("1", 1m, new DateTime(2023, 4, 1, 23, 48, 02)));
            _rentedScooters.Add(new RentedScooter("2", 1m, new DateTime(2023, 4, 1, 23, 48, 02)));
            _rentedScooters[0].RentCompleted = new DateTime(2023, 4, 4, 0, 13, 02);
            _rentedScooters[1].RentCompleted = new DateTime(2023, 4, 1, 23, 58, 14);

            _rentalCalculator.CalculateIncome(_rentedScooters).Should().Be(74);

        }
    }
}
