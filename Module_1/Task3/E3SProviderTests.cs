using System;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Task3.E3SClient;
using Task3.E3SClient.Entities;

namespace Task3
{
    [TestClass]
    public class E3SProviderTests
    {
        [TestMethod]
        public void WithoutProvider()
        {
            var client = new E3SQueryClient(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
            var res = client.SearchFTS<EmployeeEntity>("workstation:(EPRUIZHW0249)", 0, 1);

            foreach (var emp in res)
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }

        [TestMethod]
        public void WithoutProviderNonGeneric()
        {
            var client = new E3SQueryClient(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
            var res = client.SearchFTS(typeof(EmployeeEntity), "workstation:(EPRUIZHW0249)", 0, 10);

            foreach (var emp in res.OfType<EmployeeEntity>())
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }


        [TestMethod]
        public void WithProvider()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            foreach (var emp in employees.Where(e => e.project == "ESYJ-NDC"))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
            Console.WriteLine(Environment.NewLine);

            var easyjetPersonsStartWith = employees.Where(e => e.project.StartsWith("E"));
            foreach (var emp in easyjetPersonsStartWith)
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
            Console.WriteLine(Environment.NewLine);

            var easyjetPersonsContains = employees.Where(e => e.project.Contains("E"));
            foreach (var emp in easyjetPersonsContains)
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }

            Console.WriteLine(Environment.NewLine);

            var easyjetPersonsEndsWith = employees.Where(e => e.project.EndsWith("E"));
            foreach (var emp in easyjetPersonsEndsWith)
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }

        [TestMethod]
        public void WithProviderAndMethod()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
            var easyjetPersonsAnd = employees.Where(e => e.project == "ESYJ-NDC" && e.project == "EPM-ASMT");
            foreach (var emp in easyjetPersonsAnd)
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }
    }
}
