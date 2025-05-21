using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using Quiz.Models;

namespace Quiz.Models
{
    public static class UserPreferences
    {
        // Сохраняет все поля Person
        public static void SavePerson(Person person)
        {
            Preferences.Set("Name", person.Name);
            Preferences.Set("Nickname", person.Nickname);
            Preferences.Set("Email", person.Email);
            Preferences.Set("Password", person.Password); // хранящийся локально, не в базе!
        }

        // Загружает Person из Preferences
        public static Person? LoadPerson()
        {
            string name = Preferences.Get("Name", null);
            string nickname = Preferences.Get("Nickname", null);
            string email = Preferences.Get("Email", null);
            string password = Preferences.Get("Password", null);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nickname))
                return null; // недостаточно данных — пользователь не залогинен

            return new Person
            {
                Name = name,
                Nickname = nickname,
                Email = email,
                Password = password
            };
        }

        // Проверяет, есть ли сохранённый пользователь
        public static bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(Preferences.Get("Email", null)) &&
                   !string.IsNullOrEmpty(Preferences.Get("Nickname", null));
        }

        // Очищает все сохранённые данные пользователя
        public static void ClearPerson()
        {
            Preferences.Remove("Name");
            Preferences.Remove("Nickname");
            Preferences.Remove("Email");
            Preferences.Remove("Password");
        }
    }
}