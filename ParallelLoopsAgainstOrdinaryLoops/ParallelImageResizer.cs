using System.Drawing;

namespace ParallelLoopsAgainstOrdinaryLoops
{
    public class ParallelImageResizer
    {
        private string imageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

        public void ResizeImagesParallel(List<string> imagePaths, double scaleFactor)
        {
            Parallel.ForEach(imagePaths, imagePath =>
            {
                ResizeImage(imagePath, scaleFactor);
            });
        }

        private void ResizeImage(string imagePath, double scaleFactor)
        {
            using (Image image = Image.FromFile(imagePath))
            {
                int newWidth = (int)(image.Width * scaleFactor);
                int newHeight = (int)(image.Height * scaleFactor);

                using (Bitmap resizedImage = new Bitmap(image, new Size(newWidth, newHeight)))
                {
                    string newFilePath = $"{Path.GetFileNameWithoutExtension(imagePath)}_resized{Path.GetExtension(imagePath)}";
                    resizedImage.Save(newFilePath);
                }
            }
        }
    }
}
