using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Database;
using Firebase.Database.Query;
using System;

namespace MauiTest
{
    class FirebaseServices
    {

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

        public async Task<bool> IsNicknameExists(string nickname)
        {
            try
            {
                var persons = await client
                    .Child("Person")
                    .OnceAsync<Person>();

                return persons.Any(x =>
                !string.IsNullOrEmpty(x?.Object?.Nickname) &&
                x.Object.Nickname.Trim().ToLower() == nickname.Trim().ToLower());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FirebaseServices] Ошибка при проверке никнейма: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsEmailExists(string email)
        {
            try
            {
                var persons = await client
                    .Child("Person")
                    .OnceAsync<Person>();

                return persons.Any(x => x.Object.Email == email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FirebaseServices] Ошибка при проверке никнейма: {ex.Message}");
                return false;
            }
        }
    }
}
