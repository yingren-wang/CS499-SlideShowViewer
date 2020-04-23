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
        private int slideCount = 0;
        private int currentSlideIndex = 0;
        private BackgroundWorker bgw = new BackgroundWorker(); //This is our worker that plays the musics and updates the progress bar
        private bool paused = false; //used to determine whether slideshow status is paused or playing
        private bool firstPlayPress = true; //to diffirentiate pause/resume from intial play click
        private bool resumeMusic = false;
        private Slide currentSlide;
        private Slide nextSlide;


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

            //timer setup
            slideTransitionTimer.Tick += slideTransitionTimer_Tick;
            
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

        private void setupTimer(Slide slideToTime)
        {
            //Get the time in millis
            //slideTransitionTimer.Stop();
            slideTransitionTimer.Interval = (slideToTime.Duration * 1000);
            Console.WriteLine("Timer time interval has changed.");
            //slideTransitionTimer.Start();

        }

        //Function to reset the timer whenever the current duration is over
        private void slideTransitionTimer_Tick(object sender, EventArgs e)
        {
            slideTransitionTimer.Stop();
            Console.WriteLine("Timer Has Ticked");
            //slideTransitionTimer.Dispose();
            //Console.WriteLine("Timer Disposed");
            if (currentSlideIndex != slideCount)
            {
                currentSlideIndex += 1;
                changeSlides();
                slideTransitionTimer.Start();
                Console.WriteLine("New timer started by slide: " + currentSlideIndex + " set for " + slideTransitionTimer.Interval);
            }
            else
            {
                slideTransitionTimer.Stop();
                slideTransitionTimer.Dispose();
            }
        }

        private void changeSlides()
        {
            if (currentSlideIndex == 0)//first slide; initialize boxes
            {
                //Transition Functionality Here
                Console.WriteLine("First slide");
                //Change the images
                pictureBox2.Image = new Bitmap(currentSlide.Path);
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = new Bitmap(nextSlide.Path);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                setupTimer(currentSlide);
                slideTransitionTimer.Start();
                //currentSlideIndex += 1;
                Console.WriteLine("Current slide is now: " + currentSlideIndex);

            }
            /**else if(currentSlideIndex == (slideCount - 2)) //Next to last slide, there isn't a next slide
            {
                //Transition Functionality Here
                Console.WriteLine("Next to Last Slide Cluase Has Been Triggered");
                //Change the images
                pictureBox1.Visible = false;
                //currentSlideIndex += 1;
                Console.WriteLine("Current slide is now: " + currentSlideIndex);
                currentSlide = SlidesToPlay[currentSlideIndex];
                nextSlide = SlidesToPlay[currentSlideIndex + 1];
                pictureBox2.Image = new Bitmap(currentSlide.Path);
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                setupTimer(currentSlide);
                //slideTransitionTimer.Start();

            }
    */
            else if(currentSlideIndex == (slideCount - 1)) //Last slide; there isn't a current slide
            {
                //Transition Functionality Here
                Console.WriteLine("Last slide clause has been triggered");
                //Change the images
                pictureBox1.Visible = false;
                currentSlide = SlidesToPlay[currentSlideIndex];
                pictureBox2.Image = new Bitmap(currentSlide.Path);
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                //slideTransitionTimer.Stop();
            }
            else if(currentSlideIndex == slideCount)
            {
                pictureBox2.Visible = false;
                slideTransitionTimer.Stop();
            }
            else //Normal case
            {
                //Transition Functionality Here
                Console.WriteLine("Normal Case Clause has been triggered");
                //Change the images
                //currentSlideIndex += 1;
                Console.WriteLine("Current slide is now: " + currentSlideIndex);
                currentSlide = SlidesToPlay[currentSlideIndex];
                nextSlide = SlidesToPlay[currentSlideIndex + 1];
                pictureBox2.Image = new Bitmap(currentSlide.Path);
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = new Bitmap(nextSlide.Path);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                setupTimer(currentSlide);
                //slideTransitionTimer.Start();
            }
        }

        private void playAndPauseButton_Click(object sender, EventArgs e)
        {
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
                slideCount = SlidesToPlay.Count();
                currentSlideIndex = 0;
                currentSlide = SlidesToPlay[currentSlideIndex];
                nextSlide = SlidesToPlay[currentSlideIndex + 1];

                changeSlides();
                

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
                slideTransitionTimer.Start();
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
                slideTransitionTimer.Stop();

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

        
    }
}
