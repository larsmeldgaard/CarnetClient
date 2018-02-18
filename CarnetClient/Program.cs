using System;

namespace CarnetClient
{
    class Program
    {
        static void Main(string[] args)
        {


            string username = "[USERNAME]";
            string password = "[PASSWORD]";

            CarNet carnet = new CarNet();
            Console.WriteLine("Logging in");
            carnet.Carnet_Logon(username, password);
            //Console.WriteLine(carnet.startClimate());
            //Console.WriteLine(carnet.stopClimate());
            Console.WriteLine(carnet.carnet_post("/-/msgc/get-new-messages"));
            Console.WriteLine(carnet.carnet_post("/-/emanager/get-notifications"));
            Console.WriteLine(carnet.carnet_post("/-/msgc/get-new-messages"));
            Console.WriteLine( carnet.carnet_post("/-/emanager/get-emanager"));
            //Console.WriteLine(carnet.carnet_post("/-/rts/get-trip-statistics"));
            Console.WriteLine("Done");
            Console.ReadLine();

        }
    }
    
}
