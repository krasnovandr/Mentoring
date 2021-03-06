﻿using System;
using Task2.Entities;

namespace Task2
{
    static class Program
    {
        static void Main(string[] args)
        {
            var mapGenerator = new MappingGenerator();
            var mapper = mapGenerator.Generate<Foo, Bar>();
            var objectForCopy = new Foo
            {
                Name = "Alex",
                Age = 22,
                BirthDate = DateTime.Now,
                Test = new TestClass { A = 2}
            };
            var res = mapper.Map(objectForCopy);
           
            Console.WriteLine("{0}", res.Age);
            Console.WriteLine("{0}", res.BirthDate);
            Console.WriteLine("{0}", res.Name);
            Console.WriteLine("{0}", res.Test.A);
            Console.ReadKey();
        }
    }
}
