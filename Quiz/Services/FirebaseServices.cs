using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        static public List<QuizQuestion>? QuizQuestion;
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

        // Add this method to your existing FirebaseServices class:

        public static async Task<List<QuizCategory>> GetCategoriesByDifficultyAsync(string difficulty)
        {
            try
            {
                if (client == null)
                {
                    Init();
                }

                var result = new List<QuizCategory>();

                // Get the raw JSON data to handle both arrays and objects
                var jsonString = await client
                    .Child("Themes")
                    .Child(difficulty)
                    .OnceAsJsonAsync();

                if (!string.IsNullOrEmpty(jsonString) && jsonString != "null")
                {
                    var jsonData = Newtonsoft.Json.Linq.JToken.Parse(jsonString);

                    // Check if it's an array (like VeryEasy)
                    if (jsonData is Newtonsoft.Json.Linq.JArray array)
                    {
                        System.Diagnostics.Debug.WriteLine($"{difficulty} is stored as an array");

                        for (int i = 0; i < array.Count; i++)
                        {
                            var item = array[i];
                            if (item != null && item.Type != Newtonsoft.Json.Linq.JTokenType.Null)
                            {
                                try
                                {
                                    var category = item.ToObject<QuizCategory>();
                                    if (category != null)
                                    {
                                        // Ensure CategoryId is set
                                        if (string.IsNullOrEmpty(category.CategoryId))
                                        {
                                            category.CategoryId = i.ToString();
                                        }
                                        result.Add(category);
                                        System.Diagnostics.Debug.WriteLine($"Added from array: {category.MainTopic} - {category.SubTopic}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error parsing array item {i}: {ex.Message}");
                                }
                            }
                        }
                    }
                    // If it's an object (like Easy, Normal, Hard)
                    else if (jsonData is Newtonsoft.Json.Linq.JObject obj)
                    {
                        System.Diagnostics.Debug.WriteLine($"{difficulty} is stored as an object");

                        foreach (var prop in obj.Properties())
                        {
                            var item = prop.Value;
                            if (item != null && item.Type != Newtonsoft.Json.Linq.JTokenType.Null)
                            {
                                try
                                {
                                    var category = item.ToObject<QuizCategory>();
                                    if (category != null)
                                    {
                                        // Ensure CategoryId is set
                                        if (string.IsNullOrEmpty(category.CategoryId))
                                        {
                                            category.CategoryId = prop.Name;
                                        }
                                        result.Add(category);
                                        System.Diagnostics.Debug.WriteLine($"Added from object: {category.MainTopic} - {category.SubTopic}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error parsing object property {prop.Name}: {ex.Message}");
                                }
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Total loaded {result.Count} categories for difficulty: {difficulty}");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching categories for difficulty {difficulty}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<QuizCategory>();
            }
        }

        // Add this method to your FirebaseServices class:

        public static async Task<List<QuizQuestion>> GetQuizQuestionsByCategoryAsync(string categoryId)
        {
            try
            {
                if (client == null)
                {
                    Init();
                }

                var result = new List<QuizQuestion>();
                System.Diagnostics.Debug.WriteLine($"Fetching questions for category: {categoryId}");

                // First try to get the entire Question node
                var allQuestionsData = await client
                    .Child("Question")
                    .OnceSingleAsync<object>();

                if (allQuestionsData == null)
                {
                    System.Diagnostics.Debug.WriteLine("No questions data found in Firebase");
                    return result;
                }

                // Check if it's an array or object structure
                if (allQuestionsData is Newtonsoft.Json.Linq.JArray jArray)
                {
                    System.Diagnostics.Debug.WriteLine("Questions stored as array");

                    // Handle array structure: Question[categoryId][questionIndex]
                    int catId;
                    if (int.TryParse(categoryId, out catId) && catId < jArray.Count)
                    {
                        var categoryData = jArray[catId];

                        if (categoryData != null && categoryData is Newtonsoft.Json.Linq.JArray categoryArray)
                        {
                            // Skip null entries and convert to Question objects
                            foreach (var item in categoryArray)
                            {
                                if (item != null && item.Type != Newtonsoft.Json.Linq.JTokenType.Null)
                                {
                                    try
                                    {
                                        var question = item.ToObject<QuizQuestion>();
                                        if (question != null && !string.IsNullOrEmpty(question.QuestionText))
                                        {
                                            result.Add(question);
                                            System.Diagnostics.Debug.WriteLine($"Added question: {question.QuestionText}");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Error parsing question: {ex.Message}");
                                    }
                                }
                            }
                        }
                        else if (categoryData != null && categoryData is Newtonsoft.Json.Linq.JObject categoryObject)
                        {
                            // Handle case where category data is an object with numbered keys
                            foreach (var prop in categoryObject.Properties())
                            {
                                try
                                {
                                    var question = prop.Value.ToObject<QuizQuestion>();
                                    if (question != null && !string.IsNullOrEmpty(question.QuestionText))
                                    {
                                        result.Add(question);
                                        System.Diagnostics.Debug.WriteLine($"Added question from object: {question.QuestionText}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error parsing question from object: {ex.Message}");
                                }
                            }
                        }
                    }
                }
                else if (allQuestionsData is Newtonsoft.Json.Linq.JObject jObject)
                {
                    System.Diagnostics.Debug.WriteLine("Questions stored as object");

                    // Handle object structure: Question.categoryId.questionId
                    if (jObject.ContainsKey(categoryId))
                    {
                        var categoryData = jObject[categoryId];

                        if (categoryData is Newtonsoft.Json.Linq.JObject categoryObject)
                        {
                            // Sort by key and convert to Question objects
                            var sortedProps = categoryObject.Properties()
                                .OrderBy(p =>
                                {
                                    int key;
                                    return int.TryParse(p.Name, out key) ? key : int.MaxValue;
                                });

                            foreach (var prop in sortedProps)
                            {
                                try
                                {
                                    var question = prop.Value.ToObject<QuizQuestion>();
                                    if (question != null && !string.IsNullOrEmpty(question.QuestionText))
                                    {
                                        result.Add(question);
                                        System.Diagnostics.Debug.WriteLine($"Added question: {question.QuestionText}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error parsing question: {ex.Message}");
                                }
                            }
                        }
                        else if (categoryData is Newtonsoft.Json.Linq.JArray categoryArray)
                        {
                            // Handle mixed structure where category is an array
                            foreach (var item in categoryArray)
                            {
                                if (item != null && item.Type != Newtonsoft.Json.Linq.JTokenType.Null)
                                {
                                    try
                                    {
                                        var question = item.ToObject<QuizQuestion>();
                                        if (question != null && !string.IsNullOrEmpty(question.QuestionText))
                                        {
                                            result.Add(question);
                                            System.Diagnostics.Debug.WriteLine($"Added question from array: {question.QuestionText}");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Error parsing question from array: {ex.Message}");
                                    }
                                }
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Total questions loaded: {result.Count}");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching questions for category {categoryId}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<QuizQuestion>();
            }
        }

        // Alternative simplified method - add this to FirebaseServices as well:

        public static async Task<List<QuizQuestion>> GetQuizQuestionsByCategorySimpleAsync(string categoryId)
        {
            try
            {
                if (client == null)
                {
                    Init();
                }

                var result = new List<QuizQuestion>();

                // Direct path approach for array structure
                var questions = await client
                    .Child($"Question/{categoryId}")
                    .OnceSingleAsync<List<QuizQuestion>>();

                if (questions != null)
                {
                    // Filter out null entries
                    result = questions.Where(q => q != null && !string.IsNullOrEmpty(q.QuestionText)).ToList();
                    System.Diagnostics.Debug.WriteLine($"Loaded {result.Count} questions using simple method");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No questions found using simple method");
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Simple method failed: {ex.Message}");
                return new List<QuizQuestion>();
            }
        }

        // Add this debug method to FirebaseServices to help diagnose the structure:

        public static async Task<string> DebugQuestionStructureAsync()
        {
            try
            {
                if (client == null)
                {
                    Init();
                }

                var debugInfo = new System.Text.StringBuilder();
                debugInfo.AppendLine("=== Firebase Question Structure Debug ===");

                // Get the entire Question node
                var questionNode = await client
                    .Child("Question")
                    .OnceSingleAsync<object>();

                if (questionNode == null)
                {
                    debugInfo.AppendLine("Question node is null!");
                    return debugInfo.ToString();
                }

                debugInfo.AppendLine($"Question node type: {questionNode.GetType().Name}");

                if (questionNode is Newtonsoft.Json.Linq.JArray jArray)
                {
                    debugInfo.AppendLine($"Question is an array with {jArray.Count} elements");

                    for (int i = 0; i < Math.Min(jArray.Count, 5); i++)
                    {
                        var element = jArray[i];
                        if (element != null && element.Type != Newtonsoft.Json.Linq.JTokenType.Null)
                        {
                            debugInfo.AppendLine($"  [{i}]: {element.Type}");

                            if (element is Newtonsoft.Json.Linq.JArray subArray)
                            {
                                debugInfo.AppendLine($"    Sub-array with {subArray.Count} elements");
                                for (int j = 0; j < Math.Min(subArray.Count, 3); j++)
                                {
                                    if (subArray[j] != null && subArray[j].Type != Newtonsoft.Json.Linq.JTokenType.Null)
                                    {
                                        debugInfo.AppendLine($"      [{j}]: {subArray[j].Type}");
                                    }
                                }
                            }
                        }
                    }
                }
                else if (questionNode is Newtonsoft.Json.Linq.JObject jObject)
                {
                    debugInfo.AppendLine($"Question is an object with {jObject.Count} properties");

                    foreach (var prop in jObject.Properties().Take(5))
                    {
                        debugInfo.AppendLine($"  {prop.Name}: {prop.Value.Type}");

                        if (prop.Value is Newtonsoft.Json.Linq.JObject subObj)
                        {
                            debugInfo.AppendLine($"    Sub-object with {subObj.Count} properties");
                        }
                        else if (prop.Value is Newtonsoft.Json.Linq.JArray subArray)
                        {
                            debugInfo.AppendLine($"    Sub-array with {subArray.Count} elements");
                        }
                    }
                }

                return debugInfo.ToString();
            }
            catch (Exception ex)
            {
                return $"Debug failed: {ex.Message}";
            }
        }

        // Add this method to FirebaseServices for handling nested array structure:

        public static async Task<List<QuizQuestion>> GetQuestionsFromNestedArrayAsync(string categoryId)
        {
            try
            {
                if (client == null)
                {
                    Init();
                }

                var result = new List<QuizQuestion>();

                // For nested array structure like: Question[1][0], Question[1][1], etc.
                // First, try to get all questions under the category
                var categoryQuestions = await client
                    .Child("Question")
                    .Child(categoryId)
                    .OnceSingleAsync<object>();

                if (categoryQuestions == null)
                {
                    System.Diagnostics.Debug.WriteLine($"No data at Question/{categoryId}");
                    return result;
                }

                // Check if it's a JArray
                if (categoryQuestions is Newtonsoft.Json.Linq.JArray questionArray)
                {
                    System.Diagnostics.Debug.WriteLine($"Found array with {questionArray.Count} elements");

                    foreach (var item in questionArray)
                    {
                        if (item != null && item.Type != Newtonsoft.Json.Linq.JTokenType.Null)
                        {
                            try
                            {
                                var question = item.ToObject<QuizQuestion>();
                                if (question != null && !string.IsNullOrEmpty(question.QuestionText))
                                {
                                    result.Add(question);
                                    System.Diagnostics.Debug.WriteLine($"Added: {question.QuestionText}");
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Failed to parse question: {ex.Message}");
                            }
                        }
                    }
                }
                // If it's a dictionary/object with numeric keys
                else if (categoryQuestions is Newtonsoft.Json.Linq.JObject questionObj)
                {
                    System.Diagnostics.Debug.WriteLine($"Found object with {questionObj.Count} properties");

                    // Sort by numeric key
                    var sortedQuestions = questionObj.Properties()
                        .OrderBy(p =>
                        {
                            int key;
                            return int.TryParse(p.Name, out key) ? key : int.MaxValue;
                        });

                    foreach (var prop in sortedQuestions)
                    {
                        try
                        {
                            var question = prop.Value.ToObject<QuizQuestion>();
                            if (question != null && !string.IsNullOrEmpty(question.QuestionText))
                            {
                                result.Add(question);
                                System.Diagnostics.Debug.WriteLine($"Added: {question.QuestionText}");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to parse question at key {prop.Name}: {ex.Message}");
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Unexpected data type: {categoryQuestions.GetType().Name}");
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetQuestionsFromNestedArrayAsync: {ex.Message}");
                return new List<QuizQuestion>();
            }
        }
    }
}
