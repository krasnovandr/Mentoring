using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHolder
{
    class Program
    {
        static void Main(string[] args)
        {

            foreach (var file in Directory.EnumerateFiles(@"d:\Mentoring\Module_5\Task1\bin\Debug\in\"))
            {
                File.Open(file, FileMode.Open);
            }
          
            Console.ReadLine();
        }
    }
}
