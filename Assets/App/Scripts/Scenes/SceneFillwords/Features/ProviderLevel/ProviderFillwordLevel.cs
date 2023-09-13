using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using Unity.VisualScripting;
using UnityEngine;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        public GridFillWords LoadModel(int index)
        {
            GridFillWords gridFillWords = null;
            int validLevelsCount = 1;
            int currentLevel = 1;

            while (validLevelsCount <= index)
            {
                gridFillWords = ValidLevel(currentLevel);
                if (gridFillWords != null)
                {
                    validLevelsCount++;
                }
                currentLevel++;
            }


            return gridFillWords;
        }
        public GridFillWords ValidLevel(int index)
        {
            string wordsFilePath = Path.Combine("Fillwords", "words_list");
            string packFilePath = Path.Combine("Fillwords", "pack_0");

            TextAsset textAssetOfPack = Resources.Load<TextAsset>(packFilePath);
            TextAsset textAssetOfWords = Resources.Load<TextAsset>(wordsFilePath);

            SortedDictionary<int, char> sortedDictionary = new SortedDictionary<int, char>();
            GridFillWords fillWords = null;

            string[] packString;
            try
            {
                // Делим строку из файла pack_0.txt на две части  x | x;x;x
                packString = textAssetOfPack.text.Split("\n")[index - 1].Split(" ");
            }
            catch
            {
                throw new Exception("Can't find valid Level");
            }

            //  Массив для номеров слов, левая часть парсинга x | 0;0;0
            int[] lineNumberInTheListOfWords = new int[packString.Length / 2];

            // Строка для порядка букв, правая часть парсинга 0 | x;x;x
            string wordCipher = "";

            // Цикл деления файла pack_0 на две части, номер слов и последовательность цифр
            int localCounter = 0;
            for (int i = 0; i < packString.Length; i++)
            {
                if (i % 2 == 0)
                {
                    lineNumberInTheListOfWords[localCounter] = Convert.ToInt32(packString[i]);
                    localCounter++;
                }
                else
                {
                    wordCipher += packString[i] + ";";

                }
            }

            // Преобразование правой части в массив чисел
            int[] lettersPositionOnGrid = wordCipher.TrimEnd(";").Split(";").Select(int.Parse).ToArray();

            // Составление из всех загруженных слов одной большой строки
            StringBuilder pickedWords = new StringBuilder();
            for (int i = 0; i < lineNumberInTheListOfWords.Length; i++)
            {
                pickedWords.Append(textAssetOfWords.text.Split("\n")[lineNumberInTheListOfWords[i]].TrimEnd(""));
            }

            // Сравнение длины строки и количества позиций под каждый элемент строки
            if (pickedWords.Length != lettersPositionOnGrid.Length)
            {
                return null;
            }

            // Cписок букв для заполнения ячеек
            char[] letters = pickedWords.ToString().ToCharArray();

            // Квадратная сетка
            int gridSize = (int)Math.Sqrt((double)letters.Length);
            fillWords = new GridFillWords(new Vector2Int(gridSize, gridSize));

            // Пытаемся добавить в словарь буквы
            for (int i = 0; i < letters.Length; i++)
            {
                if (!sortedDictionary.TryAdd(lettersPositionOnGrid[i], letters[i]))
                {
                    return null;
                }

            }

            // Если сетка не квадратная || Если минимальный 
            if (gridSize * gridSize != sortedDictionary.Count || sortedDictionary.Keys.First() != 0 || sortedDictionary.Keys.Last() != sortedDictionary.Count - 1)
            {
                return null;
            }

            //Заполнение сетки 
            localCounter = 0;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    fillWords.Set(i, j, new CharGridModel(sortedDictionary[localCounter]));
                    localCounter++;
                }
            }
            return fillWords;
        }
    }
}