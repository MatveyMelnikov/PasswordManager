using Controllers;

namespace Views
{
    internal interface IView
    {
        public void Init(IController controller);
        public void Update();
    }
}
