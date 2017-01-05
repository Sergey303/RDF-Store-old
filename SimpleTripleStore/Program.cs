using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UniversalIndex.Text_index;

namespace SimpleTripleStore
{
    class Program
    {
        private static void Main(string[] args)
        {
            var wordIndex = new WordIndex();
            var readWords =
                ReadWords(new List<string>())
                    .SelectMany(list => list.Where(s => s.Length > 3))
                    .Take(10*1000*1000)
                    .ToArray();
            int j1 = 0;
            foreach (var source in readWords.Take(10))
            {
                Console.WriteLine(j1++ + " " + source);
            }
            wordIndex.InsertPortion(readWords.Select((s, i) => new Tuple<int, string>(i, s.ToString())));
            Console.WriteLine(GC.GetTotalMemory(false)/1024/1024);

            Console.WriteLine();
            foreach (var word in readWords.Skip(10).Take(10))
            {
                for (int j = 0; j < word.Length - 3; j++)
                {
                    string subWord = word.Substring(j);
                    var res = wordIndex.FindBySubWord(subWord).ToArray();
                    for (int i = 0; i < readWords.Length; i++)
                    {
                        if (res.Contains(i))
                        {
                            if (readWords[i].Contains(subWord))
                                continue;
                            else Console.WriteLine(readWords[i]);
                        }
                        else
                        {
                            if (readWords[i].Contains(subWord)) throw new Exception("mising");
                        }
                    }
                }
            }
        }

        public static IEnumerable<List<string>> ReadWords(List<string> stopWords)
        {
            List<string> currentWordsGroup = new List<string>();
            using (StreamReader file =
                    new StreamReader(@"C:\Users\Admin\Source\Repos\next\words3Index\words3Index\all words.txt",
                        Encoding.GetEncoding(1251)))
            {
                while (!file.EndOfStream)
                {
                    var line = file.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        yield return currentWordsGroup;
                        currentWordsGroup = new List<string>();
                        continue;
                    }
                    var strings = line.Split(new[] { " | " }, StringSplitOptions.None);
                    var word = strings[0].Replace("*", "").Trim();

                    if (strings[1].StartsWith("част") ||
                        (strings[1].StartsWith("межд") || strings[1].StartsWith("предик")) ||
                        (strings[1].StartsWith("предл") || strings[1].StartsWith("союз")))

                        stopWords.Add(word);
                    else

                        currentWordsGroup.Add(word);
                }
                yield return currentWordsGroup;
            }
        }
    }
}
