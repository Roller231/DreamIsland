using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UTeleApp; // если используется Telegram API

public class MySQLConnectorTG : MonoBehaviour
{
    [SerializeField] private Text idTG;
    // Базовый URL вашего API (убедитесь, что он доступен по HTTPS!)
    private string baseUrl = "https://nixzord.online/api/";
    // Для тестирования задаем userId, в реальном проекте получайте его из TelegramWebApp.InitData
    public string userId = "1";

    // Поля для хранения загруженных данных
    public int loadedPirateCoins;
    public int loadedMonkeyCoins;
    public bool loadedIsMale;
    public int loadedSkinId;
    public bool loadedIsFirstGame;

    // Флаг, показывающий, что данные инициализированы
    public bool isInitialized { get; private set; } = false;

    private void Awake()
    {
        TelegramWebApp.Ready();
        userId = GetUserIdFromInitData(TelegramWebApp.InitData).ToString();

        idTG.text = userId;
        StartCoroutine(InitializeUser());
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


    private IEnumerator InitializeUser()
    {
        yield return StartCoroutine(UserExists(userId, exists =>
        {
            if (exists)
            {
                StartCoroutine(LoadUserData(userId));
            }
            else
            {
                StartCoroutine(CreateUser(userId));
            }
        }));
    }

    // Проверка существования пользователя через API
    private IEnumerator UserExists(string id, Action<bool> callback)
    {
        string url = baseUrl + "user_exists.php?id=" + id;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                UserExistsResponse response = JsonUtility.FromJson<UserExistsResponse>(request.downloadHandler.text);
                callback(response.exists);
            }
            catch (Exception ex)
            {
                Debug.LogError("Ошибка парсинга ответа: " + ex.Message);
                callback(false);
            }
        }
        else
        {
            Debug.LogError("Ошибка запроса UserExists: " + request.error);
            callback(false);
        }
    }

    [Serializable]
    private class UserExistsResponse
    {
        public bool exists;
    }

    // Создание пользователя через API
    public IEnumerator CreateUser(string id)
    {
        string url = baseUrl + "create_user.php";
        WWWForm form = new WWWForm();
        form.AddField("id", id);

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ " + request.downloadHandler.text);
            // После создания можно сразу загрузить данные
            StartCoroutine(LoadUserData(id));
        }
        else
        {
            Debug.LogError("❌ Ошибка при создании пользователя: " + request.error);
        }
    }

    // Загрузка данных пользователя через API с разбором JSON-ответа
    public IEnumerator LoadUserData(string id)
    {
        string url = baseUrl + "load_user_data.php?id=" + id;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Данные пользователя (сырой ответ): " + request.downloadHandler.text);

            try
            {
                UserDataResponse response = JsonUtility.FromJson<UserDataResponse>(request.downloadHandler.text);
                if (response.success)
                {
                    loadedPirateCoins = response.data.pirateCoins;
                    loadedMonkeyCoins = response.data.monkeyCoins;
                    loadedIsMale = response.data.isMale;
                    loadedSkinId = response.data.skinId;
                    loadedIsFirstGame = response.data.isFirstGame;

                    Debug.Log($"Загруженные данные: pirateCoins = {loadedPirateCoins}, monkeyCoins = {loadedMonkeyCoins}, isMale = {loadedIsMale}, skinId = {loadedSkinId}, isFirstGame = {loadedIsFirstGame}");

                    // Устанавливаем флаг инициализации в true после успешной загрузки
                    isInitialized = true;
                }
                else
                {
                    Debug.LogError("Ошибка: success = false. " + response.error);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Ошибка парсинга загруженных данных: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("❌ Ошибка загрузки данных: " + request.error);
        }
    }

    // Обновление данных пользователя через API
    public IEnumerator UpdateUserData(string id, int pirateCoins, int monkeyCoins, bool isMale, int skinId, bool isFirstGame)
    {
        string url = baseUrl + "update_user_data.php";
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("pirateCoins", pirateCoins);
        form.AddField("monkeyCoins", monkeyCoins);
        form.AddField("isMale", isMale ? 1 : 0);
        form.AddField("skinId", skinId);
        form.AddField("isFirstGame", isFirstGame ? 1 : 0);

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ Ошибка обновления данных: " + request.error);
        }
    }

    [Serializable]
    private class UserDataResponse
    {
        public bool success;
        public UserData data;
        public string error;
    }

    [Serializable]
    private class UserData
    {
        public int pirateCoins;
        public int monkeyCoins;
        public bool isMale;
        public int skinId;
        public bool isFirstGame;
    }
}
