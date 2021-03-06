﻿using System;
using System.Windows.Forms;
using ACT_FFXIV_Kapture.Resource;

namespace ACT_FFXIV_Kapture.Plugin
{
	public partial class AboutView : UserControl
	{
		public AboutView()
		{
			InitializeComponent();

			about_TitleLabel.Text = Strings.KaptureLootTracker;

			about_AuthorKeyLabel.Text = Strings.Author;
			about_AuthorValueLabel.Text = Strings.Kalilistic;
			about_AuthorValueLabel.Tag = "https://github.com/kalilistic";

			about_SupportKeyLabel.Text = Strings.Support;
			about_SupportValueLabel.Text = Strings.Discord;
			about_SupportValueLabel.Tag = "https://discord.gg/ahFKcmx";

			about_SourceKeyLabel.Text = Strings.Source;
			about_SourceValueLabel.Text = Strings.Github;
			about_SourceValueLabel.Tag = "https://github.com/kalilistic/ACT.Kapture";

			about_LicenseKeyLabel.Text = Strings.License;
			about_LicenseValueLabel.Text = Strings.MIT;
			about_LicenseValueLabel.Tag = "https://github.com/kalilistic/Kapture/blob/master/LICENSE";

			about_AuthorValueLabel.Click += Link_Click;
			about_SupportValueLabel.Click += Link_Click;
			about_SourceValueLabel.Click += Link_Click;
			about_LicenseValueLabel.Click += Link_Click;
		}
		
		public event EventHandler<string> LinkClicked;

		private void Link_Click(object sender, EventArgs e)
		{
			LinkClicked?.Invoke(this, ((Label) sender).Tag.ToString());
		}
	}
}