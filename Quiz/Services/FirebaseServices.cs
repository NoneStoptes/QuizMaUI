using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json.Linq;
using Quiz.Models;

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

        public async Task<bool> IsNicknameExists(string nickname)
        {
            try
            {
                var persons = await client
                    .Child("Person")
                    .OnceAsync<Person>();

                if (persons == null || persons.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Firebase: список пуст или null.");
                    return false;
                }

                foreach (var x in persons)
                {
                    string nickFromDb = x?.Object?.Nickname;
                    if (!string.IsNullOrEmpty(nickFromDb))
                    {
                        System.Diagnostics.Debug.WriteLine($"Firebase Nickname: '{nickFromDb}'");
                        if (nickFromDb.Trim().ToLower() == nickname.Trim().ToLower())
                            return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirebaseServices] Ошибка при проверке никнейма: {ex.Message}");
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

                if (persons == null || persons.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Firebase: список пуст или null.");
                    return false;
                }

                foreach (var x in persons)
                {
                    string emailFromDb = x?.Object?.Email;
                    if (!string.IsNullOrEmpty(emailFromDb))
                    {
                        System.Diagnostics.Debug.WriteLine($"Firebase Nickname: '{emailFromDb}'");
                        if (emailFromDb.Trim().ToLower() == email.Trim().ToLower())
                            return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FirebaseServices] Ошибка при проверке никнейма: {ex.Message}");
                return false;
            }
        }


        public async Task<Person> GetUserByEmailAndPassword(string email, string password)
        {
            var persons = await client
                .Child("Person")
                .OnceAsync<Person>();

            var user = persons
                .FirstOrDefault(x => x.Object.Email == email && x.Object.Password == password);

            return user?.Object;
        }

        public static async Task<(string Pepper, int Index)> GetLatestPepperAsync_FromArray()
        {
            using var http = new HttpClient();
            var json = await http.GetStringAsync("https://quiz-16042007-default-rtdb.europe-west1.firebasedatabase.app/peppers.json");

            var array = JArray.Parse(json);

            // перебираем с конца и ищем первый непустой элемент
            for (int i = array.Count - 1; i >= 0; i--)
            {
                var item = array[i];
                if (item != null && item["pepper"] != null)
                {
                    string pepper = item["pepper"]!.ToString();
                    return (pepper, i);
                }
            }

            throw new Exception("Нет валидных пеперов в базе данных");
        }

        public static async Task<List<Person>> GetAllPersonsAsync()
        {
            var result = await client.Child("Person").OnceAsync<Person>();
            return result.Select(p => p.Object).ToList();
        }

        public static async Task<(string pepper, int index)> GetPepperByIndex_FromArray(int index)
        {
            var json = await new HttpClient().GetStringAsync("https://quiz-16042007-default-rtdb.europe-west1.firebasedatabase.app/peppers.json");
            var array = JArray.Parse(json);

            var item = array[index];
            if (item == null || item["pepper"] == null)
                throw new Exception($"Pepper with index {index} not found.");

            return (item["pepper"]!.ToString(), index);
        }
    }
}
