using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Models
{
    /// <summary>
    /// Represents a single question within a quiz category.
    /// Each question knows which category it belongs to and its position within that category.
    /// </summary>
    public class QuizQuestion
    {
        /// <summary>
        /// The category this question belongs to. This links the question to its parent category.
        /// Must match a CategoryId from the QuizCategory collection.
        /// Example: "animals_africa", "history_ancient", "science_physics"
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// The sequential number of this question within its category.
        /// This determines the order in which questions appear during the quiz.
        /// Example: 1 for the first question, 2 for the second, etc.
        /// </summary>
        public int QuestionNumber { get; set; }

        /// <summary>
        /// The actual question text that will be displayed to the user.
        /// Should be clear and unambiguous.
        /// Example: "What is the largest land animal in Africa?"
        /// </summary>
        public string QuestionText { get; set; }

        /// <summary>
        /// First answer option. The order of options can be randomized when displaying.
        /// Example: "Elephant"
        /// </summary>
        public string Option1 { get; set; }

        /// <summary>
        /// Second answer option.
        /// Example: "Rhinoceros"
        /// </summary>
        public string Option2 { get; set; }

        /// <summary>
        /// Third answer option.
        /// Example: "Hippopotamus"
        /// </summary>
        public string Option3 { get; set; }

        /// <summary>
        /// Fourth answer option.
        /// Example: "Giraffe"
        /// </summary>
        public string Option4 { get; set; }

        /// <summary>
        /// The correct answer. Must exactly match one of the four options above.
        /// Used to validate the user's answer and calculate their score.
        /// Example: "Elephant"
        /// </summary>
        public string CorrectAnswer { get; set; }

        /// <summary>
        /// Helper method to get all options as a list for easier display manipulation.
        /// Useful for randomizing option order or displaying in UI components.
        /// </summary>
        public List<string> GetOptions()
        {
            return new List<string> { Option1, Option2, Option3, Option4 };
        }

        /// <summary>
        /// Checks if a given answer is correct.
        /// Handles case-insensitive comparison to be more forgiving of user input.
        /// </summary>
        public bool IsCorrectAnswer(string userAnswer)
        {
            return string.Equals(CorrectAnswer, userAnswer, StringComparison.OrdinalIgnoreCase);
        }
    }
}