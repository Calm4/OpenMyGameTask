using System;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        public LevelInfo LoadLevelData(int levelIndex)
        {
            //Загружаем json файл с Resources
            string jsonFilePath = Path.Combine("WordSearch", "Levels", $"{levelIndex}");
            TextAsset textAsset = Resources.Load<TextAsset>(jsonFilePath);

            return JsonUtility.FromJson<LevelInfo>(textAsset.text);

        }
    }
}