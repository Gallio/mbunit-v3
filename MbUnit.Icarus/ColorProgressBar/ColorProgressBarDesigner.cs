using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ColorProgressBar
{
	internal class ColorProgressBarDesigner : System.Windows.Forms.Design.ControlDesigner
	{
		public ColorProgressBarDesigner()
		{}

		// clean up some unnecessary properties
		protected override void PostFilterProperties(IDictionary Properties)
		{
			Properties.Remove("AllowDrop");
			Properties.Remove("BackgroundImage");
			Properties.Remove("ContextMenu");
			Properties.Remove("FlatStyle");
			Properties.Remove("Image");
			Properties.Remove("ImageAlign");
			Properties.Remove("ImageIndex");
			Properties.Remove("ImageList");
			Properties.Remove("Text");
			Properties.Remove("TextAlign");
		}
	}
}