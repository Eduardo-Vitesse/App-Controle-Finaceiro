namespace AppControleFinanceiro.Utils.FixBugs
{
    public class KeyboardBugs
    {
        public static void HideKeyboard()
        {
            #if ANDROID
              if(Platform.CurrentActivity.CurrentFocus != null) {
                    Platform.CurrentActivity.HideKeyboard(Platform.CurrentActivity.CurrentFocus);
              }
            #endif
        }
    }
}
