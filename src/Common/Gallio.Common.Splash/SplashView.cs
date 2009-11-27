using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Gallio.Common.Splash.Internal;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// A SplashView is a custom control to display styled text intermixed with
    /// embedded content such as images.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This control is optimized for high-performance append-only text output such as
    /// scrolling consoles.  Consequently, the control does not support editing or
    /// modifications to currently displayed content (besides clearing it).
    /// </para>
    /// <para>
    /// Display updates are performed asynchronously in batches.
    /// </para>
    /// </remarks>
    public unsafe class SplashView : ScrollableControl
    {
        private readonly SplashDocument document;
        private readonly SplashLayout layout;

        private int minimumTextLayoutWidth = 100;

        /// <summary>
        /// Creates an empty SplashView.
        /// </summary>
        public SplashView()
        {
            document = new SplashDocument();
            layout = new SplashLayout(document);

            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw
                | ControlStyles.Selectable | ControlStyles.UserMouse
                | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            BackColor = SystemColors.Window;
            Padding = new Padding(5);

            AttachLayoutEvents();
        }

        private void AttachLayoutEvents()
        {
            layout.UpdateRequired += HandleLayoutChanged;
        }

        private void HandleLayoutChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        /// <summary>
        /// Gets or sets the minimum width of the text area.
        /// </summary>
        /// <remarks>
        /// If the control is resized to less than this width, then the control will automatically
        /// display a horizontal scrollbar.
        /// </remarks>
        public int MinimumTextLayoutWidth
        {
            get { return minimumTextLayoutWidth; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", "Value must be at least 1.");

                if (minimumTextLayoutWidth != value)
                {
                    minimumTextLayoutWidth = value;
                    UpdateLayoutSize();
                    UpdateScrollBars();
                }
            }
        }

        /// <summary>
        /// Clears the text in the document.
        /// </summary>
        public void Clear()
        {
            document.Clear();
        }

        /// <summary>
        /// Appends text to the document.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="text">The text to append.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="style"/> or <paramref name="text"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if more than <see cref="SplashDocument.MaxStyles" /> distinct styles are used.</exception>
        public void AppendText(Style style, string text)
        {
            document.AppendText(style, text);
        }

        /// <summary>
        /// Appends a new line to the document.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="style"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if more than <see cref="SplashDocument.MaxStyles" /> distinct styles are used.</exception>
        public void AppendLine(Style style)
        {
            document.AppendLine(style);
        }

        /// <summary>
        /// Appends an object to the document.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="obj">The object to append.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="style"/> or <paramref name="obj"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if more than <see cref="SplashDocument.MaxStyles" /> distinct styles are used.</exception>
        public void AppendObject(Style style, EmbeddedObject obj)
        {
            document.AppendObject(style, obj);
        }

        /// <summary>
        /// Gets a character snap from a position.
        /// </summary>
        /// <param name="point">The point relative to the layout origin.</param>
        /// <returns>The character snap.</returns>
        public CharSnap GetCharSnapFromPosition(Point point)
        {
            Rectangle displayRect = DisplayRectangle;
            Point layoutPoint = new Point(point.X - displayRect.Left, point.Y - displayRect.Top);
            return layout.GetCharSnapFromPosition(layoutPoint, Padding);
        }

        /// <inheritdoc />
        protected override void OnPaint(PaintEventArgs e)
        {
            UpdateLayout();

            Rectangle displayRect = DisplayRectangle;
            layout.Paint(e.Graphics, displayRect.Location, e.ClipRectangle);

            base.OnPaint(e);
        }

        /// <inheritdoc />
        protected override void OnPaddingChanged(EventArgs e)
        {
            UpdateLayoutSize();

            base.OnPaddingChanged(e);
        }

        /// <inheritdoc />
        protected override void OnResize(EventArgs e)
        {
            UpdateLayoutSize();

            base.OnResize(e);
        }

        /// <inheritdoc />
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            UpdateLayoutRightToLeft();

            base.OnRightToLeftChanged(e);
        }

        private void UpdateLayout()
        {
            UpdateLayoutSize();
            UpdateLayoutRightToLeft();
            layout.Update();
            UpdateScrollBars();
        }

        private void UpdateLayoutSize()
        {
            Rectangle displayRect = DisplayRectangle;
            layout.DesiredLayoutWidth = Math.Max(displayRect.Width, minimumTextLayoutWidth);
        }

        private void UpdateLayoutRightToLeft()
        {
            layout.DesiredLayoutRightToLeft = RightToLeft == RightToLeft.Yes;
        }

        private void UpdateScrollBars()
        {
            AutoScrollMinSize = new Size(minimumTextLayoutWidth, layout.CurrentLayoutHeight);
        }
    }
}
