using System;
using UIKit;

namespace ProspectManagement.iOS
{
    public class LinkerPleaseInclude
    {
        public void Include(UITextField textField)
        {
            textField.Text = textField.Text + "";
            textField.EditingChanged += (sender, args) => { textField.Text = ""; };
        }

        public void Include(UITextView textView)
        {
            textView.Text = textView.Text + "";
            textView.Changed += (sender, args) => { textView.Text = ""; }; 
            textView.TextStorage.DidProcessEditing += (sender, e) => textView.Text = "";
        }
    }
}
