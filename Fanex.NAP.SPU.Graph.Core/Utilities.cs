using System.IO;
using System.Linq;
using System.Reflection;

namespace Fanex.NAP.SPU.Graph.Core
{
    public static class Utilities
    {
        public static string ResolvePath(string path)
        {
            DirectoryInfo result;
            if (path.StartsWith("~"))
            {
                var location = Assembly.GetExecutingAssembly().CodeBase;
                var file = new FileInfo(location.Replace("file:///", ""));
                var isConsole = file.Directory.GetFiles("*exe").Any();

                if (isConsole)
                {
                    var arr = path.Replace("~/", "").Split('/');
                    result = file.Directory;

                    for (int i = 0; i < arr.Length - 1; i++)
                    {
                        result = result.GetDirectories(arr[i], SearchOption.AllDirectories)[0];
                    }

                    return $"{result.FullName}\\{arr[arr.Length - 1]}";
                }
                else
                {
                    result = file.Directory.Parent.GetDirectories(path.Replace("~/", ""))[0];
                    return result.FullName;
                }

            }

            return path;
        }
    }
}