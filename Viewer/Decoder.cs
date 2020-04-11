using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Threading;
using System.IO;
using System.Web;
using Newtonsoft.Json;

namespace Viewer
{
	// This class is responsible for taking the lists
	// created during the project and writing them to 
	// a json file.
	public class Decoder
	{
		public Decoder()
		{

		}

		public List<SoundTrack> ImportSoundTracksFromFile(string pathToOpen)
		{
			//make a new temp list
			List<SoundTrack> SoundTrackListFromFile = new List<SoundTrack>();

			//generate file name
			string SoundTrackPath = pathToOpen + "\\soundtracklist.json";

			//read json file in parsing objects out as new soundtrack objects and placing them in new list
			try
			{
				using (StreamReader r = new StreamReader(SoundTrackPath))
				{
					string json = r.ReadToEnd();
					SoundTrackListFromFile = JsonConvert.DeserializeObject<List<SoundTrack>>(json);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("ERROR: Json tracks file cannot be opened\n\n");
			}

			return SoundTrackListFromFile;
		}

		public List<Slide> ImportSlidesFromFile(string pathToOpen)
		{
			//make a new temp list
			List<Slide> SlideListFromFile = new List<Slide>();

			//generate file name
			string SlidePath = pathToOpen + "\\slidelist.json";

			//read json file in parsing objects out as new soundtrack objects and placing them in new list
			try
			{
				using (StreamReader r = new StreamReader(SlidePath))
				{
					string json = r.ReadToEnd();
					SlideListFromFile = JsonConvert.DeserializeObject<List<Slide>>(json);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("ERROR: Json slides file cannot be opened\n\n");
			}

			return SlideListFromFile;
		}

	}
}
