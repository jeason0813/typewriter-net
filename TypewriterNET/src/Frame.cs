using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;
using MulticaretEditor.KeyMapping;
using MulticaretEditor.Highlighting;
using MulticaretEditor;

public class Frame : AFrame
{
	private TabBar<string> tabBar;
	private SplitLine splitLine;
	private MulticaretTextBox textBox;

	public Frame(string name)
	{
		Name = name;

		SwitchList<string> list = new SwitchList<string>();
		list.Add("File 1");
		list.Add("File 2");
		tabBar = new TabBar<string>(list, TabBar<string>.DefaultStringOf);
		tabBar.Text = name;
		Controls.Add(tabBar);

		splitLine = new SplitLine();
		Controls.Add(splitLine);

		textBox = new MulticaretTextBox();
		textBox.FocusedChange += OnTextBoxFocusedChange;
		Controls.Add(textBox);

		InitResizing(tabBar, splitLine);
		tabBar.MouseDown += OnTabBarMouseDown;
	}

	override public Size MinSize { get { return new Size(tabBar.Height * 3, tabBar.Height); } }

	private void OnTabBarMouseDown(object sender, EventArgs e)
	{
		textBox.Focus();
	}

	private void OnTextBoxFocusedChange()
	{
		tabBar.Selected = textBox.Focused;
	}

	public string Title
	{
		get { return tabBar.Text; }
		set { tabBar.Text = value; }
	}

	override protected void OnResize(EventArgs e)
	{
		base.OnResize(e);
		int tabBarHeight = tabBar.Height;
		tabBar.Size = new Size(Width, tabBarHeight);
		splitLine.Location = new Point(Width - 10, tabBarHeight);
		splitLine.Size = new Size(10, Height - tabBarHeight);
		textBox.Location = new Point(0, tabBarHeight);
		textBox.Size = new Size(Width - 10, Height - tabBarHeight);
	}
}
