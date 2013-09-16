using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace TestAuthZServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting service ...");

            try
            {
                ServiceHost host = new ServiceHost(typeof(TestAuthZService.Service1));
                host.Open();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                if(exc.InnerException != null)
                    Console.WriteLine(exc.InnerException.Message);
            }

            Console.WriteLine("Service started ...");

            var input = Console.ReadLine();
        }
    }
}
