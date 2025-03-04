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

    public class QuizQuestion
    {
        /// <summary>
        /// Уровень сложности (только один из: VeryEasy, Easy, Normal, Hard)
        /// </summary>
        public DifficultyLevel Difficulty { get; set; }

        /// <summary>
        /// Сам вопрос викторины
        /// </summary>
        public string QuestionText { get; set; }

        /// <summary>
        /// Варианты ответов
        /// </summary>
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }

        /// <summary>
        /// Правильный ответ (должен совпадать с одним из вариантов)
        /// </summary>
        public string CorrectAnswer { get; set; }

        /// <summary>
        /// Идентификатор категории, к которой относится вопрос
        /// </summary>
        public string CategoryId { get; set; }
    }
}
