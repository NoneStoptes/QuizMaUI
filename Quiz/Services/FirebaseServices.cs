using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quiz.Models;
using Firebase.Database.Query;

namespace Quiz.Services
{
    class FirebaseServices
    {
        static public List<QuizCategory>? categories;
        static public List<QuizQuestion>? question;
        static public List<Person>? person;

        static FirebaseAuthClient auth;
        static FirebaseClient client;
        static public void Init()
        {
            var config = new FirebaseAuthConfig()
            {
                ApiKey = "AIzaSyAWdxWnyNVuymfGKE2iCsKH-60cKDgBbqU", //מפתח
                AuthDomain = "quiz-16042007.firebaseapp.com", //כתובת התחברות
                Providers = new FirebaseAuthProvider[] //רשימת אפשריות להתחבר
              {
          new EmailProvider() //אנחנו נשתמש בשירות חינמי של התחברות עם מייל
              },
                UserRepository = new FileUserRepository("appUserData") //לא חובה, שם של קובץ בטלפון הפרטי שאפשר לשמור בו את מזהה ההתחברות כדי לא הכניס כל פעם את הסיסמא 
            };
            auth = new FirebaseAuthClient(config); //ההתחברות

            client = new FirebaseClient(
                    "https://quiz-16042007-default-rtdb.europe-west1.firebasedatabase.app/", //כתובת מסד הנתונים
                    new FirebaseOptions()
                    );
        }

        public static async Task AddUserAsync(Person person)
        {
            try
            {
                client.Child("Person").PostAsync(person);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public static async Task<Person?> AuthenticateUserAsync(string email, string password)
        {
            try
            {
                // Проверка, инициализирован ли клиент Firebase
                if (client == null)
                {
                    throw new Exception("Firebase client не инициализирован. Убедитесь, что вызван FirebaseServices.Init().");
                }

                var usersSnapshot = await client.Child("Person").OnceAsync<Person>();

                // Проверка, получены ли данные
                if (usersSnapshot == null)
                {
                    throw new Exception("Не удалось получить данные пользователей из Firebase.");
                }

                var user = usersSnapshot.Select(u => u.Object).FirstOrDefault(u => u.Email == email);

                if (user != null && user.Password == password)
                {
                    return user;
                }

                return null; // Если пользователь не найден или пароль неверный
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при аутентификации: " + ex.Message);
            }
        }

    }
}
