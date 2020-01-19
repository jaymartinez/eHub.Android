using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace eHub.Android
{
    public static class Dialogs
    {
        public static AlertDialog SimpleAlert(Context ctx, string title, string message, string buttonText = "Ok")
        {
            return new AlertDialog.Builder(ctx)
                .SetMessage(message)
                .SetTitle(title)
                .SetPositiveButton(buttonText, new DialogCloseOnClickListener())
                .Create();
        }

        public static AlertDialog SimpleAlert(Context ctx, string title, int resId, string buttonText = "Ok")
        {
            return new AlertDialog.Builder(ctx)
                .SetMessage(resId)
                .SetTitle(title)
                .SetPositiveButton(buttonText, new DialogCloseOnClickListener())
                .Create();
        }

        public static AlertDialog AlertWithContinuation(Context ctx, string title, string message, Action continuation, string buttonText = "Ok")
        {
            return new AlertDialog.Builder(ctx)
                .SetMessage(message)
                .SetTitle(title)
                .SetPositiveButton(buttonText, (s, e) => continuation?.Invoke())
                .Create();
        }

        public static AlertDialog SimpleAlert(Context ctx, int titleResId, int messageResId, string buttonText = "Ok")
        {
            return new AlertDialog.Builder(ctx)
                .SetMessage(messageResId)
                .SetTitle(titleResId)
                .SetPositiveButton(buttonText, new DialogCloseOnClickListener())
                .Create();
        }

        public static AlertDialog SimpleAlert(Context ctx, int titleResId, string message, string buttonText = "Ok")
        {
            return new AlertDialog.Builder(ctx)
                .SetMessage(message)
                .SetTitle(titleResId)
                .SetPositiveButton(buttonText, new DialogCloseOnClickListener())
                .Create();
        }

        public static AlertDialog SimpleAlert(Context ctx, string title, string message, Action onConfirmed, string buttonText = "Ok")
        {
            return new AlertDialog.Builder(ctx)
                .SetMessage(message)
                .SetTitle(title)
                .SetPositiveButton(buttonText, (s, e) => onConfirmed())
                .Create();
        }
        public static AlertDialog SimpleAlert(Context ctx, int titleResId, int messageResId, Action onConfirmed, string buttonText = "Ok")
        {
            return new AlertDialog.Builder(ctx)
                .SetMessage(messageResId)
                .SetTitle(titleResId)
                .SetPositiveButton(buttonText, (s, e) => onConfirmed())
                .Create();
        }

        public static AlertDialog SimpleConfirm(Context ctx, string titleResId, string message, Action onConfirmed, Action onCanceled, string positiveText = "Okay", string negativeText = "Cancel")
        {
            return new AlertDialog.Builder(ctx)
                .SetTitle(titleResId)
                .SetMessage(message)
                .SetPositiveButton(positiveText, (s, e) => onConfirmed())
                .SetNegativeButton(negativeText, (s, e) => onCanceled())
                .Create();

        }

        public static AlertDialog SimpleConfirm(Context ctx, int titleResId, int messageResId, Action onConfirmed, Action onCanceled, string positiveText = "Okay", string negativeText = "Cancel")
        {
            return new AlertDialog.Builder(ctx)
                .SetTitle(titleResId)
                .SetMessage(messageResId)
                .SetPositiveButton(positiveText, (s, e) => onConfirmed())
                .SetNegativeButton(negativeText, (s, e) => onCanceled())
                .Create();

        }

        public static AlertDialog SimpleConfirm(Context ctx, string title, string message, string confirmBtn, Action onConfirmed, string cancelBtn = "Cancel")
        {
            return new AlertDialog.Builder(ctx)
                .SetTitle(title)
                .SetMessage(message)
                .SetCancelable(true)
                .SetNegativeButton(cancelBtn, new DialogCloseOnClickListener())
                .SetPositiveButton(confirmBtn, (s, e) => onConfirmed())
                .Create();
        }

        public static AlertDialog Confirm(Context ctx, string title, string message, string confirmBtn, Action<bool> onSelectionMade, string cancelBtn = "Cancel")
        {
            return new AlertDialog.Builder(ctx)
                .SetTitle(title)
                .SetMessage(message)
                .SetCancelable(true)
                .SetPositiveButton(confirmBtn, (s, e) => onSelectionMade?.Invoke(true))
                .SetNeutralButton(cancelBtn, (s, e) => onSelectionMade?.Invoke(false))
                .Create();
        }

        public static AlertDialog DestructiveConfirm(Context ctx, string title, string message, string confirmBtn, Action onConfirmed, string cancelBtn = "Cancel")
        {
            return new AlertDialog.Builder(ctx)
                .SetTitle(title)
                .SetMessage(message)
                .SetCancelable(true)
                .SetNeutralButton(cancelBtn, new DialogCloseOnClickListener())
                .SetNegativeButton(confirmBtn, (s, e) => onConfirmed())
                .Create();
        }

        public static AlertDialog SingleSelect(Context ctx, string title, int selectedPosition, Action<int, string> selectionMade, params string[] items)
        {
            return new AlertDialog.Builder(ctx)
                .SetTitle(title)
                .SetSingleChoiceItems(items, selectedPosition, (s, e) =>
                {
                    if (e.Which >= items.Length)
                        return;

                    var item = items[e.Which];
                    selectionMade?.Invoke(e.Which, item);

                    ((AlertDialog)s).Dismiss();
                })
                .SetNeutralButton("Cancel", new DialogCloseOnClickListener())
                .Create();
        }

        public static AlertDialog MultiSelect<T>(Context ctx, string title, Action<IEnumerable<MultiSelectItem<T>>> selectionComplete, params MultiSelectItem<T>[] items)
        {
            var names = new List<string>();
            var isChecked = new List<bool>();

            foreach (var item in items)
            {
                names.Add(item.Text);
                isChecked.Add(item.IsSelected);
            }

            return new AlertDialog.Builder(ctx)
                .SetTitle(title)
                .SetMultiChoiceItems(names.ToArray(), isChecked.ToArray(), (s, e) =>
                {
                    var item = items[e.Which];
                    item.IsSelected = e.IsChecked;
                })
                .SetNeutralButton("Cancel", new DialogCloseOnClickListener())
                .SetPositiveButton("Ok", (s, e) =>
                {
                    selectionComplete?.Invoke(items.Where(x => x.IsSelected));
                })
                .Create();
        }

        public static ProgressDialog IndeterminateProgress(Context ctx, string message, string title = null)
        {
            var progressSpinner = new ProgressDialog(ctx)
            {
                Indeterminate = true
            };
            progressSpinner.SetCancelable(false);
            progressSpinner.SetCanceledOnTouchOutside(false);
            progressSpinner.SetMessage(message);

            if (!string.IsNullOrWhiteSpace(title))
            {
                progressSpinner.SetTitle(title);
            }

            return progressSpinner;
        }

        public static DatePickerDialog PickDate(Context ctx, DateTime localDate, Action<DateTime> onDateSelected)
        {
            return new DatePickerDialog(ctx, (s, e) => onDateSelected(e.Date), localDate.Year, localDate.Month - 1, localDate.Day);
        }

        public static TimePickerDialog PickTime(Context ctx, TimeSpan time, Action<TimeSpan> onTimeChosen)
        {
            return new TimePickerDialog(ctx, (s, e) => onTimeChosen(new TimeSpan(e.HourOfDay, e.Minute, 0)), time.Hours, time.Minutes, false);
        }

        public class ActionSheetItem
        {
            public int Icon { get; set; }
            public string Text { get; set; }
            public Action Callback { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        public class MultiSelectItem<T>
        {
            public string Text { get; set; }
            public bool IsSelected { get; set; }
            public T Value { get; set; }
        }

        class DialogCloseOnClickListener : Java.Lang.Object, IDialogInterfaceOnClickListener, IDialogInterfaceOnCancelListener
        {
            public void OnCancel(IDialogInterface dialog)
            {
                dialog.Dismiss();
            }

            public void OnClick(IDialogInterface dialog, int which)
            {
                dialog.Dismiss();
            }
        }
    }
}