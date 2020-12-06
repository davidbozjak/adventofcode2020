using System;

namespace SantasToolbox
{
    public class MultiLineParser<T>
        where T : class
    {
        private readonly Func<T> constructorFunc;
        private readonly Action<T, string> additionFunc;
        
        private T? currentGroup = null;

        public MultiLineParser(Func<T> constructorFunc, Action<T, string> additionFunc)
        {
            this.constructorFunc = constructorFunc;
            this.additionFunc = additionFunc;
        }

        public bool AddLine(string? input, out T? value)
        {
            value = null;

            if (input == null)
            {
                if (currentGroup != null)
                {
                    value = currentGroup;
                    currentGroup = null;
                    return true;
                }
                else return false;
            }

            if (string.IsNullOrWhiteSpace(input) && currentGroup == null)
                return false;

            if (string.IsNullOrWhiteSpace(input) && currentGroup != null)
            {
                value = currentGroup;
                currentGroup = null;
                return true;
            }

            if (currentGroup == null)
            {
                currentGroup = this.constructorFunc();
            }

            this.additionFunc(this.currentGroup, input);

            return true;
        }
    }
}
