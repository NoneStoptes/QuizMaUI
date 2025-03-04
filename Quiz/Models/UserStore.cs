using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Models
{
    public static class UserStore
    {
        // Список для хранения зарегистрированных пользователей
        private static List<Person> _users = new List<Person>();

        // Метод для добавления пользователя
        public static void AddUser(Person person)
        {
            _users.Add(person);
        }

        // Метод для проверки пользователя по email и паролю
        public static Person ValidateUser(string email, string password)
        {
            return _users.Find(user => user.Email == email && user.Password == password);
        }
    }
}
