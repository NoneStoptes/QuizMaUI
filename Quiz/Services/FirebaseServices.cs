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
    }
}
