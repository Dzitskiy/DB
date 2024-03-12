using System.Text;

namespace DB1.Config
{
    public static class EntitiesToDbNamesConverter
    {
        public static string Convert(string name)
        {
            var result = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                char character = name[i];
                if (char.IsUpper(character) && i != 0)
                {
                    result.Append('_');
                }

                result.Append(character);
            }

            return result.ToString().ToLower();
        }
    }
}