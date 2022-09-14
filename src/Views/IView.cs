using Controllers;

namespace Views
{
    internal interface IView
    {
        // Methods
        public void Init(IController controller);
        public void Update();
    }
}
