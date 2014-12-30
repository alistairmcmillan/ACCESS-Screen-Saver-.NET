using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ScreenSaver.Properties;
using System.Drawing.Imaging;

namespace AlistairMcMillan.ACCESSScreenSaver
{
    partial class ScreenSaverForm : Form
    {
        static Timer myTimer = new Timer();

        // Set starting opacities for images (starts with "trust" onscreen, "excel" about to appear, the others off screen)
        float excelOpacity = 0.0f;
        float communicationOpacity = -4.0f;
        float innovationOpacity = -3.0f;
        float trustOpacity = 1.0f;

        // Set direction of fading (starts with "excel" and "communication" fading in and "innovation" and "trust" fading out)
        bool increasingExcelOpacity = true;
        bool increasingCommunicationOpacity = true;
        bool increasingInnovationOpacity = false;
        bool increasingTrustOpacity = false;

        // Keep track of whether the screensaver has become active.
        private bool isActive;

        // Keep track of the location of the mouse
        private Point mouseLocation;

        public ScreenSaverForm()
            : this(false)
        {
        }

        public ScreenSaverForm(bool debugmode)
        {
            InitializeComponent();

            // In debug mode, allow minimize after breakpoint 
            if (debugmode)
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }

            SetupScreenSaver();
        }

        /// <summary>
        /// Set up the main form as a full screen screensaver.
        /// </summary>
        private void SetupScreenSaver()
        {
            // Use double buffering to improve drawing performance
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            // Capture the mouse
            this.Capture = true;

            // Set the application to full screen mode and hide the mouse
            Cursor.Hide();
            Bounds = Screen.PrimaryScreen.Bounds;
            WindowState = FormWindowState.Maximized;
            TopMost = true;

            ShowInTaskbar = false;
            DoubleBuffered = true;
            BackgroundImageLayout = ImageLayout.Stretch;

            myTimer.Tick += new EventHandler(FadeTimer_Tick);
            myTimer.Interval = 1000/30; // 1/30.0
            myTimer.Start();
        }

        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            // Set IsActive and MouseLocation only the first time this event is called.
            if (!isActive)
            {
                mouseLocation = MousePosition;
                isActive = true;
            }
            else
            {
                // If the mouse has moved significantly since first call, close.
                if ((Math.Abs(MousePosition.X - mouseLocation.X) > 10) ||
                    (Math.Abs(MousePosition.Y - mouseLocation.Y) > 10))
                {
                    Close();
                }
            }
        }

        private void ScreenSaverForm_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }

        private void ScreenSaverForm_MouseDown(object sender, MouseEventArgs e)
        {
            Close();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Draw the current background image stretched to fill the full screen
            Image backgroundImage = (Image)Resources.ResourceManager.GetObject("background");
            if (backgroundImage != null && e != null)
            {
                e.Graphics.DrawImage(backgroundImage, 0, 0, Size.Width, Size.Height);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Compare size of background image and screen size to determine ratio for scaling other images
            Image backgroundImage = (Image)Resources.ResourceManager.GetObject("background");
            float ratio = (float)Size.Height / (float)backgroundImage.Height;

            ColorMatrix matrix = new ColorMatrix();
            ImageAttributes attributes = new ImageAttributes();
            try {
                // Draw company logo
                Image accessImage = (Image)Resources.ResourceManager.GetObject("ACCESS");
                if (accessImage != null && e != null)
                {
                    float dstWidth = Size.Width / 4;
                    float dstHeight = dstWidth * ((float)accessImage.Size.Height / (float)accessImage.Size.Width);
                    e.Graphics.DrawImage(accessImage, 
                        Size.Width - dstWidth - (Size.Height / 20), 
                        (Size.Height /20),
                        dstWidth, 
                        dstHeight);
                }

                // Draw "we enable our people to excel"
                Image excelImage = (Image)Resources.ResourceManager.GetObject("excel");
                if (excelImage != null && e != null)
                {
                    float dstWidth = excelImage.Width * ratio * (float)0.9;
                    float dstHeight = excelImage.Height * ratio * (float)0.9;
                    matrix.Matrix33 = excelOpacity;
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    Rectangle destRect = new Rectangle((int)(Size.Width / 2 - dstWidth / 2), (int)(Size.Height / 2 - dstHeight / 2), (int)dstWidth, (int)dstHeight);
                    e.Graphics.DrawImage(excelImage, destRect, 0, 0, excelImage.Width, excelImage.Height, GraphicsUnit.Pixel, attributes);
                }

                // Draw "honest and open communication"
                Image communicationImage = (Image)Resources.ResourceManager.GetObject("communication");
                if (communicationImage != null && e != null)
                {
                    float dstWidth = communicationImage.Width * ratio * (float)0.9;
                    float dstHeight = communicationImage.Height * ratio * (float)0.9;
                    matrix.Matrix33 = communicationOpacity;
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    Rectangle destRect = new Rectangle((int)(Size.Width / 2 - dstWidth / 2), (int)(Size.Height / 2 - dstHeight / 2), (int)dstWidth, (int)dstHeight);
                    e.Graphics.DrawImage(communicationImage, destRect, 0, 0, communicationImage.Width, communicationImage.Height, GraphicsUnit.Pixel, attributes);
                }

                // Draw "we encourage innovation"
                Image innovationImage = (Image)Resources.ResourceManager.GetObject("innovation");
                if (innovationImage != null && e != null)
                {
                    float dstWidth = innovationImage.Width * ratio * (float)0.9;
                    float dstHeight = innovationImage.Height * ratio * (float)0.9;
                    matrix.Matrix33 = innovationOpacity;
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    Rectangle destRect = new Rectangle((int)(Size.Width / 2 - dstWidth / 2), (int)(Size.Height / 2 - dstHeight / 2), (int)dstWidth, (int)dstHeight);
                    e.Graphics.DrawImage(innovationImage, destRect, 0, 0, innovationImage.Width, innovationImage.Height, GraphicsUnit.Pixel, attributes);
                }

                // Draw "trust and respect"
                Image trustImage = (Image)Resources.ResourceManager.GetObject("trust");
                if (trustImage != null && e != null)
                {
                    float dstWidth = trustImage.Width * ratio * (float)0.9;
                    float dstHeight = trustImage.Height * ratio * (float)0.9;
                    matrix.Matrix33 = trustOpacity;
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    Rectangle destRect = new Rectangle((int)(Size.Width / 2 - dstWidth / 2), (int)(Size.Height / 2 - dstHeight / 2), (int)dstWidth, (int)dstHeight);
                    e.Graphics.DrawImage(trustImage, destRect, 0, 0, trustImage.Width, trustImage.Height, GraphicsUnit.Pixel, attributes);
                }
            } finally {
                attributes.Dispose();
            }
        }

        void UpdateOpacity(ref bool increasing, ref float opacity)
        {
            if (opacity > 2.5)
            {
                increasing = false;
            }

            if (opacity < -5.5)
            {
                increasing = true;
            }

            if (increasing)
            {
                opacity += 0.1f;
            }
            else
            {
                opacity -= 0.1f;
            }
        }

        void FadeTimer_Tick(object sender, EventArgs e)
        {
            UpdateOpacity(ref increasingCommunicationOpacity, ref communicationOpacity);
            UpdateOpacity(ref increasingExcelOpacity, ref excelOpacity);
            UpdateOpacity(ref increasingInnovationOpacity, ref innovationOpacity);
            UpdateOpacity(ref increasingTrustOpacity, ref trustOpacity);
            this.Invalidate();
        }

    }

}
