using System;
using MySql.Data.MySqlClient;
using UnityEngine;
using UTeleApp;

public class MySQLConnector : MonoBehaviour
{
    private string connectionString = "Server=lelyim7e.beget.tech;Database=lelyim7e_nixzord;User ID=lelyim7e_nixzord;Password=141722A!a;Port=3306;Pooling=false;Charset=utf8mb4;";
    private MySqlConnection connection;

    private void Awake()
    {
        TelegramWebApp.Ready();
        string userId = GetUserIdFromInitData(TelegramWebApp.InitData).ToString();

        if (IsConnected())
        {
            if (UserExists(userId))
            {
                LoadUserData(userId);
            }
            else
            {
                CreateUser(userId);
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Подключение к БД не удалось. Функции базы данных отключены.");
        }
    }

    public static long GetUserIdFromInitData(string initData)
    {
        try
        {
            // Декодируем URL-строку
            string decodedData = Uri.UnescapeDataString(initData);

            // Ищем JSON-часть, начинающуюся с "user={"
            int userStartIndex = decodedData.IndexOf("user={") + 5; // 5 = длина "user="
            if (userStartIndex == -1)
            {
                Debug.LogError("User data not found in initData.");
                return -1;
            }

            // Находим конец JSON-объекта
            int userEndIndex = decodedData.IndexOf('}', userStartIndex);
            if (userEndIndex == -1)
            {
                Debug.LogError("Malformed initData string.");
                return -1;
            }

            // Извлекаем JSON-строку (пример: {"id":1008871802,...})
            string userJson = decodedData.Substring(userStartIndex, userEndIndex - userStartIndex + 1);

            // Ищем "id" в JSON (пример: "id":1008871802)
            string idKey = "\"id\":";
            int idStartIndex = userJson.IndexOf(idKey) + idKey.Length;

            if (idStartIndex == -1)
            {
                Debug.LogError("ID not found in user JSON.");
                return -1;
            }

            // Находим конец значения id
            int idEndIndex = userJson.IndexOfAny(new char[] { ',', '}' }, idStartIndex);
            if (idEndIndex == -1)
            {
                Debug.LogError("Malformed user JSON string.");
                return -1;
            }

            // Извлекаем ID и парсим его как long
            string idString = userJson.Substring(idStartIndex, idEndIndex - idStartIndex).Trim();
            return long.Parse(idString);
        }
        catch (Exception ex)
        {
            Debug.Log($"Error extracting User ID: {ex.Message}");
            return -1;
        }
    }

    private bool IsConnected()
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            connection.Close();
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Ошибка подключения к MySQL: " + ex.Message);
            return false;
        }
    }

    private bool UserExists(string id)
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            string query = "SELECT COUNT(*) FROM users WHERE id = @id";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@id", id);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Ошибка при проверке пользователя: " + ex.Message);
            return false;
        }
        finally
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }
    }

    public void CreateUser(string id)
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            string query = @"INSERT INTO users (id, pirateCoins, monkeyCoins, isMale, skinId, isFirstGame) 
                            VALUES (@id, 0, 0, true, 0, true)";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Debug.Log($"✅ Новый пользователь {id} создан в БД!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Ошибка при создании пользователя: " + ex.Message);
        }
        finally
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }
    }

    public void LoadUserData(string id)
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            string query = "SELECT pirateCoins, monkeyCoins, isMale, skinId, isFirstGame FROM users WHERE id = @id LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int pirateCoins = reader.GetInt32("pirateCoins");
                        int monkeyCoins = reader.GetInt32("monkeyCoins");
                        bool isMale = reader.GetBoolean("isMale");
                        int skinId = reader.GetInt32("skinId");
                        bool isFirstGame = reader.GetBoolean("isFirstGame");

                        Debug.Log($"✅ Данные пользователя загружены: pirateCoins={pirateCoins}, monkeyCoins={monkeyCoins}, isMale={isMale}, skinId={skinId}, isFirstGame={isFirstGame}");
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ Пользователь не найден в БД!");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Ошибка при загрузке данных пользователя: " + ex.Message);
        }
        finally
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }
    }

    public void UpdateUserData(string id, int pirateCoins, int monkeyCoins, bool isMale, int skinId, bool isFirstGame)
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            string query = @"UPDATE users 
                            SET pirateCoins = @pirateCoins, 
                                monkeyCoins = @monkeyCoins, 
                                isMale = @isMale, 
                                skinId = @skinId,
                                isFirstGame = @isFirstGame
                            WHERE id = @id";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@pirateCoins", pirateCoins);
                cmd.Parameters.AddWithValue("@monkeyCoins", monkeyCoins);
                cmd.Parameters.AddWithValue("@isMale", isMale);
                cmd.Parameters.AddWithValue("@skinId", skinId);
                cmd.Parameters.AddWithValue("@isFirstGame", isFirstGame);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                    Debug.Log($"✅ Данные пользователя {id} обновлены в БД!");
                else
                    Debug.LogWarning($"⚠️ Пользователь {id} не найден для обновления.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Ошибка при обновлении данных пользователя: " + ex.Message);
        }
        finally
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }
    }

    private void OnDestroy()
    {
        if (connection != null)
        {
            connection.Dispose();
        }
    }
}