// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace TDDKata
{
    internal class StringCalc
    {
        internal int Sum(string v)
        {
            try
            {
                var arrayOfInteger = new ArgumentSplitter(v).GetArguments();

                if (!arrayOfInteger.Any())
                    return -1;

                return arrayOfInteger
                    .Select(i => i > 1000 ? 0 : i)
                    .Aggregate((i1, i2) => i1 + i2);
            }
            catch
            {
                return -1;
            }
        }

        private class ArgumentSplitter
        {
            private char[] _delimeters = new char[] { ',', '\n' };
            private List<int> arguments = new List<int>();

            private string _argumentsString;

            public ReadOnlyCollection<int> Arguments => arguments.AsReadOnly();

            public ArgumentSplitter(string argumentsString)
            {
                _argumentsString = argumentsString;
            }

            public IEnumerable<int> GetArguments()
            {
                var result = new List<int>();

                if (_argumentsString == string.Empty)
                {
                    result.Add(0);
                    return result;
                }

                if (_argumentsString == null)
                {
                    return result;
                }

                var stringArgWoDelimeter = _argumentsString;
                if (_argumentsString.StartsWith("//"))
                {
                    _delimeters = FindDelimeters(_argumentsString);
                    stringArgWoDelimeter = GetArgumentsRow(_argumentsString);
                }

                if (stringArgWoDelimeter != null)
                {
                    var argumentsFromString = SplitStringToArrayInt(stringArgWoDelimeter, _delimeters);
                    result.AddRange(argumentsFromString);
                }

                return result;
            }

            private int[] SplitStringToArrayInt(string arg, char[] delimeters)
            {
                var splitedArguments = arg.Split(delimeters);

                if (splitedArguments.Any(s => s.Equals("")))
                    throw new ArgumentException("Between delimeters cannot be empty space");

                return splitedArguments
                        .Select(s => int.Parse(s, NumberStyles.None))
                        .ToArray();
            }

            private static char[] FindDelimeters(string arg)
            {
                var customDelimeterParameters = arg.Split('\n').First();
                var customDelimeter = customDelimeterParameters.Skip(2).ToArray();

                if (customDelimeter.Length != 1)
                    throw new ArgumentException("Custom delimeter length must be one symbol");

                if (int.TryParse(customDelimeter[0].ToString(), out var _))
                    throw new ArgumentException("Custom delimeter cannot be number");

                return customDelimeter;
            }

            private static string GetArgumentsRow(string arg)
            {
                var customDelimeterParameters = arg.Split('\n').First();
                return arg.Replace($"{customDelimeterParameters}\n", ""); ;
            }
        }
    }
}