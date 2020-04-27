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
    public partial class Form1 : Form
    {
        //Create dialog for opening save file
        OpenFileDialog ofd = new OpenFileDialog();
        PlayerHandler ph = new PlayerHandler();
        Decoder de = new Decoder();

        //Variable Declaration
        int showDuration;

        bool hasShow = false;

        public Form1()
        {
            ofd = new OpenFileDialog();
            InitializeComponent();

            //initialize welcome message
            instructionsTextBox.Text = "Welcome to SlidebySide Player!" +
                "\n\nPlease use the button in the top left to select your project folder " +
                "that you created. \n\n Review your slides, and then click on the \"Start Show\" button when ready!";
        }


        private void pb_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            //var tmp = pb.ImageLocation;
            //pictureBox1.ImageLocation = tmp;
            pictureBox1.Image = new Bitmap(pb.Image);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }


        private void fileOpenButton_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            DialogResult fbdResult = fbd.ShowDialog();

            if (fbdResult == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string pathToOpen = fbd.SelectedPath; //get project folder path

                //Build a list of Soundtracks from the specified file
                ph.ImportedSoundTrackList = de.ImportSoundTracksFromFile(pathToOpen);

                //print TEST soundtrack info
                foreach (SoundTrack x in ph.ImportedSoundTrackList)
                {
                    Console.WriteLine(x.Path);
                    Console.WriteLine(x.Duration);
                }

                //Update show duration
                foreach (SoundTrack x in ph.ImportedSoundTrackList)
                {
                    showDuration = showDuration + x.Duration;
                }


                    //Build a list of slides for the specefied file
                    ph.ImportedSlidesList = de.ImportSlidesFromFile(pathToOpen);

                //print TEST slide info
                foreach (Slide x in ph.ImportedSlidesList)
                {
                    Console.WriteLine(x.Path);
                    Console.WriteLine(x.Duration);
                    
                }

                hasShow = true;
            }
            else
            {
                //TODO: UPDATE INSTRUCTIONS BOX
                Console.WriteLine("Could not open project folder");
            }

            //now that lists are imported, populate the preview panes
            //populate soundtracks
            populateSoundTrackBar();
            populateSlideBar();
            //populate the slides
            //@TODO: Chandler call your function to put the images in preview panes here!
        }


        private void populateSlideBar()
        {
            slideDisplayPanel.Controls.Clear();

            foreach (Slide slide in ph.ImportedSlidesList)
            {
                string slideName = Path.GetFileName(slide.Path);

                PictureBox pb = new PictureBox();
                pb.MouseDown += new MouseEventHandler(pb_MouseDown);
                pb.Image = new Bitmap(slide.Path);
                pb.SizeMode = PictureBoxSizeMode.StretchImage;

                slideDisplayPanel.Controls.Add(pb);
            }
            slideDisplayPanel.Update();
        }

        private void populateSoundTrackBar()
        {
                //clear panel before redraw
                musicLayoutPanel.Controls.Clear();
                foreach (SoundTrack track in ph.ImportedSoundTrackList)
                {
                    //string trackName = Path.GetFileName(track.Path);

                    //build new panel
                    Panel newSoundTrackPanel = new Panel();
                    Label newSoundTrackLabel = new Label();
                    newSoundTrackLabel.Font = new Font("Arial Black", 9);
                    string trackName = Path.GetFileNameWithoutExtension(track.Path);
                    newSoundTrackLabel.Text = trackName;
                    newSoundTrackPanel.Controls.Add(newSoundTrackLabel);
                    musicLayoutPanel.Controls.Add(newSoundTrackPanel);
                }
                //Iterator for trackLength percentages
                int trackLengthIterator = 0;
                foreach (Panel panel in musicLayoutPanel.Controls)
                {
                    //format each panel
                    //Handle division based on time of each track
                    panel.BackColor = System.Drawing.Color.LightGreen;
                    double percentage = (double)(ph.ImportedSoundTrackList.ElementAt(trackLengthIterator).Duration) / (double)(showDuration);
                    panel.Width = (int)(percentage * ((double)musicLayoutPanel.Width - 5)) - 6;
                    panel.Height = musicLayoutPanel.Height - 5;
                    trackLengthIterator += 1;
                }
                //Update Panel with new drawn panels based on current list
                musicLayoutPanel.Update();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (hasShow)
            {
                //make a window for playing the show and display it (passing in current Lists)
                Form2 showWindow = new Form2(ph.ImportedSoundTrackList, ph.ImportedSlidesList);
                showWindow.ShowDialog();
            }
            else
            {
                Console.WriteLine("Please add a slideshow to play");
            }
            
        }

        private void slideDisplayPanel_Paint(object sender, PaintEventArgs e)
        {
            this.slideDisplayPanel.AutoScroll = true;
            this.slideDisplayPanel.WrapContents = true;
            this.slideDisplayPanel.VerticalScroll.Enabled = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
