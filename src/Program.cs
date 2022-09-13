using Views;
using Controllers;

using FileSystem;

class Program
{
    static void Main(string[] args)
    {
        IView view = new ConsoleView();
        IController controller = new Controller();

        controller.Init();
        view.Init(controller);

        while (true)
        {
            view.Update();
        }

        /*PluginManager pluginManager = new();

        string[]? titles = pluginManager.GetAllPluginsTitles();

        if (titles == null)
            return;

        foreach (string title in titles)
            Console.WriteLine(title);

        var algrs = pluginManager.GetEncryptionAlgorithms(titles[0]);

        algrs[0].Encrypt(new byte[0], new byte[0]);
        algrs[0].Decrypt(new byte[0], new byte[0]);*/
    }
}