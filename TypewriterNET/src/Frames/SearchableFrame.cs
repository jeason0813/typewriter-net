using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MulticaretEditor;

namespace TypewriterNET.Frames
{
	public interface SearchableFrame
	{
		MulticaretTextBox TextBox { get; }

		void AddSearchPanel(Control control);

		void RemoveSearchPanel(Control control);
	}
}
