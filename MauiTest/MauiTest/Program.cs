using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Threading.Tasks;

namespace MauiTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            FirebaseServices.Init(); // Initialize Firebase services

            Console.WriteLine("Enter your nickname: ");
            string Nickname = Console.ReadLine();
            Console.WriteLine("Enter your email: ");
            string Email = Console.ReadLine();



            FirebaseServices service = new FirebaseServices();
            bool isNickNameTaken = await service.IsNicknameExists(Nickname);
            bool isEmailTaken = await service.IsEmailExists(Email);

            Console.WriteLine($"Is NickName taken: {isNickNameTaken}\nIs Email taken: {isEmailTaken}");
        }
    }
}
