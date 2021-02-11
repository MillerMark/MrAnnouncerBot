using System;
using CloudinaryDotNet;
using System.Linq;
using System.Net.Http;
using DndCore;
using CloudinaryDotNet.Actions;
using BotCore;

namespace CardMaker
{
	public class CloudinaryClient
	{
		string imagePath;
		Cloudinary cloudinary;

		public CloudinaryClient(string imagePath, MySecureString cloudinaryUrl)
		{
			this.imagePath = imagePath;
			cloudinary = new Cloudinary(cloudinaryUrl.GetStr());
		}

		/// <summary>
		/// Uploads the specified local image to cloudinary.com
		/// </summary>
		/// <param name="imageName">The name of the local image to upload, located in the imagePath constant declared in this class.</param>
		/// <returns>Returns the full path to the uploaded image.</returns>
		public string UploadImage(string imageName)
		{
			var uploadParams = new ImageUploadParams()
			{
				File = new FileDescription(System.IO.Path.Combine(imagePath, imageName)),
				PublicId = $"cards/{System.IO.Path.GetFileNameWithoutExtension(imageName)}",
				Overwrite = true,
				Invalidate = true
			};
			var uploadResult = cloudinary.Upload(uploadParams);
			return $"https://res.cloudinary.com/dragonhumpers/cards/{imageName}";
		}
	}
}
