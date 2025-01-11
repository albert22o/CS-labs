using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

using MP3File = TagLib.File;

namespace Lab_No6
{
	internal sealed class MusicTrack
	{
		private readonly Uri _defaultAlbumCover = new("/Images/default-album-cover.jpeg", UriKind.Relative);

		internal MusicTrack(string pathToTrack)
		{
			MP3File musicTrack = MP3File.Create(pathToTrack);
			TrackLength = musicTrack.Properties.Duration;
			PathToTrack = new Uri(pathToTrack, UriKind.Absolute);
			TrackName = musicTrack.Tag.Title;
			ArtistName = musicTrack.Tag.FirstPerformer;
			AlbumName = musicTrack.Tag.Album;

			if (musicTrack.Tag.Pictures.Length != 0)
			{
				byte[] imageBytes;

				using MemoryStream mstr = new(musicTrack.Tag.Pictures[0].Data.Data);
				using Image imgFromStream = Image.FromStream(mstr);

				int width = 350;
				int height = 350;

				using Bitmap bitmap = new(imgFromStream, new Size(width, height));
				using MemoryStream mstr2 = new();
				bitmap.Save(mstr2, System.Drawing.Imaging.ImageFormat.Jpeg);
				imageBytes = mstr2.ToArray();

				AlbumArt = new();
				AlbumArt.BeginInit();
				AlbumArt.StreamSource = new MemoryStream(imageBytes);
				AlbumArt.EndInit();
			}
			else
				AlbumArt = new(_defaultAlbumCover);
		}

		internal BitmapImage AlbumArt { get; private set; }
		internal TimeSpan TrackLength { get; private set; }
		internal Uri PathToTrack { get; private set; }

		internal string TrackName { get; private set; }
		internal string ArtistName { get; private set; }
		internal string AlbumName { get; private set; }
	}
}