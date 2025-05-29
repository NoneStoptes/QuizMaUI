using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;  

namespace Quiz.Models
{
    public class PasswordHasher
    {
        // Пеппер в Base64, загружается из конфигурации/среды исполнения
        private readonly byte[] _pepper;

        public PasswordHasher(string pepperBase64)
        {
            _pepper = Convert.FromBase64String(pepperBase64);
        }

        /// <summary>
        /// Хеширует пароль: salt(16) ∥ hash(32), затем Base64.
        /// </summary>
        public string Hash(string password)
        {
            // 1) Генерируем соль
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            // 2) Собираем вход KDF = UTF8(password) ∥ pepper
            byte[] passBytes = Encoding.UTF8.GetBytes(password);
            byte[] input = new byte[passBytes.Length + _pepper.Length];
            Buffer.BlockCopy(passBytes, 0, input, 0, passBytes.Length);
            Buffer.BlockCopy(_pepper, 0, input, passBytes.Length, _pepper.Length);

            // 3) Настраиваем Argon2id
            var argon2 = new Argon2id(input)
            {
                Salt = salt,
                DegreeOfParallelism = 4, // число потоков
                MemorySize = 64 * 1024, // 64 МБ
                Iterations = 4
            };

            // 4) Получаем 32-байтный хеш
            byte[] hash = argon2.GetBytes(32);

            // 5) Склеиваем salt ∥ hash и кодируем в Base64
            byte[] result = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, result, salt.Length, hash.Length);
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Проверка пароля: извлекаем salt из хранимого хеша, повторно считаем, сравниваем.
        /// </summary>
        public bool Verify(string password, string storedBase64)
        {
            byte[] stored = Convert.FromBase64String(storedBase64);
            byte[] salt = stored.AsSpan(0, 16).ToArray();
            byte[] hash = stored.AsSpan(16).ToArray();

            // Собираем input = UTF8(password) ∥ pepper
            byte[] passBytes = Encoding.UTF8.GetBytes(password);
            byte[] input = new byte[passBytes.Length + _pepper.Length];
            Buffer.BlockCopy(passBytes, 0, input, 0, passBytes.Length);
            Buffer.BlockCopy(_pepper, 0, input, passBytes.Length, _pepper.Length);

            // Параметры KDF те же
            var argon2 = new Argon2id(input)
            {
                Salt = salt,
                DegreeOfParallelism = 4,
                MemorySize = 64 * 1024,
                Iterations = 4
            };
            byte[] computed = argon2.GetBytes(32);

            // Сравнение в «фиксированное» время
            return CryptographicOperations.FixedTimeEquals(computed, hash);
        }
    }
}
