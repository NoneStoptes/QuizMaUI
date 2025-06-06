using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Models
{
    public enum DifficultyLevel
    {
        VeryEasy,
        Easy,
        Normal,
        Hard
    }

    /// <summary>
    /// Represents a quiz category that will be displayed as a card on the HomePage.
    /// This model is purely for UI display purposes - it shows users what quiz topics are available.
    /// </summary>
    public class QuizCategory
    {
        /// <summary>
        /// Unique identifier for this category. This will be passed to the quiz page
        /// when a user selects this category.
        /// Example: "animals_africa", "history_ancient", "science_physics"
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// The main topic or subject area.
        /// Example: "Animals", "History", "Science"
        /// </summary>
        public string MainTopic { get; set; }

        /// <summary>
        /// The specific area within the main topic.
        /// Example: "African Wildlife", "Ancient Rome", "Quantum Physics"
        /// </summary>
        public string SubTopic { get; set; }

        /// <summary>
        /// A brief description of what this quiz covers.
        /// This helps users understand what knowledge will be tested.
        /// Example: "Test your knowledge about lions, elephants, and other amazing African animals!"
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// URL to the category's image stored in Firebase Storage.
        /// This image will be displayed on the category card.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// The total number of questions in this category.
        /// This is stored directly rather than counted dynamically for better performance.
        /// </summary>
        public int TotalQuestions { get; set; }
    }
}