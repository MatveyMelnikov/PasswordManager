using Controllers;

namespace Views
{
    internal class ConsoleView : IView
    {
        protected IController? _controller = null;

        public void Init(IController controller)
        {
            _controller = controller;

            while (true)
            {
                if (_controller.IsRegistered)
                    Console.WriteLine("--SignIn--");
                else
                    Console.WriteLine("--Registration--");

                Console.WriteLine("-Enter the first key word:");
                string? firstKeyWord = Console.ReadLine();
                Console.WriteLine("-Enter the second key word:");
                string? secondKeyWord = Console.ReadLine();

                if (firstKeyWord == null || secondKeyWord == null)
                    throw new Exception("Data entry error. Strings are empty");

                if (_controller.Enter(firstKeyWord, secondKeyWord))
                {
                    Console.Clear();
                    break;
                }
                else
                {
                    Console.WriteLine("Login failed");
                    ConsolePause();
                    Console.Clear();
                }
            }
        }

        public void Update()
        {
            String? command = null;

            ShowMenu();
            command = Console.ReadLine();

            switch (command)
            {
                case "1":
                    ShowNote();
                    break;
                case "2":
                    AddNote();
                    break;
                case "3":
                    DeleteNote();
                    break;
                case "4":
                    ChangeDataFileName();
                    break;
                case "5":
                    SetPlugin();
                    break;
                default:
                    break;
            }
            Console.Clear();
        }

        protected void ConsolePause()
        {
            Console.WriteLine("-Press any key to continue...");
            Console.ReadKey();
        }

        protected void ShowMenu()
        {
            if (!IsInit())
                throw new Exception("View did not initialize required fields");

            Console.WriteLine("--Menu--");
            Console.WriteLine("1) Show note;");
            Console.WriteLine("2) Add new note;");
            Console.WriteLine("3) Delete note;");
            Console.WriteLine("4) Change data file;");
            Console.WriteLine("5) Set plugin;");
            Console.WriteLine($"--Notes ({_controller!.GetDataFileName()})--");

            string[]? titles = _controller.GetNotesTitles();
            if (titles == null)
                return;

            for (int i = 0; i < titles.Length; i++)
                Console.WriteLine($"{i + 1}) {titles[i]}");

            Console.WriteLine("-Enter the command number:");
        }

        protected void ShowNote()
        {
            if (!IsInit())
                throw new Exception("View did not initialize required fields");

            Console.WriteLine("-Enter the note number to display it:");
            int numOfNote = 0;
            try
            {
                numOfNote = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Input error: {e.GetType}");
                ConsolePause();
                return;
            }

            string[]? note = _controller?.GetNote(numOfNote - 1);
            if (note == null)
                return;

            Console.WriteLine($"-Target: {note[0]}");
            Console.WriteLine($"-First key word: {note[1]}");
            Console.WriteLine($"-Second key word: {note[2]}");
            ConsolePause();
        }

        protected void AddNote()
        {
            if (!IsInit())
                throw new Exception("View did not initialize required fields");

            String? target = null, firstKeyWord = null, secondKeyWord = null;

            Console.WriteLine("-Enter the target of note:");
            target = Console.ReadLine();
            Console.WriteLine("-Enter the first key word:");
            firstKeyWord = Console.ReadLine();
            Console.WriteLine("-Enter the second key word:");
            secondKeyWord = Console.ReadLine();
            if (target == null || firstKeyWord == null || secondKeyWord == null)
            {
                Console.WriteLine("Input error");
                ConsolePause();
                return;
            }

            _controller!.Encrypt(target, firstKeyWord, secondKeyWord);
        }

        protected void DeleteNote()
        {
            if (!IsInit())
                throw new Exception("View did not initialize required fields");

            Console.WriteLine("-Enter the note number to delete it:");
            int numOfNote = 0;
            try
            {
                numOfNote = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Input error: {e.GetType}");
                ConsolePause();
                return;
            }
            _controller!.DeleteNote(numOfNote - 1);
        }

        protected void ChangeDataFileName()
        {
            if (!IsInit())
                throw new Exception("View did not initialize required fields");

            string[]? dataFiles = _controller!.GetAllDataFiles();
            if (dataFiles == null)
                return;

            for (int i = 0; i < dataFiles.Length; i++)
                Console.WriteLine($"{i + 1}) {dataFiles[i]}");

            Console.WriteLine("-Enter the data file number to install it as a current:");
            int dataFileNum = 0;
            try
            {
                dataFileNum = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Input error: {e.GetType}");
                ConsolePause();
                return;
            }
            dataFileNum -= 1;

            if (dataFileNum < 0 || dataFileNum >= dataFiles.Length)
                return;
            _controller!.ChangeDataFileName(dataFiles[dataFileNum]);
        }

        protected void SetPlugin()
        {
            if (!IsInit())
                throw new Exception("View did not initialize required fields");

            string[]? pluginFiles = _controller!.GetAllPluginTitles();
            if (pluginFiles == null)
                return;

            Console.WriteLine("-Plugins:");
            Console.WriteLine("1) Set by default");
            for (int i = 0; i < pluginFiles.Length; i++)
                Console.WriteLine($"{i + 2}) {pluginFiles[i]}");

            Console.WriteLine("-Enter the plugin file number to install it as a current:");
            int pluginFileNum = 0;
            try
            {
                pluginFileNum = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Input error: {e.GetType}");
                ConsolePause();
                return;
            }
            pluginFileNum -= 2;
            string pluginName = pluginFileNum == -1 ? "default" : pluginFiles[pluginFileNum];

            Console.WriteLine("-Do you want to re-encrypt all data files? (\"yes\" for confirmation):");
            bool reEncryptData = Console.ReadLine()?.ToLower() == "yes" ? true : false;

            Console.WriteLine("-Do you want to re-encrypt login data? (\"yes\" for confirmation):");
            bool reEncryptLoginData = Console.ReadLine()?.ToLower() == "yes" ? true : false;

            _controller.SetPlugin(pluginName, reEncryptData, reEncryptLoginData);
        }

        protected bool IsInit()
        {
            if (_controller == null)
                return false;
            else
                return true;
        }
    }
}
