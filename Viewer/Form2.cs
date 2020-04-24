using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Timers;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using System.Drawing.Imaging;

namespace Viewer
{
    public partial class Form2 : Form
    {
        //Make lists to use for this window
        List<SoundTrack> SoundTracksToPlay;
        List<Slide> SlidesToPlay;

        //Variable Declaration
        private MediaPlayer currentPlayer = new MediaPlayer();
        private bool musicPlaying = false;
        private readonly object _lock = new object();
        private Dispatcher myDispatcher;
        private int tracksPlayed = 0;
        private int pausedTrack = 0; //holds index of paused track
        private int numTracksToPlay = 0;
        private int totalDuration = 0;
        private double totalElapsedTime = 0;
        private double pausedTime; //holds location in track during pause
        private int totalMinuteDuration = 0;
        private int totalSecondDuration = 0;
        //private int slideCount = 0;
        //private int currentSlideIndex = 0;
        private BackgroundWorker bgw = new BackgroundWorker(); //This is our worker that plays the musics and updates the progress bar
        private bool paused = false; //used to determine whether slideshow status is paused or playing
        private bool firstPlayPress = true; //to diffirentiate pause/resume from intial play click
        private bool resumeMusic = false;
        //private Slide currentSlide;
        //private Slide nextSlide;

        //Jareds slide transition vars
        private int slideListIndex =0;

        private float Opacity = 1.0f;


        public Form2(List<SoundTrack> ListOfTracks, List<Slide> ListOfSlides)
        {
            InitializeComponent();

            //used for handling the playing of multiple tracks in succession
            myDispatcher = Dispatcher.CurrentDispatcher;
            currentPlayer.MediaFailed += (s, e) => { Console.WriteLine("Media Failed to play...\n"); };

            // set up the background worker to DoWork and update progress when it's called to run Async
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork); // This plays the song and updates the progress
            bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgw.WorkerReportsProgress = true;
            bgw.WorkerSupportsCancellation = true;

            //udpate the lists for this window with what was given in window creation
            SoundTracksToPlay = ListOfTracks;
            SlidesToPlay = ListOfSlides;
            this.FormClosed += Close_Stop;
            progressBar.ForeColor = System.Drawing.Color.DeepPink;

            slideTimer.Tick += DoTransition;
            ////timer setup
            //slideTransitionTimer.Tick += slideTransitionTimer_Tick;


        }

        private void Close_Stop(object sender, FormClosedEventArgs e)
        {
            currentPlayer.Stop();
            bgw.CancelAsync();
            this.Dispose();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

       
        private void playAndPauseButton_Click(object sender, EventArgs e)
        {
            //int opacity = 50;

            if (firstPlayPress)
            {

                topPictureBox.BackColor = System.Drawing.Color.Black;
                Slide firstSlide = SlidesToPlay.ElementAt(slideListIndex);
                Image newImage = new Bitmap(firstSlide.Path);
                //topPictureBox.Image = newImage;
                topPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                slideTimer.Interval = SlidesToPlay[0].Duration * 1000;
                slideTimer.Enabled = true;
                slideTimer.Start();

                //topPictureBox.Image = newImage;

                sourceBmp1 = (Bitmap)Image.FromStream(new MemoryStream(File.ReadAllBytes(firstSlide.Path)));
                sourceBmp2 = new Bitmap(100,100);
                using (Graphics gfx = Graphics.FromImage(sourceBmp2))
                using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0)))
                {
                    gfx.FillRectangle(brush, 0, 0, 100, 100);
                }

                fadeBmp1?.Dispose();
                fadeBmp2?.Dispose();
                fadeBmp1 = sourceBmp1.Clone() as Bitmap;
                fadeBmp2 = sourceBmp2.Clone() as Bitmap;
                topPictureBox.Invalidate();
                transitionTimer.Interval = 200;
                if(SlidesToPlay[0].transitionType == transitionTypes.crossFade)
                {
                    transitionTimer.Tick += DoFade;
                    transitionTimer.Start();
                }
                // do other transitions on start
            }

            //if button not pressed yet or music finished entirely, start play sequence (ie. first time click)
            if (musicPlaying == false && !bgw.IsBusy && paused == false)
            {
                totalElapsedTime = 0;
                totalDuration = 0;
                musicPlaying = true;
                firstPlayPress = true;
                paused = false;
                numTracksToPlay = SoundTracksToPlay.Count;

                //Set up functionality for slides and timers
                //get size of slideList
                //slideCount = SlidesToPlay.Count();
                //currentSlideIndex = 0;
                //currentSlide = SlidesToPlay[currentSlideIndex];
                //nextSlide = SlidesToPlay[currentSlideIndex + 1];

                //changeSlides();

                ///////////////////////////////////////new slide timer stuff from Jared///////////
                ///
                slideTimer.Interval = SlidesToPlay[slideListIndex].Duration * 1000;// convert to seconds
                slideTimer.Enabled = true;
                slideTimer.Start();


                //calculate total duration for progress bar
                foreach (SoundTrack track in SoundTracksToPlay)
                {
                    totalDuration = totalDuration + track.Duration;
                }

                UpdateTotalTime(totalDuration);//update minutes and secs for labels

                progressBar.Value = 0;
                progressBar.Maximum = totalDuration;//give an extra second buffer in case of threading issues

                //start music playing thread in background
                bgw.RunWorkerAsync();
            }
            else if (musicPlaying == false && paused == true) //resuming from puased state
            {
                currentPlayer.Play();
                musicPlaying = true;
                paused = false;
                resumeMusic = true;
                slideTimer.Start();
                //start music playing thread again
                bgw.RunWorkerAsync();
            }
            else //currently playing, so pause sequence
            {
                currentPlayer.Pause();
                musicPlaying = false;
                paused = true;
                resumeMusic = false;
                bgw.CancelAsync();
                slideTimer.Stop();

                //reset progess bar at end
                //progressBar.Value = 0;
                //progressBar.Update();
                // reset tracks played
                //tracksPlayed = 0;
                // reset duration
                //totalDuration = 0;
                //numTracksToPlay = 0;
            }
        }

        // Run async so the music can play and the progress bar update so the UI thread isn't
        // 100% consumed by the while loop updating the progress bar
        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (firstPlayPress) //play start
            {
                firstPlayPress = false; 
                var timeElapsed = 0.0;

                // iterate through the list of tracks chosen
                for (tracksPlayed = 0; tracksPlayed < numTracksToPlay; tracksPlayed++)
                {
                    var currentTime = 0.0; // in seconds
                    SoundTrack currentTrack = SoundTracksToPlay.ElementAt(tracksPlayed);
                    string currentPath = currentTrack.Path;

                    // Since we're on a background thread doing UI stuff, we need to dispatch
                    // out work to the currentPlayer because it's on the main thread
                    myDispatcher.Invoke(() =>
                    {
                        PlayMusic(currentPlayer, currentPath, paused);
                    }
                    );

                    // update the progressbar until the current track is complete
                    while (currentTime < currentTrack.Duration)
                    {
                        if (bgw.CancellationPending != true)
                        {
                            Thread.Sleep(50);
                            currentTime += 0.05;
                            pausedTime = currentTime;//in case of pause
                        }

                        //still playing, stay here
                        bgw.ReportProgress((int)(timeElapsed + currentTime));

                        // If the user has hit the button a second time and chosen to end playing of music
                        // then we need to check in the loop and return if so
                        //NEW PAUSE: if hit second time, then wait for unpause
                        if (bgw.CancellationPending == true)
                        {
                            pausedTrack = tracksPlayed; //in case of puase issued
                            e.Cancel = true;
                            return; //pause ended
                        }
                    }
                    timeElapsed += currentTime;
                    totalElapsedTime = timeElapsed; //in case of pause issued
                }
                // we've finished playing
                musicPlaying = false;
            }
            else //play resume/ continue playing under resume
            {
                Console.WriteLine(pausedTrack);
                var timeElapsed = 0.0;

                if (resumeMusic == true)//if called under resume
                {
                    timeElapsed = totalElapsedTime;
                }
                
                // iterate through the list of tracks chosen (starting at paused track)
                for (tracksPlayed = pausedTrack; tracksPlayed < numTracksToPlay; tracksPlayed++)
                {
                    var currentTime = 0.0;
                    SoundTrack currentTrack = SoundTracksToPlay.ElementAt(tracksPlayed);
                    string currentPath = currentTrack.Path;

                    //if this is a resume, get location in track before pause and skip invocation of playing next track
                    if (resumeMusic == true)
                    {
                        currentTime = pausedTime; // since this is a resume
                        resumeMusic = false; //reset resume
                    }
                    else //skip if a resume, else start playing the track
                    {
                        //Since we're on a background thread doing UI stuff, we need to dispatch
                        // out work to the currentPlayer because it's on the main thread
                        myDispatcher.Invoke(() =>
                        {
                            PlayMusic(currentPlayer, currentPath, paused);
                        }
                        );
                    }

                    // update the progressbar until the current track is complete
                    while (currentTime < currentTrack.Duration)
                    {
                        if (bgw.CancellationPending != true)
                        {
                            Thread.Sleep(50);
                            currentTime += 0.05;
                            pausedTime = currentTime;//in case of pause
                        }

                        //still playing, stay here
                        bgw.ReportProgress((int)(timeElapsed + currentTime));

                        // If the user has hit the button a second time and chosen to end playing of music
                        // then we need to check in the loop and return if so
                        //NEW PAUSE: if hit second time, then wait for unpause
                        if (bgw.CancellationPending == true)
                        {
                            pausedTrack = tracksPlayed; //incase of pause
                            e.Cancel = true;
                            return; //pause ended
                        }
                    }
                    timeElapsed += currentTime;
                    totalElapsedTime = timeElapsed; // incase of pause
                }
                // we've finished playing so reset values
                musicPlaying = false;
                totalElapsedTime = 0;
            }
        }

        // Background worker listener for when progress is reported from the background worker thread;
        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage <= progressBar.Maximum) //check for out of bounds
            {
                progressBar.Value = e.ProgressPercentage;
                progressBar.Update();
                int minutesElapsed = e.ProgressPercentage / 60;
                int secondsElapsed = e.ProgressPercentage % 60;
                string builtElapsedTimeString = minutesElapsed.ToString("00") + ":" + secondsElapsed.ToString("00") + "/" + totalMinuteDuration.ToString("00") + ":" + totalSecondDuration.ToString("00");
                elapsedTimeLabel.Text = builtElapsedTimeString;
            }
            else
            {
                Console.WriteLine("ERROR: Progress bar percentage attempting to write outside bounds. /%: " + e.ProgressPercentage);
            }
        }

        void PlayMusic(MediaPlayer currentPlayer, string path, bool paused)
        {
            //get path
            currentPlayer.Open(new Uri(path, UriKind.Relative));

            //update the label containing the track title
            string trackName = Path.GetFileName(path); //get track name from path
            currentTrackLabel.Text = trackName;


            currentPlayer.Play(); //start music
  
        }

        void UpdateTotalTime(int totalDuration)
        {
            totalMinuteDuration = totalDuration / 60;
            totalSecondDuration = totalDuration % 60;
        }


        //Jareds attempt at slide transitions///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void nextSlideButton_Click(object sender, EventArgs e)
        {
            //get the current and next slide in list
            int nextSlideIndex = 0;
            Slide currentSlide = SlidesToPlay.ElementAt(slideListIndex);

            //check for out of bounds, then execute
            if (slideListIndex < SlidesToPlay.Count - 1)
            {
                nextSlideIndex = slideListIndex + 1;
                Slide nextSlide = SlidesToPlay.ElementAt(nextSlideIndex);

                //call the transition
                ChangeSlide(currentSlide, nextSlide);

                slideListIndex = slideListIndex + 1;
            }
        }

        private void prevSlideButton_Click(object sender, EventArgs e)
        {
            //get the current and next slide in list
            int prevSlideIndex = 0;
            Slide currentSlide = SlidesToPlay.ElementAt(slideListIndex);

            //check for out of bounds, then execute
            if (slideListIndex > 0)
            {
                prevSlideIndex = slideListIndex - 1;
                Slide nextSlide = SlidesToPlay.ElementAt(prevSlideIndex);

                //call the transition
                ChangeSlide(currentSlide, nextSlide);
                slideListIndex = slideListIndex - 1;
            }
        }

        private void ChangeSlide(Slide current, Slide replacement)
        {
            slideTimer.Stop();
            slideTimer.Interval = replacement.Duration * 1000;
            slideTimer.Start();

            int lrud = (int)replacement.transitionType; //for wipe transition types
            
            sourceBmp1?.Dispose();
            sourceBmp2?.Dispose();
            sourceBmp1 = (Bitmap)Image.FromStream(new MemoryStream(File.ReadAllBytes(replacement.Path)));
            sourceBmp2 = (Bitmap)Image.FromStream(new MemoryStream(File.ReadAllBytes(current.Path)));
            fadeBmp1?.Dispose();
            fadeBmp2?.Dispose();
            fadeBmp1 = sourceBmp1.Clone() as Bitmap;
            fadeBmp2 = sourceBmp2.Clone() as Bitmap;
            
            switch (replacement.transitionType)
            {
                //CROSSFADE
                case transitionTypes.crossFade:
                    transitionTimer.Tick += DoFade;
                    transitionTimer.Start();
                    break;
                case transitionTypes.wipeLeft:
                case transitionTypes.wipeRight:
                case transitionTypes.wipeUp:
                case transitionTypes.wipeDown:
                    //transitionTimer.Elapsed += DoWipe;
                    //transitionTimer.Start();
                    break;
            }
        }

        private void DoTransition(object sender, EventArgs e)
        {
            if (slideListIndex < SlidesToPlay.Count - 1)
            {
                ChangeSlide(SlidesToPlay[slideListIndex], SlidesToPlay[++slideListIndex]);
            }
            else
            {
                slideTimer.Stop();
                slideTimer.Enabled = false;
            }
        }

        float opacity1 = 0.0f;
        float opacity2 = 1.0f;
        float increment = .2f;

        private void DoFade(object sender, EventArgs e)
        {
            opacity1 += increment;
            opacity2 -= increment;
            fadeBmp1?.Dispose();
            fadeBmp2?.Dispose();
            fadeBmp1 = sourceBmp1.SetOpacity(opacity1);
            fadeBmp2 = sourceBmp2.SetOpacity(opacity2);
            topPictureBox.Invalidate();
            if ((opacity1 >= 1.0f || opacity1 <= .0f) || (opacity2 >= 1.0f || opacity2 <= .0f))
            {
                transitionTimer.Stop();
                //reset
                opacity1 = 0.0f;
                opacity2 = 1.0f;
                increment = .2f;
                transitionTimer.Tick -= DoFade;
            }

            //if (Opacity > 0) // If we haven't fully faded out yet...
            //{
            //    topPictureBox.Invalidate();
            //    Bitmap bmp = new Bitmap(topPictureBox.Image.Width, topPictureBox.Image.Height);

            //    Graphics g = Graphics.FromImage(bmp);
            //    ColorMatrix colmat = new ColorMatrix();
            //    colmat.Matrix33 = Opacity;
            //    Opacity = Opacity - 0.1f; // increments of 10 (1 second transition)
            //    ImageAttributes imgAttr = new ImageAttributes();
            //    imgAttr.SetColorMatrix(colmat, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            //    g.DrawImage(topPictureBox.Image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, topPictureBox.Image.Width, topPictureBox.Image.Height, GraphicsUnit.Pixel, imgAttr);
            //    g.Dispose();
            //    // update the new fade in Image
            //    topPictureBox.Image = bmp;
            //}
            //else // Stop the timer and reset Opacity for next applicable slide
            //{
            //    topPictureBox.Image = bottomPictureBox.Image;
            //    transitionTimer.Stop();
            //    transitionTimer.Tick -= DoFade;
            //    Opacity = 1.0f;
            //}
        }







        private void wipeTransition(PictureBox current, PictureBox next, int lrud)
        {
            //handle left,right,up,down
            if (lrud == 1) //LEFT
            {
                //CODE FOR TRANSITIONING BETWEEN IMAGES
            }
            else if (lrud == 2)//RIGHT
            {
                //CODE FOR TRANSITIONING BETWEEN IMAGES
            }
            else if (lrud == 3)//UP
            {
                //CODE FOR TRANSITIONING BETWEEN IMAGES
            }
            else if (lrud == 4)//DOWN
            {
                //CODE FOR TRANSITIONING BETWEEN IMAGES
            }
        }


        private void wipeTransitonPREV(PictureBox current, PictureBox prev, int lrud)
        {
            //handle left,right,up,down
            if (lrud == 1) //LEFT
            {
                //CODE FOR TRANSITIONING BETWEEN IMAGES
            }
            else if (lrud == 2)//RIGHT
            {
                //CODE FOR TRANSITIONING BETWEEN IMAGES
            }
            else if (lrud == 3)//UP
            {
                //CODE FOR TRANSITIONING BETWEEN IMAGES
            }
            else if (lrud == 4)//DOWN
            {
                //CODE FOR TRANSITIONING BETWEEN IMAGES
            }
        }


        private Bitmap sourceBmp1 = null;
        private Bitmap sourceBmp2 = null;
        private Bitmap fadeBmp1 = null;
        private Bitmap fadeBmp2 = null;

        private void topPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (fadeBmp1 == null || fadeBmp2 == null) return;
            var units = GraphicsUnit.Pixel;
            e.Graphics.DrawImage(fadeBmp2, new RectangleF(PointF.Empty, topPictureBox.ClientSize), fadeBmp2.GetBounds(ref units), units);
            e.Graphics.DrawImage(fadeBmp1, new RectangleF(PointF.Empty, topPictureBox.ClientSize), fadeBmp1.GetBounds(ref units), units);
        }
    }

    public static class BitmapExtensions
    {
        static float[][] fadeMatrix = {
        new float[] {1, 0, 0, 0, 0},
        new float[] {0, 1, 0, 0, 0},
        new float[] {0, 0, 1, 0, 0},
        new float[] {0, 0, 0, 1, 0},
        new float[] {0, 0, 0, 0, 1}
    };

        public static Bitmap SetOpacity(this Bitmap bitmap, float Opacity, float Gamma = 1.0f)
        {
            var mx = new ColorMatrix(fadeMatrix);
            mx.Matrix33 = Opacity;
            var bmp = new Bitmap(bitmap.Width, bitmap.Height);

            using (var g = Graphics.FromImage(bmp))
            using (var attributes = new ImageAttributes())
            {
                attributes.SetGamma(Gamma, ColorAdjustType.Bitmap);
                attributes.SetColorMatrix(mx, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.Clear(System.Drawing.Color.Transparent);
                g.DrawImage(bitmap, new Rectangle(0, 0, bmp.Width, bmp.Height),
                    0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attributes);
                return bmp;
            }
        }
    }
}
