using FluentAssertions;

namespace ScooterRental.Tests
{
    public class ScooterServiceTests
    {
        private IScooterService _scooterService;
        private List<Scooter> _scooters;

        [SetUp]
        public void Setup()
        {
            _scooters = new List<Scooter>();
            _scooterService = new ScooterService(_scooters);
        }

        [Test]
        public void AddScooter_AddValidScooter_ScooterAdded()
        {
            _scooterService.AddScooter("1", 1m);
            _scooters.Count.Should().Be(1);
        }

        [Test]
        public void AddScooter_AddScooterWithoutId_ThrowsScooterIdNotProvidedException()
        {
            Action act = () =>
            {
                _scooterService.AddScooter("", 0.1m);
            };

            act.Should().Throw<ScooterIdNotProvidedException>();
        }

        [Test]
        public void AddScooter_AddScooterWithNullId_ThrowsScooterIdNotProvidedException()
        {
            Action act = () =>
            {
                _scooterService.AddScooter(null, 0.1m);
            };

            act.Should().Throw<ScooterIdNotProvidedException>();
        }

        [Test]
        public void AddScooter_AddScooterWithNegativePrice_ThrowsScooterIdNotProvidedException()
        {
            Action act = () =>
            {
                _scooterService.AddScooter("1", -0.1m);
            };

            act.Should().Throw<InvalidPriceException>();
        }

        [Test]
        public void RemoveScooter_ValidIdProvided_ScooterRemoved()
        {
            _scooters.Add(new Scooter("1", 0.1m));
            _scooterService.RemoveScooter("1");
            _scooters.Any(x => x.Id == "1").Should().BeFalse();
        }

        [Test]
        public void RemoveScooter_NullIdProvided_ThrowsScooterIdNotProvidedException()
        {
            Action act = () =>
            {
                _scooterService.RemoveScooter(null);
            };

            act.Should().Throw<ScooterIdNotProvidedException>();
        }

        [Test]
        public void RemoveScooter_EmptyIdProvided_ThrowsScooterIdNotProvidedException()
        {
            Action act = () =>
            {
                _scooterService.RemoveScooter("");
            };

            act.Should().Throw<ScooterIdNotProvidedException>();
        }

        [Test]
        public void RemoveScooter_InvalidIdProvided_ThrowsScooterNotFoundException()
        {
            Action act = () =>
            {
                _scooterService.RemoveScooter("3");
            };

            act.Should().Throw<ScooterIdNotProvidedException>();
        }

        [Test]
        public void GetScooters_ReturnsAllScooters()
        {
            _scooters.Add(new Scooter("1", 1m));

            var scooters = _scooterService.GetScooters();
            scooters.Count.Should().Be(1);
        }

        [Test]
        public void GetScooterById_ValidIdProvided_ReturnsScooter()
        {
            var scooter = new Scooter("1", 2m);
            _scooters.Add(scooter);

            _scooterService.GetScooterById(scooter.Id).Should().Be(scooter);
        }

        [Test]
        public void GetScooterById_NullIdProvided_ThrowsScooterIdNotProvidedException()
        {
            Action act = () =>
            {
                _scooterService.RemoveScooter(null);
            };

            act.Should().Throw<ScooterIdNotProvidedException>();
        }

        [Test]
        public void GetScooterById_EmptyIdProvided_ThrowsScooterIdNotProvidedException()
        {
            Action act = () =>
            {
                _scooterService.GetScooterById("");
            };

            act.Should().Throw<ScooterIdNotProvidedException>();
        }

        [Test]
        public void GetScooterById_InvalidIdProvided_ThrowsScooterNotFoundException()
        {
            Action act = () =>
            {
                _scooterService.GetScooterById("3");
            };

            act.Should().Throw<ScooterNotFoundException>();
        }
    }
}