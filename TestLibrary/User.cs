using System;

namespace TestLibrary
{
    struct User
    {
        public string name;
        public int age;

        delegate void AccountHandler(string message);
        event AccountHandler Notify;

        public void DisplayInfo()
        {
            Console.WriteLine($"Name: {name}  Age: {age}");
        }
    }
}
