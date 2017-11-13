using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuchShop.Services
{
    public class GreetingService : IGreetingService
    {
        public string GetGreeting()
        {
            return "Hello";
        }
    }
}
