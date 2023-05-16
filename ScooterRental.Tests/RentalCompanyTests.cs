using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;

namespace ScooterRental.Tests
{
    public class RentalCompanyTests
    {
        private IRentalCompany _company;
        private IRentalCalculator _calculator;
        private IRentalArchive _archive;
        private const string CompanyName = "test";
        private AutoMocker _mocker;

        [SetUp]
        public void Setup()
        {
            _mocker = new AutoMocker();
            _company = new RentalCompany(CompanyName, _mocker.GetMock<IScooterService>().Object, _mocker.GetMock<IRentalCalculator>().Object, _mocker.GetMock<IRentalArchive>().Object, _mocker.GetMock<RentalCalculatorService>().Object);
        }

        [Test]
        public void CreateRentalCompany_TestAsNameProvided_NameShouldBeTest()
        {
            _company.Name.Should().Be(CompanyName);
        }

        [Test]
        public void StartRent_ValidIdProvided_ScooterIsRented()
        {
            var scooter = new Scooter("1", 1m);
            var scooterServiceMock = _mocker.GetMock<IScooterService>();
            scooterServiceMock.Setup(x => x.GetScooterById("1")).Returns(scooter);

            _company.StartRent("1");

            scooter.IsRented.Should().BeTrue();
        }

        [Test]
        public void EndRent_ValidIdProvided_ScooterIsNotRented()
        {
            var scooter = new Scooter("1", 1m);
            var scooterServiceMock = _mocker.GetMock<IScooterService>();
            scooterServiceMock.Setup(x => x.GetScooterById("1")).Returns(scooter);
            scooter.IsRented = true;
            _company.EndRent("1");

            scooter.IsRented.Should().BeFalse();
        }

        [Test]
        public void StartRent_IdOfRentedScooterProvided_ThrowsScooterAlreadyRentedException()
        {
            Action act = () =>
            {
                var scooter = new Scooter("1", 1m);
                var scooterServiceMock = _mocker.GetMock<IScooterService>();
                scooterServiceMock.Setup(x => x.GetScooterById("1")).Returns(scooter);

                _company.StartRent("1");
                _company.StartRent("1");
            };

            act.Should().Throw<ScooterAlreadyRentedException>();
        }

        [Test]
        public void EndRent_IdOfUnrentedScooterProvided_ThrowsScooterNotRentedException()
        {
            Action act = () =>
            {
                var scooter = new Scooter("1", 1m);
                var scooterServiceMock = _mocker.GetMock<IScooterService>();
                scooterServiceMock.Setup(x => x.GetScooterById("1")).Returns(scooter);
                scooter.IsRented = false;

                _company.EndRent("1");
            };

            act.Should().Throw<ScooterNotRentedException>();
        }

        [Test]
        public void CalculateIncome_NoYearInputAndIncludeIncompleteRents_ReturnsAllIncome()
        {
            IList<RentedScooter> rentedScooters = new List<RentedScooter>();

            var rentalArchiveMock = _mocker.GetMock<IRentalArchive>();
            rentalArchiveMock.Setup(x => x.GetRentedScootersRecords()).Returns(rentedScooters);
            var rentalCalculatorMock = _mocker.GetMock<IRentalCalculator>();
            rentalCalculatorMock.Setup(x => x.CalculateIncome(rentedScooters)).Returns(26);

            decimal income = _company.CalculateIncome(null, true);

            income.Should().Be(26);
        }

        [Test]
        public void CalculateIncome_YearInputAndIncludeIncompleteRents_ReturnsAllIncomeForYear()
        {
            var rentedScooter = new RentedScooter("1", 1m, DateTime.UtcNow);
            var rentedScooter2 = new RentedScooter("2", 1m, new DateTime(2017, 11, 20, 23, 48, 13));

            IList<RentedScooter> rentedScooters = new List<RentedScooter>();
            rentedScooters.Add(rentedScooter);
            rentedScooters.Add(rentedScooter2);
            rentedScooter.RentCompleted = DateTime.UtcNow.AddMinutes(15);
            rentedScooter2.RentCompleted = new DateTime(2017, 11, 20, 23, 59, 15);

            var rentalArchiveMock = _mocker.GetMock<IRentalArchive>();
            rentalArchiveMock.Setup(x => x.GetRentedScootersRecords()).Returns(rentedScooters);
            var rentalCalculatorMock = _mocker.GetMock<IRentalCalculator>();
            rentalCalculatorMock.Setup(x => x.CalculateRent(rentedScooter)).Returns(15);

            decimal income = _company.CalculateIncome(2023, true);

            income.Should().Be(15);
        }

        [Test]
        public void CalculateIncome_NoYearInputAndNotIncludeIncompleteRents_ReturnsAllIncomeForCompletedRents()
        {
            var rentedScooter = new RentedScooter("1", 1m, DateTime.UtcNow);
            var rentedScooter2 = new RentedScooter("2", 1m, new DateTime(2017, 11, 20, 23, 48, 13));
            IList<RentedScooter> rentedScooters = new List<RentedScooter>();
            rentedScooters.Add(rentedScooter);
            rentedScooters.Add(rentedScooter2);
            rentedScooter2.RentCompleted = new DateTime(2017, 11, 20, 23, 59, 15);

            var rentalArchiveMock = _mocker.GetMock<IRentalArchive>();
            rentalArchiveMock.Setup(x => x.GetRentedScootersRecords()).Returns(rentedScooters);
            var rentalCalculatorMock = _mocker.GetMock<IRentalCalculator>();
            rentalCalculatorMock.Setup(x => x.CalculateRent(rentedScooter2)).Returns(11);
            rentalCalculatorMock.Setup(x => x.CalculateIncome(rentedScooters)).Returns(11);

            decimal income = _company.CalculateIncome(null, false);

            income.Should().Be(11);
        }

        [Test]
        public void CalculateIncome_YearInputAndNotIncludeIncompleteRents_ReturnsAllIncomeForYearForCompletedRents()
        {
            var rentedScooter = new RentedScooter("1", 1m, DateTime.UtcNow);
            var rentedScooter2 = new RentedScooter("2", 1m, new DateTime(2023, 11, 20, 23, 48, 13));
            IList<RentedScooter> rentedScooters = new List<RentedScooter>();
            rentedScooters.Add(rentedScooter);
            rentedScooters.Add(rentedScooter2);
            rentedScooter2.RentCompleted = new DateTime(2023, 11, 20, 23, 59, 15);

            var rentalArchiveMock = _mocker.GetMock<IRentalArchive>();
            rentalArchiveMock.Setup(x => x.GetRentedScootersRecords()).Returns(rentedScooters);
            var rentalCalculatorMock = _mocker.GetMock<IRentalCalculator>();
            rentalCalculatorMock.Setup(x => x.CalculateRent(rentedScooter2)).Returns(11);

            decimal income = _company.CalculateIncome(2023, false);

            income.Should().Be(11);
        }

        [Test]
        public void CalculateIncome_YearGreaterThanCurrent_ThrowsInvalidYearException()
        {
            Action act = () =>
            {
                IList<RentedScooter> rentedScooters = new List<RentedScooter>();
                var rentalArchiveMock = _mocker.GetMock<IRentalArchive>();
                rentalArchiveMock.Setup(x => x.GetRentedScootersRecords()).Returns(rentedScooters);
                var rentalCalculatorMock = _mocker.GetMock<IRentalCalculator>();

                _company.CalculateIncome(DateTime.UtcNow.Year + 1, true);
            };

            act.Should().Throw<InvalidYearException>();
        }

        [Test]
        public void CalculateIncome_NegativeYearInput_ThrowsInvalidYearException()
        {
            Action act = () =>
            {
                IList<RentedScooter> rentedScooters = new List<RentedScooter>();
                var rentalArchiveMock = _mocker.GetMock<IRentalArchive>();
                rentalArchiveMock.Setup(x => x.GetRentedScootersRecords()).Returns(rentedScooters);
                var rentalCalculatorMock = _mocker.GetMock<IRentalCalculator>();

                _company.CalculateIncome(-1, true);
            };

            act.Should().Throw<InvalidYearException>();
        }

        [Test]
        public void CalculateIncome_ZeroYearInput_ThrowsInvalidYearException()
        {
            Action act = () =>
            {
                IList<RentedScooter> rentedScooters = new List<RentedScooter>();
                var rentalArchiveMock = _mocker.GetMock<IRentalArchive>();
                rentalArchiveMock.Setup(x => x.GetRentedScootersRecords()).Returns(rentedScooters);
                var rentalCalculatorMock = _mocker.GetMock<IRentalCalculator>();

                _company.CalculateIncome(0, true);
            };

            act.Should().Throw<InvalidYearException>();
        }
    }
}
