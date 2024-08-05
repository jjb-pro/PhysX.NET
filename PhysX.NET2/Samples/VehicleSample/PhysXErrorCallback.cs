namespace PhysX.Samples.VehicleSample;

internal class PhysXErrorCallback : ErrorCallback
{
    public override void ReportError(ErrorCode errorCode, string message, string file, int lineNumber)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"{errorCode} error in {file} at {lineNumber}: {message}.");
        Console.ResetColor();
    }
}