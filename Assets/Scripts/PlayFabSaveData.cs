using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public class PlayFabSaveData : MonoBehaviour
{
    private DataToSave dataToSave;

    private void Start()
    {
        dataToSave = CreateSampleData();
    }

    
    private DataToSave CreateSampleData()
    {
        return new DataToSave
        {
            characters = new List<CharacterData>
            {
                new CharacterData
                {
                    characterId = "Character1",
                    bodyParts = new List<BodyPartData>
                    {
                        new BodyPartData
                        {
                            partIndex = 0,
                            subParts = new List<SubPartData>
                            {
                                new SubPartData { subPartIndex = 0, activeVariantIndex = 1, matIndex = 2 },
                                new SubPartData { subPartIndex = 1, activeVariantIndex = 3, matIndex = 1 } 
                            }
                        },
                        new BodyPartData
                        {
                            partIndex = 1, 
                            subParts = new List<SubPartData>
                            {
                                new SubPartData { subPartIndex = 0, activeVariantIndex = 0, matIndex = 3 } 
                            }
                        }
                    }
                }
            }
        };
    }

   
    public void SaveDataToPlayFab()
    {
        string jsonString = JsonUtility.ToJson(dataToSave);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { "CharacterData", jsonString } }
        };

        PlayFabClientAPI.UpdateUserData(request, OnDataSaved, OnError);
    }

    private void OnDataSaved(UpdateUserDataResult result)
    {
        Debug.Log("Character data saved to PlayFab successfully!");
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error saving character data to PlayFab: " + error.ErrorMessage);
    }

    
    public void LoadDataFromPlayFab()
    {
        var request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, OnDataLoaded, OnError);
    }

    private void OnDataLoaded(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("CharacterData"))
        {
            string jsonString = result.Data["CharacterData"].Value;
            dataToSave = JsonUtility.FromJson<DataToSave>(jsonString);
            DisplayData(dataToSave);
        }
        else
        {
            Debug.LogWarning("No character data found in PlayFab.");
        }
    }

    
    private void DisplayData(DataToSave data)
    {
        foreach (var character in data.characters)
        {
            Debug.Log("Character ID: " + character.characterId);
            foreach (var bodyPart in character.bodyParts)
            {
                Debug.Log("Body Part Index: " + bodyPart.partIndex);
                foreach (var subPart in bodyPart.subParts)
                {
                    Debug.Log($"  Sub Part Index: {subPart.subPartIndex}, Active Variant Index: {subPart.activeVariantIndex}, Material Index: {subPart.matIndex}");
                }
            }
        }
    }
}

[System.Serializable]
public class SubPartData
{
    public int subPartIndex;      
    public int activeVariantIndex; 
    public int matIndex;           
}

[System.Serializable]
public class BodyPartData
{
    public int partIndex;                 
    public List<SubPartData> subParts;    
}

[System.Serializable]
public class CharacterData
{
    public string characterId;          
    public List<BodyPartData> bodyParts; 
}

[System.Serializable]
public class DataToSave
{
    public List<CharacterData> characters; 
}
