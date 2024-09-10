namespace VideoSrtSearchSystem.DTO.Response.Srt
{
    public class DownloadSrtResponse
    {
        public string FileName { get; set; } = string.Empty;

        public MemoryStream SrtFile { get; set; } = new MemoryStream();
    }
}
