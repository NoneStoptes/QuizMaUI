using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quiz.Models;

namespace Quiz.Services
{
    class LocalDataServices
    {
        //All data will be here:
        //User
        //QuizQuestions
        //Categories
        //This is not a database, just "Faking" the data
        //Later on, it will be replaced by data from the DataBase
        static public List<QuizCategory>? categories;
        static public List<QuizQuestion>? QuizQuestions;


        static public void Init()
        {
            // Структура класса QuizCategory; string MainTopic, string SubTopic, string Description, string ImageUrl
            categories = new List<QuizCategory>();
            categories.Add(new QuizCategory() { MainTopic = "Animals", SubTopic = "Cat", Description = "Cat is Animal", ImageUrl = "https://as1.ftcdn.net/jpg/10/66/60/76/1000_F_1066607684_N6t5mW8riYEyG6olgWTRnQ1hJJ2tKOx5.jpg" });
            categories.Add(new QuizCategory() { MainTopic = "2", SubTopic = "Sport", Description = "Sport if good for health", ImageUrl = "https://as1.ftcdn.net/jpg/10/66/60/76/1000_F_1066607684_N6t5mW8riYEyG6olgWTRnQ1hJJ2tKOx5.jpg" });
            categories.Add(new QuizCategory() { MainTopic = "3", SubTopic = "Furniture", Description = "We all need furnitures", ImageUrl = "https://as1.ftcdn.net/jpg/10/66/60/76/1000_F_1066607684_N6t5mW8riYEyG6olgWTRnQ1hJJ2tKOx5.jpg" });
            categories.Add(new QuizCategory() { MainTopic = "4", SubTopic = "Electronic", Description = "Phones, Computer, bip bip", ImageUrl = "https://as1.ftcdn.net/jpg/10/66/60/76/1000_F_1066607684_N6t5mW8riYEyG6olgWTRnQ1hJJ2tKOx5.jpg" });
            categories.Add(new QuizCategory() { MainTopic = "5", SubTopic = "Clothes", Description = "Fashion", ImageUrl = "https://as1.ftcdn.net/jpg/10/66/60/76/1000_F_1066607684_N6t5mW8riYEyG6olgWTRnQ1hJJ2tKOx5.jpg" });
            categories.Add(new QuizCategory() { MainTopic = "6", SubTopic = "Israel", Description = "Gr8 country", ImageUrl = "https://as1.ftcdn.net/jpg/10/66/60/76/1000_F_1066607684_N6t5mW8riYEyG6olgWTRnQ1hJJ2tKOx5.jpg" });
            categories.Add(new QuizCategory() { MainTopic = "7", SubTopic = "USA", Description = "United State of America", ImageUrl = "https://as1.ftcdn.net/jpg/10/66/60/76/1000_F_1066607684_N6t5mW8riYEyG6olgWTRnQ1hJJ2tKOx5.jpg" });

            //Структура класса QuizQuestion; DifficultyLevel Difficulty, string QuestionText, string Option1, string Option2, string Option3, string Option4, string CorrectAnswer, string CategoryId
            QuizQuestions = new List<QuizQuestion>();
            QuizQuestions.Add(new QuizQuestion() { Difficulty = DifficultyLevel.Easy, Name = "Falafel", Description = "Best Street Food", Categories = new List<QuizCategory>() { categories[0], categories[5] } });
            QuizQuestions.Add(new QuizQuestion() { Id = "2", Name = "Nike Running Shoes", Description = "Run fast", Categories = new List<QuizCategory>() { categories[1], categories[6] } });
            QuizQuestions.Add(new QuizQuestion() { Id = "3", Name = "DryFit Shirt", Description = "Dry fast", Categories = new List<QuizCategory>() { categories[1], categories[4], categories[6] } });
            QuizQuestions.Add(new QuizQuestion() { Id = "4", Name = "Tesla", Description = "Drive fast", Categories = new List<QuizCategory>() { categories[6] } });
            QuizQuestions.Add(new QuizQuestion() { Id = "5", Name = "Tank Merkava III", Description = "Shoot fast", Categories = new List<QuizCategory>() { categories[5] } });
            QuizQuestions.Add(new QuizQuestion() { Id = "6", Name = "Kitchen Table", Description = "Great Kitchen Table", Categories = new List<QuizCategory>() { categories[2] } });
            QuizQuestions.Add(new QuizQuestion() { Id = "7", Name = "Pasta", Description = "Pasta Pesto", Categories = new List<QuizCategory>() { categories[0] } });
            QuizQuestions.Add(new QuizQuestion() { Id = "8", Name = "Iphone 15", Description = "Iphone 14 - a bit better", Categories = new List<QuizCategory>() { categories[3], categories[6] } });
            QuizQuestions.Add(new QuizQuestion() { Id = "9", Name = "Android Sumsung", Description = "Sumsung great for debugging Maui", Categories = new List<QuizCategory>() { categories[3], categories[6] } });
            QuizQuestions.Add(new QuizQuestion() { Id = "10", Name = "Junior Bed", Description = "Shomrat Hazorea bed for kids", Categories = new List<QuizCategory>() { categories[2], categories[5] } });
        }



        public static async Task<bool> TryLogin(string userNameString, string passwordString)
        {
            if (userNameString == "haha@hoho.com" && passwordString == "123456")
            {
                return true;
            }
            return false;
        }
        public static async Task<List<QuizCategory>> GetCategoriesAsync()
        {
            return categories;
        }
        public static async Task<List<QuizCategory>> GetCategoriesByOrderAsync()
        {
            List<QuizCategory> newList = categories.OrderBy(ctgry => ctgry.Order).ToList();
            return categories;
        }
        public static async Task<List<QuizQuestion>> GetAllQuizQuestionsAsync()
        {
            return QuizQuestions;
        }
        public static async Task<List<QuizQuestion>> GetAllQuizQuestionsAccordingAQuizCategoryAsync(string QuizCategoryName)
        {
            List<QuizQuestion> filteredQuizQuestions = QuizQuestions.Where(itm => itm.Categories.Any(ctgry => ctgry.Name == QuizCategoryName)).ToList();
            return filteredQuizQuestions;
        }
        public static async Task<bool> DeleteQuizQuestionAsync(QuizQuestion theQuizQuestionToDelete)
        {
            if (QuizQuestions != null)
            {
                if (QuizQuestions.Contains(theQuizQuestionToDelete))
                {
                    QuizQuestions.Remove(theQuizQuestionToDelete);
                    await Task.CompletedTask; // To mimic asynchronous behavior
                    return true;
                }
            }
            return false;
        }
        public static async Task<bool> AddQuizQuestionAsync(QuizQuestion theQuizQuestionToAdd)
        {
            QuizQuestion newQuizQuestion = new QuizQuestion() { Id = theQuizQuestionToAdd.Id, Name = theQuizQuestionToAdd.Name, Description = theQuizQuestionToAdd.Description, Categories = theQuizQuestionToAdd.Categories };
            QuizQuestions!.Add(newQuizQuestion);
            return true;
        }
    }
}
}
