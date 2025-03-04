using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Services
{
    class AppService
    {
        static public List<Category>? categories;
        static public List<Item>? items;

        static FirebaseAuthClient auth;
        static FirebaseClient client;
        static AuthCredential loginAuthUser;




        static public void Init()
        {
            var config = new FirebaseAuthConfig()
            {
                ApiKey = "AIzaSyA3_SJlgz_Ckt8DtDp2cYmT7WJ7txHS3Bg", //מפתח
                AuthDomain = "mauiproject-aaa04.firebaseapp.com", //כתובת התחברות
                Providers = new FirebaseAuthProvider[] //רשימת אפשריות להתחבר
              {
          new EmailProvider() //אנחנו נשתמש בשירות חינמי של התחברות עם מייל
              },
                UserRepository = new FileUserRepository("appUserData") //לא חובה, שם של קובץ בטלפון הפרטי שאפשר לשמור בו את מזהה ההתחברות כדי לא הכניס כל פעם את הסיסמא 
            };
            auth = new FirebaseAuthClient(config); //ההתחברות

            client =
              new FirebaseClient(@"https://mauiproject-aaa04-default-rtdb.europe-west1.firebasedatabase.app/", //כתובת מסד הנתונים
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(auth.User.Credential.IdToken)// מזהה ההתחברות של המשתמש עם השרת, הנתון נשמר במכשיר
              });
        }

        public static async Task<bool> TryRegister(string userNameString, string passwordString)
        {
            try
            {
                var respond = await auth.CreateUserWithEmailAndPasswordAsync(userNameString, passwordString);
                // User is signed up and logged in
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static async Task<bool> TryLogin(string userNameString, string passwordString)
        {
            if (userNameString == null || passwordString == null)
            {
                return false;
            }
            try
            {
                var authUser = await auth.SignInWithEmailAndPasswordAsync(userNameString, passwordString);
                loginAuthUser = authUser.AuthCredential;
                // Authentication successful 
                // We keep the token or Credential in loginAuthUser, so we can erase it later in logout
                // You can access the authenticated user's details via authUser.User
                // you should create a new user or person
                // Person person = new Person(){Email=authUser.User.info.Email, ...
                // Don't put the password in the Person :)
                return true;
            }
            catch (FirebaseAuthException ex)
            {
                // Authentication failed
                return false;
            }
        }
        public static async Task<bool> Logout()
        {
            try
            {
                loginAuthUser = null;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static async Task<List<Category>?> GetCategoriesAsync()
        {
            //FB for firebase
            try
            {
                var categoriesFromFB = await client.Child("categories").OnceAsync<Category>();
                categories = categoriesFromFB.Select(fbItm => new Category() { Id = fbItm.Key, Name = fbItm.Object.Name, Description = fbItm.Object.Description, Order = fbItm.Object.Order }).ToList();
                return categories;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<List<Category>> GetCategoriesByOrderAsync()
        {
            List<Category> newList = categories.OrderBy(ctgry => ctgry.Order).ToList();
            return categories;
        }

        public static async Task<List<Item>> GetAllItemsAsync()
        {
            //FB for firebase
            try
            {
                // Create Empty List of Items
                List<Item> newitems = new List<Item>();

                // Fetch them from FireBase and translate them (not to Item) to another class FBItem. (the Class is beneath)
                // Why do add FBItem? FBItem is almost the same as Item, only FBItem has exactly the structure as in FireBase (List of Categories Ids)
                // So we will populate a List of FBItem and in 2nd stage, change each of them to our Item
                var itemsFromFB = await client.Child("items").OnceAsync<FBItem>();
                // We are going over each of our new FBItem
                // And creating an Item with the same data (only now instead of Category, we have Category ID)
                foreach (var fbItem in itemsFromFB)
                {
                    Item item = new Item() { Id = fbItem.Key, Name = fbItem.Object.Name, Description = fbItem.Object.Description };
                    List<Category> tCategories = new List<Category>();
                    foreach (var ctryID in fbItem.Object.Categories)
                    {
                        Category specificCategory = categories.FirstOrDefault(category => category.Id == ctryID);
                        tCategories.Add(specificCategory);
                        item.Categories = tCategories;
                    }
                    newitems.Add(item);
                }
                // only if the list is't the same, change it.
                if (items != newitems)
                {
                    items = newitems;
                }
                //items = itemsFromFB.Select(fbItm => new Item() { Id = fbItm.Key, Name = fbItm.Object.Name, Description = fbItm.Object.Description ,  }).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<List<Item>> GetAllItemsAccordingACategoryAsync(string categoryName)
        {
            //the following is an example of how to get filtered items from the ALREADY loaded data.
            List<Item> filteredItems = items.Where(itm => itm.Categories.Any(ctgry => ctgry.Name == categoryName)).ToList();
            return filteredItems;
        }

        public static async Task<bool> DeleteItemAsync(Item theItemToDelete)
        {
            if (items != null)
            {

                if (items.Contains(theItemToDelete))
                {
                    //remove item by its FirebaseKey
                    string keyToDelete = theItemToDelete.Id;
                    await client.Child("items").Child(keyToDelete).DeleteAsync();

                    items.Remove(theItemToDelete);
                    return true;
                }
            }
            return false;
        }

        public static async Task<bool> AddItemAsync(Item theItemToAdd)
        {
            try
            {
                // Look for all categories Id that are used by an Item
                List<string> categoriesIds = theItemToAdd.Categories.Select(category => category.Id).ToList();
                // Create a new FireBase Item
                FBItem fbItem = new FBItem() { Name = theItemToAdd.Name, Description = theItemToAdd.Description, Categories = categoriesIds };
                // Save the new Item
                var result = await client.Child("items").PostAsync(fbItem);
                // Get back from the action, the Id that firebase created for the specific Item
                // and assign it to our new local Item
                theItemToAdd.Id = result.Key;
                // Add the new Item to the list
                items.Add(theItemToAdd);
                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }

        class FBItem // Same as Item, only List<String>? Categories is a bit different. It uses a list of Id's
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public List<String>? Categories { get; set; }
        }
    }
}
