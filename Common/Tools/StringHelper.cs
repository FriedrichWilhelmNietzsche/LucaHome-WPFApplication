namespace Common.Tools
{
    public class StringHelper
    {
        public static bool StringsAreEqual(string[] stringArray)
        {
            bool areEqual = true;

            for (int index = 1; index < stringArray.Length; index++)
            {
                if(stringArray[index].Length != stringArray[index - 1].Length)
                {
                    return false;
                }

                areEqual &= stringArray[index] == stringArray[index - 1];
            }

            return areEqual;
        }

        public static string SelectString(string[] stringArray, string stringToFound)
        {
            for (int index = 1; index < stringArray.Length; index++)
            {
                if (stringArray[index].Contains(stringToFound))
                {
                    if (!stringArray[index].Contains("Error"))
                    {
                        return stringArray[index];
                    }
                }
            }

            return string.Empty;
        }

        public static int GetStringCount(string stringToTest, string charToFind)
        {
            int lastIndex = 0;
            int count = 0;

            while (lastIndex != -1)
            {
                lastIndex = stringToTest.IndexOf(charToFind, lastIndex);
                if (lastIndex != -1)
                {
                    count++;
                    lastIndex += charToFind.Length;
                }
            }

            return count;
        }
    }
}
