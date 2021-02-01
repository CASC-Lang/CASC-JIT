using System.Linq;

namespace CASC
{
    public static class IOUtil
    {
        public static string ArrayToString(object[] array)
            => string.Format("[{0}]", string.Join(", ", array.Select(obj =>
                    {
                        if (obj is string str)
                        {
                            return $"\"{str}\"";
                        }
                        else
                        {
                            return obj.ToString();
                        }
                    })
                    )
                );
    }
}