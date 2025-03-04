using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Models
{
    class QuizCategory
    {
        /// <summary>
        /// Основная тема карточки (например, "Животные")
        /// </summary>
        public string MainTopic { get; set; }

        /// <summary>
        /// Подтема карточки (например, "Африканские животные")
        /// </summary>
        public string SubTopic { get; set; }

        /// <summary>
        /// Описание темы
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Ссылка на изображение, загруженное в Firebase Storage
        /// </summary>
        public string ImageUrl { get; set; }
    }
}
