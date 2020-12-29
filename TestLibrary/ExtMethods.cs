using System;
using System.Collections.Generic;
using System.Text;

namespace TestLibrary
{ 
    public static class ExtMethods
    {
        public static int ExtMethod(this ClassExt classExt)
        {
            return 1;
        }

        public static int CharCount(this string str, char c)
        {
            int counter = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == c)
                    counter++;
            }
            return counter;
        }
    }
}
