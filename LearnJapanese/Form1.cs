using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;

//
using LearnJapanese.Classes;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;

namespace LearnJapanese
{
    public partial class Form1 : Form
    {
        private List<RadioButton> _radioGroup;
        private readonly List<int> _indexRadioLoaded;
        private readonly List<int> _indexWordLoaded;
        private readonly Random _random;
        private List<JapaneseWord> _japaneseWords;
        private string _answer;
        private const int RADIOBUTTONS = 4;

        private readonly ImageSearchAPI _client;
        public Form1()
        {
            InitializeComponent();
            LoadWords();
            LoadRadioGroup();
            _indexRadioLoaded = new List<int>();
            _indexWordLoaded = new List<int>();
            _random = new Random();
            _client = new ImageSearchAPI(new ApiKeyServiceClientCredentials(ApplicationContants.SubscriptionKey));
            LoadDataQuiz();
        }
        private void LoadRadioGroup()
        {
            _radioGroup = new List<RadioButton> { rdoAnswer1, rdoAnswer2, rdoAnswer3, rdoAnswer4 };
        }
        private void LoadWords()
        {
            _japaneseWords = ProcessFile(ConfigurationManager.AppSettings["FilePath"]);
        }

        private IEnumerable<string> GetRandomWord()
        {
            while (true)
            {
                int num = _random.Next(_japaneseWords.Count);
                if (!_indexWordLoaded.Contains(num))
                {
                    _indexWordLoaded.Add(num);
                    yield return _japaneseWords[num].Japanese;
                }
            }
        }

        private IEnumerable<RadioButton> GetRandomRadio()
        {
            while (true)
            {
                int num = _random.Next(_radioGroup.Count);
                if (!_indexRadioLoaded.Contains(num))
                {
                    _indexRadioLoaded.Add(num);
                    yield return _radioGroup[num];
                }
            }
        }

        private List<JapaneseWord> ProcessFile(string path)
        {
            return File.ReadAllLines(path)
                .Where(line => line.Length > 1)
                .Select(JapaneseWord.ParseFromTxt)
                .ToList();
        }

        private void LoadDataQuiz()
        {
            // Get random word in source
            var word = _japaneseWords[_random.Next(_japaneseWords.Count)];

            LoadAnswers(word);

            LoadComponent(word);

            // Set answer for check
            _answer = word.Japanese;

            IncreaseNumOfQuestion();
;        }

        private void IncreaseNumOfQuestion()
        {
            int increaseQuestionNum = int.Parse(lblQuestionNum.Text.Replace(".", string.Empty));
            lblQuestionNum.Text = (increaseQuestionNum + 1).ToString();
        }

        private void LoadAnswers(JapaneseWord word)
        {
            // Get random only 3 radio, the fourth is answer
            for (int i = 0; i < RADIOBUTTONS - 1; i++)
            {
                GetRandomRadio().First().Text = GetRandomWord().First(text => Text.Length > 1);
            }

            bool isAlreadyHaveAnswerInRadio = false;

            foreach (var radioButton in _radioGroup)
            {
                if (radioButton.Text == word.Japanese)
                    isAlreadyHaveAnswerInRadio = true;
            }

            if (isAlreadyHaveAnswerInRadio)
                GetRandomRadio().First().Text = GetRandomWord().First(text => Text.Length > 1);
            else
                GetRandomRadio().First().Text = word.Japanese;
        }

        private void LoadComponent(JapaneseWord word)
        {
            lblVietnamese.Text = word.Vietnamese;

            var imageResults = _client.Images.SearchAsync(query: word.Vietnamese).Result;

            pbbImage.ImageLocation = imageResults.Value[_random.Next(5)].ThumbnailUrl;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var radioButton in _radioGroup)
                {
                    if (radioButton.Checked)
                    {
                        if (radioButton.Text == _answer)
                        {
                            MessageBox.Show(ApplicationContants.ANSWERCORRECT,ApplicationContants.MESSAGEBOXINFORMATION, MessageBoxButtons.OK);
                            LoadAgain();
                        }
                        else
                        {
                            MessageBox.Show(ApplicationContants.ANSWERWORNG, ApplicationContants.MESSAGEBOXINFORMATION, MessageBoxButtons.OK);
                            LoadAgain();
                        }
                    }
                }
                if (_radioGroup.All(rdo => !rdo.Checked))
                {
                    MessageBox.Show(ApplicationContants.NOTCHOOSEANSWER);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, ApplicationContants.MESSAGEBOXINFORMATION, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadAgain()
        {
            _indexRadioLoaded.Clear();
            _indexWordLoaded.Clear();
            LoadDataQuiz();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                LoadAgain();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, ApplicationContants.MESSAGEBOXINFORMATION, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void lesson5VocabularyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _japaneseWords = ProcessFile(ApplicationContants.JAPANESESOURCE5);
                lblTotalVocabulary.Text = ApplicationContants.TOTALVOCABULARY + _japaneseWords.Count;
                MessageBox.Show(ApplicationContants.LOADEDSUCCESS);
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format(ApplicationContants.ERROR,exception.Message));
            }
        }
    }
}
