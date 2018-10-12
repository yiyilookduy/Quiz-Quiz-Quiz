using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LearnJapanese.Test
{
    [TestClass]
    public class Test
    {
        private List<JapaneseWord> japaneseWords;
        [TestInitialize]
        public void Initialize()
        {
            japaneseWords = ProcessFile("japanese.txt");
        }

        private List<JapaneseWord> ProcessFile(string path)
        {
            return File.ReadAllLines(path)
                .Where(line => line.Length > 1)
                .Select(JapaneseWord.ParseFromTxt)
                .ToList();
        }

        

        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(japaneseWords.Count >0);
        }
    }
}
