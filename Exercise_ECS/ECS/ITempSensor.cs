namespace ECS
{
    // Interface for a temperature sensor
    public interface ITempSensor
    {
        int GetTemp();
        bool RunSelfTest();
    }
}