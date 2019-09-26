using static System.Console;

namespace ECS
{
    // Interface for a heater
    public interface IHeater
    {
        void TurnOn();
        void TurnOff();
        bool RunSelfTest(); 
    }
}