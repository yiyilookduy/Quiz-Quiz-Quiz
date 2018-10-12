namespace LearnJapanese
{
    public class JapaneseWord
    {
        public string Japanese { get; set; }
        public string Vietnamese { get; set; }

        public static JapaneseWord ParseFromTxt(string line)
        {
            var columns = line.Split(',');

            return new JapaneseWord
            {
                Japanese = columns[0],
                Vietnamese = columns[1]
            };
        }
    }
}
