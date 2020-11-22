using System.Text;

namespace SantasToolbox
{
    public static class StringHelpers
    {
        public static StringBuilder RemoveAllOccurrencesOfChar(this StringBuilder stringBuilder, char charToRemove)
        {
            for (int i = 0; i < stringBuilder.Length; i++)
            {
                if (stringBuilder[i] == charToRemove)
                {
                    stringBuilder.Remove(i, 1);
                    i--;
                }
            }

            return stringBuilder;
        }
    }
}
