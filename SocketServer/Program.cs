using SocketServer.Class;

namespace SocketServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Service service = new Service();
            service.Start();
        }
    }   
}
