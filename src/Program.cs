using Views;
using Controllers;

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
    }
}