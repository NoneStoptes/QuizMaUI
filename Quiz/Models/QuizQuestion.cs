using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Models
{
    /// <summary>
    /// Represents a single question within a quiz.
    /// Questions are stored in Firebase under their category folder structure,
    /// so the category relationship is maintained by the database path rather than a property.
    /// </summary>
    public class QuizQuestion
    {
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
    }
}