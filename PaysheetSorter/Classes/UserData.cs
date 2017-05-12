namespace PaysheetSorter.Classes
{
    public class UserData
    {
        private string _pattern;
        private string _email;

        public string Pattern
        {
            get
            {
                return _pattern;
            }

            set
            {
                _pattern = value;
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                _email = value;
            }
        }

        public UserData(string pattern, string email = "")
        {
            Pattern = pattern;
            Email = email;
        }

    }
}