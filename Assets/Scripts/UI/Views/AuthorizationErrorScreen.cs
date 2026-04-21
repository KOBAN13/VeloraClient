using UI.Core;
using UI.ViewModels;

namespace UI.Views
{
    public class AuthorizationErrorScreen : Screen<AuthorizationErrorViewModel>
    {
        public override void Initialize()
        {
            Bind();
        }
    }
}
