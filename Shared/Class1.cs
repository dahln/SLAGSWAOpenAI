namespace Shared
{
    public class ImageRequest
    {
        public string ImageDescription { get; set; } = string.Empty;
    }

    public class ImageResponse
    {
        public Uri ImageSource { get; set; } = null!;
    }
}
