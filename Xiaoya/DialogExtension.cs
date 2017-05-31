using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Xiaoya
{
    public static class DialogExtension
    {
        private static TaskCompletionSource<ContentDialog> _currentDialogShowRequest;

        /// <summary>
        /// Begins an asynchronous operation showing a dialog.
        /// If another dialog is already shown using
        /// ShowAsyncQueue method - it will wait
        /// for that previous dialog to be dismissed before showing the new one.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">This method can only be invoked from UI thread.</exception>
        public static async Task<ContentDialogResult> ShowAsyncQueue(this ContentDialog dialog)
        {
            if (!Window.Current.Dispatcher.HasThreadAccess)
            {
                throw new InvalidOperationException("This method can only be invoked from UI thread.");
            }

            while (_currentDialogShowRequest != null)
            {
                await _currentDialogShowRequest.Task;
            }

            var request = _currentDialogShowRequest = new TaskCompletionSource<ContentDialog>();
            var result = await dialog.ShowAsync();
            _currentDialogShowRequest = null;
            request.SetResult(dialog);

            return result;
        }
    }
}
