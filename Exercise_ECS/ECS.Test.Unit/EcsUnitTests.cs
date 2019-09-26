using System;
using NSubstitute;
using NUnit.Framework;


namespace ECS.Test.Unit
{
    [TestFixture]
    public class EcsUnitTests
    {
        // member variables to hold uut and fakes
        private ECS _uut;
        private IHeater _heater;
        private ITempSensor _tempSensor;
        private IWindow _window; 


        [SetUp]
        public void Setup()
        {
            // Create the fake stubs and mocks
            _heater = Substitute.For<IHeater>();
            _tempSensor = Substitute.For<ITempSensor>();
            _window = Substitute.For<IWindow>(); 

            _uut = new ECS(_tempSensor, _heater, _window, 5,25);
        }

        [Test]
        public void RunSelfTest_TempSensorFails_SelfTestFails()
        {
            _tempSensor.RunSelfTest().Returns(false);
            _heater.RunSelfTest().Returns(true);
            Assert.IsFalse(_uut.RunSelfTest());
        }

        [Test]
        public void Regulate_TempBelowThreshold_HeaterTurnedOn()
        {
            _tempSensor.GetTemp().Returns(_uut.LowerTemperatureThreshold - 10);
            _uut.Regulate();
            _heater.Received(1).TurnOn();
        }

        #region Threshold tests

        [Test]
        public void Thresholds_ValidUpperTemperatureThresholdSet_NoExceptionsThrown()
        {
            // Check that it doesn't throw
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.UpperTemperatureThreshold = 27; }, Throws.Nothing);
        }

        [Test]
        public void Thresholds_ValidLowerTemperatureThresholdSet_NoExceptionsThrown()
        {
            // Check that it doesn't throw 
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.LowerTemperatureThreshold = 5; }, Throws.Nothing);
        }

        [Test]
        public void Thresholds_UpperSetToLower_NoExceptionsThrown()
        {
            // Check that it doesn't throw when they are equal
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.UpperTemperatureThreshold = _uut.LowerTemperatureThreshold; }, Throws.Nothing);
        }

        [Test]
        public void Thresholds_LowerSetToUpper_NoExceptionsThrown()
        {
            // Check that it doesn't throw when they are equal
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.LowerTemperatureThreshold = _uut.UpperTemperatureThreshold; }, Throws.Nothing);
        }


        public void Thresholds_InvalidUpperTemperatureThresholdSet_ArgumentExceptionThrown()
        {
            // Check that it throws when upper is illegal
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.UpperTemperatureThreshold = 24; }, Throws.TypeOf<ArgumentException>());
        }

        public void Thresholds_InvalidLowerTemperatureThresholdSet_ArgumentExceptionThrown()
        {
            // Check that it throws when lower is illegal
            // First parameter is a lambda expression, implicitly acting
            Assert.That(() => { _uut.LowerTemperatureThreshold = 29; }, Throws.TypeOf<ArgumentException>());
        }

        #endregion

        #region Regulation tests

        #region T < Tlow

        [Test]
        public void Regulate_TempIsLow_HeaterIsTurnedOn()
        {
            // Setup stub with desired response
            _tempSensor.GetTemp().Returns(2);
            // Act
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            _heater.Received(1).TurnOn(); 
        }


        [Test]
        public void Regulate_TempIsLow_WindowIsClosed()
        {
            // Setup stub with desired response
            _tempSensor.GetTemp().Returns(2);
            // Act
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            _window.Received(1).Close();
        }

        #endregion

        #region T == Tlow

        [Test]
        public void Regulate_TempIsAtLowerThreshold_HeaterIsTurnedOff()
        {
            // Setup the stub with desired response
            _tempSensor.GetTemp().Returns(5);
            // Act
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            _heater.Received(1).TurnOff();
        }

        [Test]
        public void Regulate_TempIsAtLowerThreshold_WindowIsClosed()
        {
            // Setup the stub with desired response
            _tempSensor.GetTemp().Returns(5);
            // Act
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            _window.Received(1).Close();
        }

        #endregion

        #region Tlow < T < Thigh

        [Test]
        public void Regulate_TempIsBetweenLowerAndUpperThresholds_HeaterIsTurnedOff()
        {
            // Setup the stub with desired response
            _tempSensor.GetTemp().Returns(20);
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            _heater.Received(1).TurnOff();
        }

        [Test]
        public void Regulate_TempIsBetweenLowerAndUpperThresholds_WindowIsClosed()
        {
            // Setup the stub with desired response
            _tempSensor.GetTemp().Returns(20);
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            _window.Received(1).Close();
        }

        #endregion

        #region T == Thigh

        [Test]
        public void Regulate_TempIsAtUpperThreshold_HeaterIsTurnedOff()
        {
            // Setup the stub with desired response
            _tempSensor.GetTemp().Returns(25);
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            _heater.Received(1).TurnOff();
        }

        [Test]
        public void Regulate_TempIsAtUpperThreshold_WindowIsClosed()
        {
            // Setup the stub with desired response
            _tempSensor.GetTemp().Returns(25);
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            _window.Received(1).Close();
        }

        #endregion

        #region T > Thigh

        [Test]
        public void Regulate_TempIsAboveUpperThreshold_HeaterIsTurnedOff()
        {
            // Setup the stub with desired response
            _tempSensor.GetTemp().Returns(30);
            _uut.Regulate();

            // Assert on the mock - was the heater called correctly
            _heater.Received(1).TurnOff();
        }

        [Test]
        public void Regulate_TempIsAboveUpperThreshold_WindowIsOpened()
        {
            // Setup the stub with desired response
            _tempSensor.GetTemp().Returns(30);
            _uut.Regulate();

            // Assert on the mock - was the window called correctly
            _window.Received(1).Open();
        }

        #endregion

        #endregion


    }
}
