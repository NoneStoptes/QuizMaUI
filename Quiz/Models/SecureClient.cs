using System;
using System.Security.Cryptography;
using System.Text;

namespace Quiz.Models
{
    public class SecureClient
    {
        private readonly byte[] _key; // 32 байта для AES-256-GCM

        /// <summary>
        /// key — 32-байтный секрет (AES-256).
        /// </summary>
        public SecureClient(byte[] key)
        {
            if (key == null || key.Length != 32)
                throw new ArgumentException("Ключ должен быть ровно 32 байта для AES-256", nameof(key));
            _key = key;
        }

        /// <summary>
        /// Шифрует plain и возвращает payload = nonce(12) ∥ tag(16) ∥ ciphertext.
        /// </summary>
        public byte[] Encrypt(byte[] plain)
        {
            // 1) Генерируем 12-байтный nonce
            byte[] nonce = RandomNumberGenerator.GetBytes(12);

            // 2) Подготавливаем буферы
            byte[] ciphertext = new byte[plain.Length];
            byte[] tag = new byte[16];

            // 3) Шифруем
            using (var aes = new AesGcm(_key))
            {
                aes.Encrypt(nonce, plain, ciphertext, tag);
            }

            // 4) Собираем output = nonce ∥ tag ∥ ciphertext
            byte[] output = new byte[nonce.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, output, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, output, nonce.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, output, nonce.Length + tag.Length, ciphertext.Length);

            return output;
        }

        /// <summary>
        /// Удобный метод: шифрует строку UTF-8 и выдаёт Base64.
        /// </summary>
        public string EncryptToBase64(string plainText)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = Encrypt(plainBytes);
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// При желании можно добавить метод Decrypt, разбирающий payload = nonce ∥ tag ∥ ciphertext.
        /// </summary>
        public byte[] Decrypt(byte[] payload)
        {
            // 1) извлекаем nonce, tag, ciphertext
            byte[] nonce = new byte[12];
            byte[] tag = new byte[16];
            byte[] ciphertext = new byte[payload.Length - nonce.Length - tag.Length];

            Buffer.BlockCopy(payload, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(payload, nonce.Length, tag, 0, tag.Length);
            Buffer.BlockCopy(payload, nonce.Length + tag.Length, ciphertext, 0, ciphertext.Length);

            // 2) расшифровываем
            byte[] plain = new byte[ciphertext.Length];
            using (var aes = new AesGcm(_key))
            {
                aes.Decrypt(nonce, ciphertext, tag, plain);
            }

            return plain;
        }
    }
}
