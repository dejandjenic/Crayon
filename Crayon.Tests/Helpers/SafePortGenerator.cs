namespace Crayon.Tests.Helpers;

public class SafePortGenerator
{
    public static object lockObject = new();
    public static List<int> ports = new();
    public static int Generate()
    {
        int port = new Random().Next(7001, 7770);
        lock (lockObject)
        {
            while (ports.Contains(port))
            {
                port = new Random().Next(7001, 7770);
            }
            ports.Add(port);
        }

        return port;
    }
}