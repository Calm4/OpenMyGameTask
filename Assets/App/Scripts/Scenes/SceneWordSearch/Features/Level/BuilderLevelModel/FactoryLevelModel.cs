using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using UnityEngine;


namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{

    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();

            model.LevelNumber = levelNumber;

            model.Words = value.words;
            model.InputChars = BuildListChars(value.words);

            return model;
        }

        private List<char> BuildListChars(List<string> words)
        {
            // ќбщий словарь с буквами и их количеством
            Dictionary<char, int> letters = new Dictionary<char, int>(); 

            foreach (string word in words)
            {
                // —ловарь под буквы и их количеством в каждом слове, дл€ выбора максимального количества определенной буквы 
                Dictionary<char, int> letterCount = new Dictionary<char, int>();

                foreach (char letter in word)
                {
                    if (!letterCount.ContainsKey(letter))
                    {
                        letterCount[letter] = 1;
                    }
                    else
                    {
                        letterCount[letter]++;
                    }
                }

                // ¬ыбор количества раз использовани€ определенной буквы 
                foreach (var let in letterCount)
                {
                    int count = let.Value;
                    char letter = let.Key;
                    if (!letters.ContainsKey(letter))
                    {
                        letters[letter] = count;
                    }
                    else
                    {
                        letters[letter] = Math.Max(letters[letter], count);
                    }
                }
            }

            List<char> chars = new List<char>();

            foreach (var letter in letters)
            {
                int count = letter.Value;

                for (int i = 0; i < count; i++)
                {
                    chars.Add(letter.Key);
                }
            }


            //ѕеремешевание букв в игровом кольце
            System.Random random = new System.Random();
            int charsCount = chars.Count;
            while (charsCount > 1)
            {
                charsCount--;
                int randomPosition = random.Next(charsCount + 1);

                char temp = chars[randomPosition];
                chars[randomPosition] = chars[charsCount];
                chars[charsCount] = temp;
            }

            return chars;
        }
    }
}