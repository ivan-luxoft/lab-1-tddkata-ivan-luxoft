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
        private const int ERROR = -1;
        private const int MAX_VALID_NUMBER = 1000;

        internal int Sum(string v)
        {
            ReadOnlyCollection<int> arrayOfInteger;
            try
            {
                arrayOfInteger = new ArgumentSplitter(v).Arguments;
            }
            catch
            {
                return ERROR;
            }
            if (arrayOfInteger.Count == 0)
                return ERROR;
            return arrayOfInteger
                .Select(i => i > MAX_VALID_NUMBER ? 0 : i)
                .Sum();
        }

        private class ArgumentSplitter
        {
            private char[] delimeters = new char[] { ',', '\n' };
            private List<int> arguments = new List<int>();

            public ReadOnlyCollection<int> Arguments => arguments.AsReadOnly();

            public ArgumentSplitter(string argumentsString)
            {
                if (argumentsString == "")
                    arguments.Add(0);
                else if (argumentsString != null)
                {
                    var stringArgWoDelimeter = FindDelimeterAndReturnArgumentsRow(argumentsString);
                    if (stringArgWoDelimeter != null)
                    {
                        var argumentsFromString = SplitStringToArrayInt(stringArgWoDelimeter);
                        this.arguments.AddRange(argumentsFromString);
                    }
                }
            }

            private int[] SplitStringToArrayInt(string arg)
            {
                var splitedArguments = arg.Split(this.delimeters);
                ValidateSplitedArguments(splitedArguments);
                return splitedArguments
                    .Select(s => int.Parse(s, NumberStyles.None))
                    .ToArray();
            }

            private static void ValidateSplitedArguments(string[] splitedArguments)
            {
                if (splitedArguments.Any(s => s.Equals("")))
                    throw new ArgumentException("Between delimeters cannot be empty space");
                foreach (var splitedArgument in splitedArguments)
                {
                    if (!int.TryParse(splitedArgument, out int _))
                        throw new ArgumentException("Between delimeters should be only digits");
                }
            }

            private void ValidateCustomDelimeter(char[] customDelimeter)
            {
                if (customDelimeter.Length != 1)
                    throw new ArgumentException("Custom delimeter length must be one symbol");
                if (int.TryParse(customDelimeter[0].ToString(), out var _))
                    throw new ArgumentException("Custom delimeter cannot be number");
            }

            private char[] GetCustomDelimeter(string arg)
            {
                var customDelimeterParameters = arg.Split('\n').First();
                var customDelimeter = customDelimeterParameters.Skip(2).ToArray();
                return customDelimeter;
            }

            private string FindDelimeterAndReturnArgumentsRow(string arg)
            {
                if (!arg.StartsWith("//"))
                    return arg;
                this.delimeters = GetCustomDelimeter(arg);
                ValidateCustomDelimeter(this.delimeters);
                return arg.Replace($"//{new string(this.delimeters)}\n", "");
            }
        }
    }
}