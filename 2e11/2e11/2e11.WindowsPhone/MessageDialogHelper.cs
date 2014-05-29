using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

// From  http://ishamsaid.tumblr.com/post/76639478308/messagebox-class-problem-solved-in-windows-phone-8-1

namespace _2e11 {
    public class MessageDialogHelper {
        public static async void Show(string content, string title) {
            MessageDialog messageDialog = new MessageDialog(content, title);
            await messageDialog.ShowAsync();
        }
    }
}
