using Start.Runtime;

namespace Start.Script
{
    public static partial class UIActions
    {
        public static void OpenAPanel()
        {
            Actions.OpenUI(nameof(APanel));
        }

        public static void APanel_Close()
        {
            Actions.CloseUI(nameof(APanel));
        }

        public static void OpenBPanel()
        {
            Actions.OpenUI(nameof(BPanel));
        }

        public static void OpenCPopup()
        {
            Actions.OpenUI(nameof(CPopup));
        }

        public static void GoBackAPanel()
        {
            Actions.GoBackUI(nameof(APanel));
        }
    }
}