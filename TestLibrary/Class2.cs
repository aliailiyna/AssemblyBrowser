using System;
using System.Collections.Generic;
using System.Text;

namespace TestLibrary
{
    class Class2
    {
        List<int> list;
        int num;

        internal enum InEnum
        {
            clEnum1, clEnum2
        }

        interface Interface1Nested
        {
            string Name { get; set; }
            int GetNum();
        }
        class Class3Nested
        {
            byte b;
        }

        Class3Nested classNested;
        Interface1Nested interf;
    }
}
