using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace InitializationSystem
{
    public static class Serializer
    {
        public static readonly string persistentDataPath = Application.persistentDataPath + "/";
        public enum SerializeType { EncryptedBinary, EncryptedJson, Binary, Json }

        // Key and initialization vector for encryption
        private static byte[] encryptionKey = new byte[] { 0x43, 0x87, 0x23, 0x72, 0x45, 0x56, 0x68, 0x14,
                                                          0x24, 0x54, 0x78, 0x12, 0x90, 0x76, 0x25, 0x33 };
        private static byte[] iv = new byte[] { 0x19, 0x84, 0x57, 0x32, 0x45, 0x90, 0x65, 0x11,
                                              0x79, 0x45, 0x31, 0x58, 0x88, 0x45, 0x68, 0x25 };

        // By default we use encrypted binary format
        public static T DeserializeFromPDP<T>(string fileName, bool logIfFileNotExists = true) where T : new()=>        
             DeserializeFromPDP<T>(fileName, SerializeType.EncryptedBinary, logIfFileNotExists);        

        public static T DeserializeFromPDP<T>(string fileName, SerializeType serializationType, bool logIfFileNotExists = true) where T : new()
        {
            string path = persistentDataPath + fileName;
            if (File.Exists(path))
            {
                try
                {
                    switch (serializationType)
                    {
                        case SerializeType.Json:
                            string json = File.ReadAllText(path);
                            return JsonUtility.FromJson<T>(json) ?? new T();

                        case SerializeType.EncryptedJson:
                            string encryptedJson = File.ReadAllText(path);
                            string decryptedJson = Decrypt(encryptedJson);
                            return JsonUtility.FromJson<T>(decryptedJson) ?? new T();

                        case SerializeType.EncryptedBinary:
                            byte[] encryptedData = File.ReadAllBytes(path);
                            byte[] decryptedData = DecryptBytes(encryptedData);
                            using (var memStream = new MemoryStream(decryptedData))
                            {
                                var formatter = new BinaryFormatter();
                                return (T)formatter.Deserialize(memStream);
                            }

                        case SerializeType.Binary:
                        default:
                            using (var stream = File.Open(path, FileMode.Open))
                            {
                                var formatter = new BinaryFormatter();
                                return (T)formatter.Deserialize(stream);
                            }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Serializer] Deserialization error for {path}: {ex.Message}");
                }
            }
            else if (logIfFileNotExists)
            {
                Debug.LogWarning($"[Serializer]: File not found at '{path}'");
            }
            return new T();
        }

        // By default we use encrypted binary format
        public static void SerializeToPDP<T>(T objectToSerialize, string fileName) =>
            SerializeToPDP(objectToSerialize, fileName, SerializeType.EncryptedBinary);        

        public static void SerializeToPDP<T>(T objectToSerialize, string fileName, SerializeType serializationType)
        {
            string path = persistentDataPath + fileName;
            try
            {
                switch (serializationType)
                {
                    case SerializeType.Json:
                        string json = JsonUtility.ToJson(objectToSerialize, true);
                        File.WriteAllText(path, json);
                        break;

                    case SerializeType.EncryptedJson:
                        string jsonToEncrypt = JsonUtility.ToJson(objectToSerialize);
                        string encryptedJson = Encrypt(jsonToEncrypt);
                        File.WriteAllText(path, encryptedJson);
                        break;

                    case SerializeType.EncryptedBinary:
                        using (var memStream = new MemoryStream())
                        {
                            var formatter = new BinaryFormatter();
                            formatter.Serialize(memStream, objectToSerialize);
                            byte[] binaryData = memStream.ToArray();
                            byte[] encryptedBinary = EncryptBytes(binaryData);
                            File.WriteAllBytes(path, encryptedBinary);
                        }
                        break;

                    case SerializeType.Binary:
                    default:
                        using (var stream = File.Open(path, FileMode.Create))
                        {
                            var formatter = new BinaryFormatter();
                            formatter.Serialize(stream, objectToSerialize);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Serializer] Serialization error for {path}: {ex.Message}");
            }
        }
      
        private static string Encrypt(string data)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = encryptionKey;
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter writer = new StreamWriter(cryptoStream))
                            {
                                writer.Write(data);
                            }
                        }

                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Serializer] Encryption error: {ex.Message}");
                return string.Empty;
            }
        }
      
        private static string Decrypt(string encryptedData)
        {
            try
            {
                byte[] cipherText = Convert.FromBase64String(encryptedData);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = encryptionKey;
                    aes.IV = iv;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(cipherText))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(cryptoStream))
                            {
                                return reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Serializer] Decryption error: {ex.Message}");
                return string.Empty;
            }
        }

        // Encrypting a byte array
        private static byte[] EncryptBytes(byte[] data)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = encryptionKey;
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(data, 0, data.Length);
                            cryptoStream.FlushFinalBlock();
                        }

                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Serializer] Byte encryption error: {ex.Message}");
                return new byte[0];
            }
        }

        // Decoding the byte array
        private static byte[] DecryptBytes(byte[] encryptedData)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = encryptionKey;
                    aes.IV = iv;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(encryptedData, 0, encryptedData.Length);
                            cryptoStream.FlushFinalBlock();
                        }

                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Serializer] Byte decryption error: {ex.Message}");
                return new byte[0];
            }
        }
     
        public static void SerializeToJsonPDP<T>(T objectToSerialize, string fileName, bool prettyPrint = true) =>
            SerializeToPDP(objectToSerialize, fileName, SerializeType.EncryptedJson);        

        public static T DeserializeFromJsonPDP<T>(string fileName, bool logIfFileNotExists = true) where T : new() =>        
             DeserializeFromPDP<T>(fileName, SerializeType.EncryptedJson, logIfFileNotExists);        

        // Methods for debugging - use unencrypted formats
        public static void SerializeToPlainJsonPDP<T>(T objectToSerialize, string fileName, bool prettyPrint = true) =>
            SerializeToPDP(objectToSerialize, fileName, SerializeType.Json);        

        public static T DeserializeFromPlainJsonPDP<T>(string fileName, bool logIfFileNotExists = true) where T : new()=>       
              DeserializeFromPDP<T>(fileName, SerializeType.Json, logIfFileNotExists);         

        // Method to export encrypted save to JSON for debugging
        public static string ExportToPlainJson<T>(string fileName) where T : new()
        {
            T data = DeserializeFromPDP<T>(fileName);
            return JsonUtility.ToJson(data, true);
        }

        // Method to save to unencrypted JSON for debugging
        public static void SaveToPlainJsonFile<T>(string encryptedFileName, string plainJsonFileName) where T : new()
        {
            string json = ExportToPlainJson<T>(encryptedFileName);
            File.WriteAllText(persistentDataPath + plainJsonFileName, json);
        }

        public static bool FileExistsAtPDP(string fileName) =>
            File.Exists(persistentDataPath + fileName);

        public static void DeleteFileAtPDP(string fileName)
        {
            string path = persistentDataPath + fileName;
            if (File.Exists(path)) File.Delete(path);
        }

        // Method for changing encryption key
        public static void SetEncryptionKey(byte[] newKey, byte[] newIV)
        {
            if (newKey.Length != 16)
                throw new ArgumentException("The key must be 16 bytes (128 bits)");

            if (newIV.Length != 16)
                throw new ArgumentException("The initialization vector must be 16 bytes (128 bits)");

            encryptionKey = newKey;
            iv = newIV;
        }

        // Method for generating a random key
        public static (byte[] Key, byte[] IV) GenerateRandomKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();
                return (aes.Key, aes.IV);
            }
        }

        // Method to get key based on unique device identifier
        public static void SetDeviceSpecificKey()
        {
            string deviceId = SystemInfo.deviceUniqueIdentifier;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(deviceId + "salt_value"));
                encryptionKey = new byte[16];
                iv = new byte[16];

                Array.Copy(hashBytes, 0, encryptionKey, 0, 16);                
                Array.Copy(hashBytes, 16, iv, 0, 16);
            }
        }
    }
}