using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;

public class FirebaseDataController : MonoBehaviour
{
    public DatabaseReference database;
    public FirebaseAuth auth;


    private void Start()
    {
        InitializeFirebase();
    }
    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        database = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public async void seed()
    {
        ClimateControlSystemConfig systemConfig = new();

        systemConfig.name = $"TestingConfig";
        systemConfig.pictureNames.Add("testStringValue_1");
        systemConfig.pictureNames.Add("testStringValue_2");
        systemConfig.pictureNames.Add("testStringValue_3");
        systemConfig.houseConfig = new HouseConfig();
        systemConfig.houseConfig.components.Add(new ClimateControlComponent());
        systemConfig.houseConfig.rooms.Add(new RoomConfig(0, false));
        systemConfig.houseConfig.rooms.Add(new RoomConfig(1, false));
        systemConfig.houseConfig.rooms.Add(new RoomConfig(2, false));
        systemConfig.houseConfig.rooms.Add(new RoomConfig(3, true));
        systemConfig.houseConfig.rooms.ForEach(r => { r.components.Add(new ClimateControlComponent()); });

        systemConfig.utilityConfig = new();
        //StartCoroutine(SaveConfig(systemConfig));
        var objectReturnValues = await GetConfig("TestingConfig");
        foreach (string name in objectReturnValues.pictureNames)
        {
            print(name);
        }
        print("----Reached end----");
    }

    public void ShowDetails()
    {
        print($"Current user: {auth.CurrentUser.UserId}");
        print($"Database is null: {database is null}");
    }

    public void SaveClimateControlSystemConfig(ClimateControlSystemConfig climateControlSystemConfig)
    {
        StartCoroutine(SaveConfig(climateControlSystemConfig));
    }
    public void SaveClimateControlSystemConfig(ClimateControlSystemConfig climateControlSystemConfig, AsyncRequestHelper helper)
    {
        StartCoroutine(SaveConfig(climateControlSystemConfig, helper));
    }

    private IEnumerator SaveConfig(ClimateControlSystemConfig climateControlSystemConfig, AsyncRequestHelper helper)
    {
        var configAsJson = JsonUtility.ToJson(climateControlSystemConfig);
        var DBTask = database.Child(auth.CurrentUser.UserId).Child(climateControlSystemConfig.name).SetRawJsonValueAsync(configAsJson);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to save data with {DBTask.Exception}");
        }
        helper.isProcessing = false;
    }

    public IEnumerator SaveConfig(ClimateControlSystemConfig climateControlSystemConfig)
    {
        var configAsJson = JsonUtility.ToJson(climateControlSystemConfig);
        var DBTask = database.Child(auth.CurrentUser.UserId).Child(climateControlSystemConfig.name).SetRawJsonValueAsync(configAsJson);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to save data with {DBTask.Exception}");
        }
    }

    public async Task<ClimateControlSystemConfig> GetConfig(string name)
    {
        var DBTask = await database.Child(auth.CurrentUser.UserId).Child(name).GetValueAsync();

        var dataSnapshot = DBTask.GetRawJsonValue();
        ClimateControlSystemConfig systemConfigs = JsonUtility.FromJson<ClimateControlSystemConfig>(dataSnapshot);
        return systemConfigs;
    }

    public async void AddSystemsToList(List<ClimateControlSystemConfig> list)
    {
        List<string> systemNames = await GetAllChildNames();
        foreach (var item in systemNames)
        {
            ClimateControlSystemConfig climateControlSystemConfig = await GetConfig(item);
            if (climateControlSystemConfig != null) list.Add(climateControlSystemConfig);
        }
    }

    public async void AddSystemsToList(List<ClimateControlSystemConfig> list, AsyncRequestHelper helper)
    {
        List<string> systemNames = await GetAllChildNames();
        foreach (var item in systemNames)
        {
            ClimateControlSystemConfig climateControlSystemConfig = await GetConfig(item);
            if (climateControlSystemConfig != null) list.Add(climateControlSystemConfig);
        }
        helper.isProcessing = false;
    }

    public async Task<List<string>> GetAllChildNames()
    {
        List<string> childNames = new();
        var DBTask = await database.Child(auth.CurrentUser.UserId).GetValueAsync();

        var dataSnapshot = DBTask.GetRawJsonValue();
        if (dataSnapshot != null)
        {
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataSnapshot);

            foreach (var item in dict)
            {
                childNames.Add(item.Key);
            }
        }

        return childNames;
    }
}
