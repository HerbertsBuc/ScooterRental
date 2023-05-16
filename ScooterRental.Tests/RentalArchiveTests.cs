using FluentAssertions;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScooterRental.Tests
{
    public class RentalArchiveTests
    {

        private RentalArchive _archive;
        private AutoMocker _mocker;
        private IList<RentedScooter> _rentedScooters;


        [SetUp]
        public void Setup()
        {
            _mocker = new AutoMocker();
            _archive = new RentalArchive(_mocker.GetMock<IList<RentedScooter>>().Object);
            _rentedScooters = new List<RentedScooter>();
        }

        [Test]
        public void AddRent_AllValuesProvided_AddsRentToArchive()
        {
            _archive = new RentalArchive(_rentedScooters);
            _archive.AddRent("1", 1m, DateTime.UtcNow);
            _archive.GetRentedScootersRecords().Count.Should().Be(1);
        }

        [Test]
        public void EndRent_AllValuesProvided_SetsEndTime()
        {
            _archive = new RentalArchive(_rentedScooters);
            _archive.AddRent("1", 1m, new DateTime(2023, 4, 3, 14, 15, 02));
            DateTime endOfRent = new DateTime(2023, 4, 3, 14, 28, 02);
            _archive.EndRent("1", endOfRent);
            _rentedScooters[0].RentCompleted.Should().Be(endOfRent);

        }
    }
}
