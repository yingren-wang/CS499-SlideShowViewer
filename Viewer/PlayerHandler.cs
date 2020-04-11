using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Threading;

namespace Viewer
{
    public struct Slide
    {
        private string path;
        public string Path { get; set; }

        private int duration;
        public int Duration { get; set; }

        private int transitionTime;
        public int TransitionTime { get; set; }

        public enum transitionType
        {
            wipeLeft,
            wipeRight,
            wipeUp,
            wipdeDown,
            crossFade
        }


    }

    //struct to hold soundtrack information
    public struct SoundTrack
    {

        private string path;

        public string Path { get; set; }

        private int duration;

        public int Duration { get; set; }
    }
    public class PlayerHandler
    {
        //------------------
        // member variables
        //------------------

        public MediaPlayer soundTrackPlayer = new MediaPlayer();

        private readonly object _lock = new object();

        //List to hold all imported sound tracks
        public List<SoundTrack> ImportedSoundTrackList = new List<SoundTrack>();

        //List to hold all imported slides
        public List<Slide> ImportedSlidesList = new List<Slide>();

        //Constructor
        public PlayerHandler()
        {

        }

        //-----------
        // methods
        //-----------

    }
}
