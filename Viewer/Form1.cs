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

        public Form1()
        {
            ofd = new OpenFileDialog();
            InitializeComponent();

            //initialize welcome message
            instructionsTextBox.Text = "Welcome to SlidebySide Player!" +
                "\n\nPlease use the button in the top left to select your project folder " +
                "that you created. \n\n Review your slides, and then click on the \"Start Show\" button when ready!";
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


                //Build a list of slides for the specefied file
                ph.ImportedSlidesList = de.ImportSlidesFromFile(pathToOpen);

                //print TEST slide info
                foreach (Slide x in ph.ImportedSlidesList)
                {
                    Console.WriteLine(x.Path);
                    Console.WriteLine(x.Duration);
                }
            }
            else
            {
                //TODO: UPDATE INSTRUCTIONS BOX
                Console.WriteLine("Could not open project folder");
            }

            //now that lists are imported, populate the preview panes
            //populate soundtracks
            populateSoundTrackBar();

            //populate the slides
            //@TODO: Chandler call your function to put the images in preview panes here!
        }

        private void populateSoundTrackBar()
        {
            //clear panel before redraw
            musicLayoutPanel.Controls.Clear();

            int counter = 0;

            foreach (SoundTrack track in ph.ImportedSoundTrackList)
            {
                string trackName = Path.GetFileName(track.Path);
                //build new panel
                Panel newSoundTrackPanel = new Panel();
                Label newSoundTrackLabel = new Label();
                newSoundTrackLabel.AutoSize = true;
                newSoundTrackLabel.Font = new Font("Arial Black", 9);
                newSoundTrackLabel.TextAlign = ContentAlignment.BottomCenter;
                newSoundTrackLabel.Text = trackName;
                newSoundTrackPanel.Controls.Add(newSoundTrackLabel);
                musicLayoutPanel.Controls.Add(newSoundTrackPanel);

                //test code
                Console.WriteLine(counter);
                Console.WriteLine(trackName);
                counter++;


                foreach (Panel panel in musicLayoutPanel.Controls)
                {
                    //format each panel

                    panel.BackColor = System.Drawing.Color.LightGreen;
                    panel.Width = (musicLayoutPanel.Width - 5) / musicLayoutPanel.Controls.Count - 5;
                    panel.Height = musicLayoutPanel.Height - 5;
                }
            }

            //Update Panel with new drawn panels based on current list
            musicLayoutPanel.Update();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            //make a window for playing the show and display it (passing in current Lists)
            Form2 showWindow = new Form2(ph.ImportedSoundTrackList, ph.ImportedSlidesList);
            showWindow.ShowDialog();
        }
    }
}
