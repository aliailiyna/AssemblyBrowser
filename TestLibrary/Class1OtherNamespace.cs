using TestLibrary;

namespace OtherNamespace
{
    class Class1OtherNamespace
    {
        int num;
        string str;
        ClassEnum en;

        void Nothing()
        {

        }

        int GetNum()
        {
            return num;
        }

        int GetSqr(int num1, int num2)
        {
            return num1 * num2;
        }

        string GetDoubleString(string first)
        {
            return first + first + str;
        }
    }
}
