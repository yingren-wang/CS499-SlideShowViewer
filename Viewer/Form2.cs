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

        // Variable for slide transitions
        System.Timers.Timer wipeTimer = new System.Timers.Timer();


        public Form2(List<SoundTrack> ListOfTracks, List<Slide> ListOfSlides)
        {
            InitializeComponent();
            PictureBox.CheckForIllegalCrossThreadCalls = false;
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
            slideChangeTimer.Tick += slideChangeTimer_Tick;
            
        }


        private static Image ResizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        private void resizeSlides()
        {
            foreach(Slide slide in SlidesToPlay)
            {

            }
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

        private void setupSlideChangeTimer(Slide slideToTime)
        {
            //Get the time in millis
            slideChangeTimer.Interval = (slideToTime.Duration * 1000);
            Console.WriteLine("Timer time interval has changed.");
        }

        //Function to reset the timer whenever the current duration is over
        private void slideChangeTimer_Tick(object sender, EventArgs e)
        {
            slideChangeTimer.Stop();
            Console.WriteLine("Timer Has Ticked");

            if (currentSlideIndex != slideCount)
            {
                currentSlideIndex += 1;
                changeSlides();
                slideChangeTimer.Start();
                Console.WriteLine("New timer started by slide: " + currentSlideIndex + " set for " + slideChangeTimer.Interval);
            }
            else
            {
                slideChangeTimer.Stop();
                slideChangeTimer.Dispose();
            }
        }

        
        private void wipeTransition(PictureBox slideWipeIn, int transitionType, int duration, string path) 
        {
            int movement = 10; // use the slide duration to set up the movement
            
            wipeTimer.Interval = 55;
           
            switch (transitionType)
            {
                case 0: // none

                    break;


                case 1: // wipe left
                        // picturebox2 stays in front, picturebox2 wipes away, leaving picturebox 1 as the incoming slide.
                        // after the transition is finished, picturebox2 get set to the new current slide, and is then re-sized and repositioned
                        // to be in front of picturebox1
                    pictureBox1.Visible = true;
                    pictureBox2.Visible = true;
                    pictureBox1.Image = ResizeImage(new Bitmap(currentSlide.Path), new Size(1066, 532));    // set to current slide
                    pictureBox2.Image = ResizeImage(new Bitmap(nextSlide.Path), new Size(1066, 532));       // set to next slide
                    
                    break;

                case 2: // wipe right
                        // picturebox1's visability is set to true and the image gets set to current slide, picturebox2's width gets set to 0 and the image is set to the next slide.
                        // picturebox2 slowly wipes over picturebox 1.  Once the wipe is done, picturebox1 is set to nextslide and visibility is set to false

                    pictureBox1.Visible = true;
                    pictureBox1.Image = ResizeImage(new Bitmap(currentSlide.Path), new Size(1066, 532));
                    pictureBox2.Image = ResizeImage(new Bitmap(nextSlide.Path), new Size(1066, 532));
                    pictureBox2.Width = 0;
                    break;
                case 3: // wipe up
                        // picturebox1's visablity is set to true, picturebox1 image is set to current slide, picturebox2 is set to next slide and its height is set to 0
                        // picturebox2 wipes over picturebox1.  once the wipe is done, picturebox1 is set to the nextslide and visablity is set to false

                    pictureBox1.Visible = true;
                    pictureBox1.Image = ResizeImage(new Bitmap(nextSlide.Path), new Size(1066, 532));       // set to next slide
                    pictureBox2.Image = ResizeImage(new Bitmap(currentSlide.Path), new Size(1066, 532));    // set to current slide
                    break;
                case 4: // wipe down
                        // picturebox1's visability is set to true, picturebox2 is current slide, picturebox1 is next slide
                        // picturebox2 wipes down, revealing picturebox1.  After the transistion is finished, picturebox2 gets set to the new current slide
                        // picturebox2 get set to the original size, and picturebox1 get set to the next slide and visability is set to false
                    pictureBox1.Visible = true;
                    pictureBox2.Visible = true;
                    pictureBox1.Image = ResizeImage(new Bitmap(currentSlide.Path), new Size(1066, 532));
                    pictureBox2.Image = ResizeImage(new Bitmap(nextSlide.Path), new Size(1066, 532));
                    pictureBox2.Height = 0;

                    
                    break;
                //case 5: // crossfade
                //    break;
            }

            //pictureBox2.Image = new Bitmap(path);
            //pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            wipeTimer.Start();
            wipeTimer.Elapsed += (sender, e) => OnWipeTransitionEvent(sender, e, movement, transitionType);
            
        }

        private void OnWipeTransitionEvent(Object sender, System.Timers.ElapsedEventArgs e, int movement, int transitionType)
        {
            Console.WriteLine("Transition Time Elapsed at time: " + DateTimeOffset.Now.ToUnixTimeMilliseconds());
            Console.WriteLine("Transition Type Integer is: " + transitionType);
            switch (transitionType)
            {
                case 0: // none

                    break;

                case 1: // wipe left
                    Console.WriteLine("Case Wipe Left");
                    if (pictureBox2.Width < 1066)
                    {
                        pictureBox2.Width += movement;
                    }
                    else
                    {
                        wipeTimer.Stop();
                    }
                    break;
                case 2: // wipe right
                    Console.WriteLine("Case Wipe Right");
                    if (pictureBox1.Width > 0)
                    {
                        pictureBox1.Width -= movement;
                    }
                    else
                    {
                        wipeTimer.Stop();
                    }
                    break;
                    
                case 3: // wipe up
                    Console.WriteLine("Case Wipe Up");
                    if (pictureBox2.Height > 0)
                    {
                        pictureBox2.Height -= movement;
                    }
                    else
                    {
                        Console.WriteLine("Wipe Up Timer Has Been Stopped");
                        wipeTimer.Stop();
                    }
                    break;
                case 4: // wipe down
                    Console.WriteLine("Case Wipe Down");
                    if (pictureBox2.Height < 532)
                    {
                        pictureBox2.Height += movement;
                    }
                    else
                    {
                        wipeTimer.Stop();
                    }
                    break;

                    //case 5: // crossfade
                    //          Console.WriteLine("Case Wipe Cross Fade");
                    //    break;
            }


        }

        private void changeSlides()
        {
            if (currentSlideIndex == 0) {
                //---------------
                // first slide
                //---------------
                
                wipeTransition(pictureBox2, currentSlide.transitionType, currentSlide.Duration, currentSlide.Path); // perform the wipe
                setupSlideChangeTimer(currentSlide);                // call the timer
                slideChangeTimer.Start();       // start the timer
                //currentSlide = SlidesToPlay[currentSlideIndex];     // update current slide
                //nextSlide = SlidesToPlay[currentSlideIndex + 1];    // update next slide

                // this happens post-transition
                //pictureBox2.Width = 1066;       // resetting picturebox2 size
                //pictureBox2.Height = 532;       
                //pictureBox1.Width = 1066;       // resetting picturebox1 size
                //pictureBox1.Height = 532;
                //pictureBox1.Image = ResizeImage(new Bitmap(nextSlide.Path), new Size(1066, 532));       // set to next slide
                //pictureBox2.Image = ResizeImage(new Bitmap(currentSlide.Path), new Size(1066, 532));    // set to current slide
                //pictureBox1.Visible = false;    // setting picturebox1 to be invisible
            }
            else if(currentSlideIndex < slideCount - 1)
            {   //--------------------
                // operating normally
                //--------------------
                currentSlide = SlidesToPlay[currentSlideIndex];     // update current slide
                nextSlide = SlidesToPlay[currentSlideIndex + 1];    // update next slide
                
                // do the wipe
                wipeTransition(pictureBox2, currentSlide.transitionType, currentSlide.Duration, currentSlide.Path);
                setupSlideChangeTimer(currentSlide);

                // this happens post transition
                pictureBox2.Width = 1066;       // resetting picturebox2 size
                pictureBox2.Height = 532;
                pictureBox1.Width = 1066;       // resetting picturebox1 size
                pictureBox1.Height = 532;
                pictureBox1.Image = ResizeImage(new Bitmap(nextSlide.Path), new Size(1066, 532));       // set to next slide
                pictureBox2.Image = ResizeImage(new Bitmap(currentSlide.Path), new Size(1066, 532));    // set to current slide
                //pictureBox1.Visible = false;    // setting picturebox1 to be invisible

                slideChangeTimer.Start();       // start the timer
            }
            else if(currentSlideIndex == slideCount - 1) //
            {   //------------
                // last slide
                //------------

                //pictureBox1.Visible = false;    // set picturebox1 to invisible
                
                currentSlide = SlidesToPlay[currentSlideIndex]; // update current slide

                wipeTransition(pictureBox2, currentSlide.transitionType, currentSlide.Duration, currentSlide.Path);
                setupSlideChangeTimer(currentSlide);

                pictureBox2.Width = 1066;       // resetting picturebox2 size
                pictureBox2.Height = 532;
                pictureBox2.Image = ResizeImage(new Bitmap(currentSlide.Path), new Size(1066, 532));    // set to current slide

                //pictureBox1.Visible = false;
                slideChangeTimer.Start();       // start the timer
            }
            else if (currentSlideIndex == slideCount)
            {   //------------------
                // end of slideshow
                //------------------
                pictureBox1.Visible = false;
                pictureBox2.Visible = false;
                slideChangeTimer.Stop();
            }

            //if (currentslideindex == 0)//first slide; initialize boxes
            //{
            //    //transition functionality here
            //    console.writeline("first slide");
            //    //change the images
            //    picturebox2.image = new bitmap(currentslide.path);
            //    picturebox2.sizemode = pictureboxsizemode.stretchimage;
            //    picturebox1.image = new bitmap(nextslide.path);
            //    picturebox1.sizemode = pictureboxsizemode.stretchimage;
            //    setupslidechangetimer(currentslide);
            //    slidechangetimer.start();
            //    //currentslideindex += 1;
            //    console.writeline("current slide is now: " + currentslideindex);

            //}
            //else if (currentslideindex == (slidecount - 1)) //last slide; there isn't a current slide
            //{
            //    //transition functionality here
            //    console.writeline("last slide clause has been triggered");
            //    //change the images
            //    picturebox1.visible = false;
            //    currentslide = slidestoplay[currentslideindex];
            //    picturebox2.image = new bitmap(currentslide.path);
            //    picturebox2.sizemode = pictureboxsizemode.stretchimage;
            //    //slidechangetimer.stop();
            //}
            //else if (currentslideindex == slidecount)
            //{
            //    picturebox2.visible = false;
            //    slidechangetimer.stop();
            //}
            //else //normal case
            //{
            //    //transition functionality here
            //    console.writeline("normal case clause has been triggered");
            //    //change the images
            //    //currentslideindex += 1;
            //    console.writeline("current slide is now: " + currentslideindex);
            //    currentslide = slidestoplay[currentslideindex];
            //    nextslide = slidestoplay[currentslideindex + 1];
            //    picturebox2.image = new bitmap(currentslide.path);
            //    picturebox2.sizemode = pictureboxsizemode.stretchimage;
            //    picturebox1.image = new bitmap(nextslide.path);
            //    picturebox1.sizemode = pictureboxsizemode.stretchimage;
            //    setupslidechangetimer(currentslide);
            //    slidechangetimer.start();
            //}
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
                pictureBox1.Image = ResizeImage(new Bitmap(nextSlide.Path), new Size(1066, 532));    // set to current slide
                pictureBox2.Image = ResizeImage(new Bitmap(currentSlide.Path), new Size(1066, 532));    // set to current slide
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
                slideChangeTimer.Start();
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
                slideChangeTimer.Stop();

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
