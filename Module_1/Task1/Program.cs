using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Task1
{
    internal class Program
    {
        private static void Main()
        {
            Expression<Func<int, int>> incrementExpression = a => a + 1;
            Expression<Func<int, int>> decrementExpression = a => a - 1;

            var mapperList = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };
            var transformator = new ExpressionTreeTransformator(mapperList);
            var incr = transformator.VisitAndConvert(incrementExpression, "");
            var decr = transformator.VisitAndConvert(decrementExpression, "");

            var incrementResult = incr.Compile().Invoke(2);
            var decrementResult = decr.Compile().Invoke(2);

            Console.WriteLine(incrementExpression);
            Console.WriteLine(incr);
            Console.WriteLine(incrementResult);

            Console.WriteLine(decrementExpression);
            Console.WriteLine(decr);
            Console.WriteLine(decrementResult);

            Expression<Func<int, int, int>> expression = (a, b) => (a + b) + (a - 2);
            var convertedExpression = new ExpressionTreeTransformator(mapperList).VisitAndConvert(expression, "");

            if (convertedExpression != null)
            {
                Console.WriteLine(convertedExpression);
                Console.WriteLine(convertedExpression.Compile().Invoke(2, 1));
            }

            Console.ReadLine();
        }
    }
}

