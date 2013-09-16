using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TestAuthzBehaviorClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                System.Threading.Thread.Sleep(3000);

                TestAuthZService.Service1Client proxy = new TestAuthzBehaviorClient.TestAuthZService.Service1Client("biztalkEndpoint");

                string res = string.Empty;
                try
                {
                    res = proxy.GetDataNoAuthz(100);
                    Console.WriteLine("Result : " + res);
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.GetType().ToString());
                    Console.WriteLine(exc.Message);
                    if (exc.InnerException != null)
                        Console.WriteLine(exc.InnerException.Message);
                }

                try
                {
                    res = proxy.GetData(5);
                    Console.WriteLine("Result : " + res);
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.GetType().ToString());
                    Console.WriteLine(exc.Message);
                    if (exc.InnerException != null)
                        Console.WriteLine(exc.InnerException.Message);
                }
                
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                if(exc.InnerException != null)
                    Console.WriteLine(exc.InnerException.Message);
            }
            var input = Console.ReadLine();
        }
    }
}
