using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

using MbUnit.Core.Reports.Serialization;

namespace ColorProgressBar
{
	[Description("Color Progress Bar")]
	[ToolboxBitmap(typeof(ProgressBar))]
	[Designer(typeof(ColorProgressBarDesigner))]
	public class ColorProgressBar : System.Windows.Forms.Control
	{	
	
		//
		// set default values
		//
		private int _Value = 0;
		private int _Minimum = 0;
		private int _Maximum = 100;
		private int _Step = 10;

		private FillStyles _FillStyle = FillStyles.Dashed;

		private Color _BarColor = Color.FromArgb(255, 128, 128);
		private Color _BorderColor = Color.Black;
        private Color textColor = Color.Black;
        private Font textFont = new Font("Verdana", 8);

        private double testDuration = 0;
        private ReportCounter counter = new ReportCounter();

		public enum FillStyles
		{
			Solid,
			Dashed
		}

		public ColorProgressBar()
		{
			base.Size = new Size(150, 15);
			SetStyle(
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.DoubleBuffer,
				true
				);
		}

		[Description( "ColorProgressBar color")]
		[Category( "ColorProgressBar" )]
		public Color BarColor
		{
			get
			{
				return _BarColor;
			}
			set
			{
				_BarColor = value;
				this.Invalidate();
			}
		}

		[Description( "ColorProgressBar fill style")]
		[Category( "ColorProgressBar" )]
		public FillStyles FillStyle
		{
			get
			{
				return _FillStyle;
			}
			set
			{
				_FillStyle = value;
				this.Invalidate();
			}
		}

		[Description( "The current value for the ColorProgressBar, "+
			 "in the range specified by the Minimum and Maximum properties." )]
		[Category( "ColorProgressBar" )]
		// the rest of the Properties windows must be updated when this peroperty is changed.
		[RefreshProperties(RefreshProperties.All)]
		public int Value
		{
			get
			{
				return _Value;
			}
			set
			{
				if (value < _Minimum)
				{
					throw new ArgumentException("'"+value+"' is not a valid value for 'Value'.\n"+
						"'Value' must be between 'Minimum' and 'Maximum'.");
				}

				if (value > _Maximum)
				{
					throw new ArgumentException("'"+value+"' is not a valid value for 'Value'.\n"+
						"'Value' must be between 'Minimum' and 'Maximum'.");
				}

				_Value = value;			
				this.Invalidate();
			}
		}
		
		[Description("The lower bound of the range this ColorProgressbar is working with.")]
		[Category("ColorProgressBar")]
		[RefreshProperties(RefreshProperties.All)]
		public int Minimum
		{
			get
			{
				return _Minimum;
			}
			set
			{
				_Minimum = value;

				if (_Minimum > _Maximum)
					_Maximum = _Minimum;
				if (_Minimum > _Value)
					_Value = _Minimum;

				this.Invalidate();
			}
		}

		[Description("The uppper bound of the range this ColorProgressbar is working with.")]
		[Category("ColorProgressBar")]
		[RefreshProperties(RefreshProperties.All)]
		public int Maximum
		{
			get
			{
				return _Maximum;
			}
			set
			{
				_Maximum = value;

				if (_Maximum < _Value)
					_Value = _Maximum;
				if (_Maximum < _Minimum)
					_Minimum = _Maximum;

				this.Invalidate();
			}
		}

		[Description("The amount to jump the current value of the control by when the Step() method is called.")]
		[Category("ColorProgressBar")]		
		public int Step
		{
			get
			{
				return _Step;
			}
			set
			{
				_Step = value;
				this.Invalidate();
			}
		}

		[Description("The border color of ColorProgressBar")]
		[Category("ColorProgressBar")]		
		public Color BorderColor
		{
			get
			{
				return _BorderColor;
			}
			set
			{
				_BorderColor = value;
				this.Invalidate();
			}
		}
		
		//
		// Call the PerformStep() method to increase the value displayed by the amount set in the Step property
		//
		public void PerformStep()
		{
			if (_Value < _Maximum)
				_Value += _Step;
			else
				_Value = _Maximum;

			this.Invalidate();
		}
		
		//
		// Call the PerformStepBack() method to decrease the value displayed by the amount set in the Step property
		//
		public void PerformStepBack()
		{
			if (_Value > _Minimum)
				_Value -= _Step;
			else
				_Value = _Minimum;

			this.Invalidate();
		}

		//
		// Call the Increment() method to increase the value displayed by an integer you specify
		// 
		public void Increment(int value)
		{
			if (_Value < _Maximum)
				_Value += value;
			else
				_Value = _Maximum;

			this.Invalidate();
		}
		
		//
		// Call the Decrement() method to decrease the value displayed by an integer you specify
		// 
		public void Decrement(int value)
		{
			if (_Value > _Minimum)
				_Value -= value;
			else
				_Value = _Minimum;

			this.Invalidate();
		}

        public ReportCounter Counter
        {
            get
            {
                return this.counter;
            }
        }
                

        public double TestDuration
        {
            get { return testDuration; }
            set { testDuration = value; }
        }
	
		
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			//
			// Calculate matching colors
			//
			Color darkColor = ControlPaint.Dark(_BarColor);
			Color bgColor = Color.White;//ControlPaint.Dark(_BarColor);

            int alpha = 200;

			//
			// Fill background
			//
			SolidBrush bgBrush = new SolidBrush(bgColor);

			// The region of the progress bar!
			int leftbar = 1;
			int topbar = 1;
			int X = this.Width-1;
			int Y = this.Height-1;
			Point[] points = {   new Point(leftbar + 2, topbar), 
								 new Point(X-2, topbar), 
								 new Point(X-1, topbar + 1), 
								 new Point(X, topbar + 2), 
								 new Point(X, Y-3), 
								 new Point(X-1, Y-2), 
								 new Point(X-2, Y-1), 
								 new Point(leftbar + 2, Y), 
								 new Point(leftbar + 1, Y-2), 
								 new Point(leftbar, Y-3), 
								 new Point(leftbar, topbar + 2),
								 new Point(leftbar + 1, topbar + 1),
			};    
			GraphicsPath path = new GraphicsPath();
			path.AddLines(points);

			Region reg = new Region(path);
			e.Graphics.FillRegion(Brushes.White, reg); //bgBrush
			bgBrush.Dispose();
			
			// 
			// Check for value
			//
			if (_Maximum == _Minimum || _Value == 0)
			{
				// Draw border only and exit;
				drawBorder(e.Graphics);
				return;
			}

			//
			// The following is the width of the bar. This will vary with each value.
			//
			int fillWidth = (this.Width * _Value) / (_Maximum - _Minimum);
			
			//
			// GDI+ doesn't like rectangles 0px wide or high
			//
			if (fillWidth == 0)
			{
				// Draw border only and exit;
				drawBorder(e.Graphics);
				return;
			}

			//Make the bars of the progress complete, just like XP bars
			if(fillWidth%8 != 0)
			{
				int rest = fillWidth % 8;
				fillWidth += (8-rest);
			}

			//
			// Rectangles for upper and lower half of bar
			//
			Rectangle chunkbar = new Rectangle(3,2, fillWidth, this.Height-4);
			//Rectangle topRect = new Rectangle(0, 0, fillWidth, this.Height / 2);
			//Rectangle buttomRect = new Rectangle(0, this.Height / 2, fillWidth, this.Height / 2);

			//
			// The gradient brush
			//
			LinearGradientBrush brush;			

			//
			// Paint upper half
			//
			brush = new LinearGradientBrush(chunkbar, Color.White, _BarColor, 90.0f);
			float[] relativeIntensities = {0.1f, 1.0f, 1.0f, 1.0f, 1.0f, 0.85f, 0.1f}; 
			float[] relativePositions =   {0.0f, 0.2f, 0.5f, 0.5f, 0.5f, 0.8f, 1.0f};

			// create a Blend object and assign it to silverBrush07
			Blend blend = new Blend();
			blend.Factors = relativeIntensities;
			blend.Positions = relativePositions; 
			brush.Blend = blend;

			e.Graphics.FillRectangle(brush, chunkbar);
			brush.Dispose();

			//
			// Calculate separator's setting
			//
			// Separator is made smaller like the XP progress
			int sepWidth = 8;//(int)(this.Height * .67);
			int sepCount = (int)(fillWidth / sepWidth);
			//Color sepColor = ControlPaint.LightLight(_BarColor);
			Color sepColor = Color.White;//ControlPaint.Dark(_BarColor);

			//
			// Paint separators
			//
			switch (_FillStyle)
			{
				case FillStyles.Dashed:
					// Draw each separator line
					for (int i = 0; i <= sepCount; i++)
					{
						e.Graphics.DrawLine(new Pen(sepColor, 2),
							sepWidth * i+3, 0, sepWidth * i+3, this.Height);
					}
					break;

				case FillStyles.Solid:
					// Draw nothing
					break;

				default:
					break;
			}

			//
			// Draw border and exit
			//
			drawBorder(e.Graphics);

            //text addtion added by Andy Stopford
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            string text = string.Format(
                  "{0} tests - {1} successes - {2} failures  - {3} skipped - {4} ignored - {5:0.0}s",
                  this.Counter.RunCount,
                  this.Counter.SuccessCount,
                  this.Counter.FailureCount,
                  this.Counter.SkipCount,
                  this.Counter.IgnoreCount,
                  this.TestDuration
                  );

            e.Graphics.DrawString(
                text,
                this.textFont,
                new SolidBrush(FromColor(this.textColor,alpha)),
                this.ClientRectangle.Left + this.ClientRectangle.Width / 2,
                this.ClientRectangle.Top + this.ClientRectangle.Height / 2,
                format
                );
		}

		//
		// Draw border
		//
		protected void drawBorder(Graphics g)
		{
			// additions by DDISoft
			int X = this.ClientRectangle.Width;
			int Y = this.ClientRectangle.Height;
			Point[] points = {   new Point(1, 2), 
								 new Point(2, 1), 
								 new Point(3, 0), 
								 new Point(X-4, 0), 
								 new Point(X-3, 1), 
								 new Point(X-2, 2), 
								 new Point(X-2, Y-3), 
								 new Point(X-3, Y-2), 
								 new Point(X-4, Y-1), 
								 new Point(3, Y-1), 
								 new Point(2, Y-2),
								 new Point(1, Y-3),
								 new Point(1, 2), 
			};  
      
			Point[] points2 = {  new Point(2, 2), 
								 new Point(3, 1), 
								 new Point(4, 1), 
								 new Point(X-5, 1), 
								 new Point(X-4, 1), 
								 new Point(X-3, 2), 
			};  
		
			g.DrawCurve(new Pen(Brushes.Gray, 1), points, 0);
			g.DrawCurve(new Pen(Brushes.LightGray, 1), points2, 0);
			g.DrawLine(new Pen(Brushes.LightGray, 1),2,2,2,Y-3);

			//Old original code
			/*Rectangle borderRect = new Rectangle(0, 0,
				ClientRectangle.Width - 1, ClientRectangle.Height - 1);
			g.DrawRectangle(new Pen(_BorderColor, 1), borderRect);*/
		}

        private Color FromColor(Color c, int alpha)
        {
            return Color.FromArgb(
                alpha,
                c.R,
                c.G,
                c.B
                );
        }

	}
}