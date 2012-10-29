namespace kv
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using StashyLib;

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var f = new FileStashy();

            string valueFromPipe = ReadPipeLine();

            if (args.Length == 0)
            {
                // List all keys.
                bool foundOne = false;
                foreach (var s in f.ListKeys<Snippet>())
                {
                    if (foundOne) Console.WriteLine();
                    Console.Write(s);
                    foundOne = true;
                }
            }

            if (args.Length == 1 && valueFromPipe == null)
            {
                if (args[0].In("h", "?", "help", "/?", "-?", "/h", "-h", "--help"))
                {
                    Console.WriteLine(@"
kv -- a key-value store integrated with clipboard.
inspired by: https://github.com/stevenleeg/boo
 
usage:

kv name fred smith
    saves the value, 'fred smith' under the key, 'name'

kv name
    retrieve the value 'fred smith' straight to your clipboard.

kv
    lists all keys

kv h* 
    lists all keys that match the pattern 'h*'

kv -r name
    will remove the key ‘name’ (and its value) from your store");
                    return;
                }

                // search by key;
                var key = args[0];
                try
                {
                    var snippet = f.Load<Snippet>(key);
                    string value = snippet.Value;
                    Console.Write(value);
                    Clipboard.SetDataObject(value, true);
                }
                catch (FileNotFoundException)
                {
                    bool foundOne = false;
                    if (key.Contains("*") || key.Contains("?"))
                    {
                        // List all keys.
                        foreach (var s in f.ListKeys<Snippet>(key))
                        {
                            if (foundOne) Console.WriteLine();
                            Console.Write(s);
                            foundOne = true;
                        }
                    }
                    
                    if (!foundOne)
                    {
                        Console.Write("No such key");
                    }
                }
            }

            if (args.Length > 1 || (args.Length == 1 && valueFromPipe != null))
            {
                if (args[0].In("r", "-r", "/r", "--remove"))
                {
                    //To delete a key use -r
                    //, e.g. kv -r a
                    f.Delete<Snippet>(args[1]);
                    return;
                }

                var key = args[0];
                string data = valueFromPipe;

                if (args.Length > 1)
                {
                    // all of the arguments after the first one are joined up and used as the value.
                    string[] value = new string[args.Length - 1];
                    for (int i = 1; i < args.Length; i++)
                    {
                        value[i - 1] = args[i];
                    }

                    data = string.Join(" ", value);
                }

                try
                {
                    f.Save<Snippet>(new Snippet() { Value = data, Key = key }, key);
                    Console.Write("Saved");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetType().ToString());
                }
            }
        }

        // read the whole pipeline -- or return null if there is nothing in the pipe.
        // hat tip: http://stackoverflow.com/questions/199528/c-console-receive-input-with-pipe/4074212#4074212
        private static string ReadPipeLine()
        {
            string valueFromPipe = null;
            try
            {
                bool isKeyAvailable = System.Console.KeyAvailable;
            }
            catch (InvalidOperationException)
            {
                valueFromPipe = System.Console.In.ReadToEnd();
            }
            return valueFromPipe;
        }
    }

    public class Snippet
    {
        public string Value { get; set; }
        public string Key { get; set; }
    }

    //hat tip to http://sysi.codeplex.com
    public static class Extensions
    {
        public static bool In(this string self, params string[] strings)
        {            
            foreach (var s in strings)
            {
                if (s.ToLowerInvariant() == self) return true;
            }

            return false;
        }
    }
}
