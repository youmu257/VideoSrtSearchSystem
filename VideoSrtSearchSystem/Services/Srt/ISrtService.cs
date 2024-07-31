using VideoSrtSearchSystem.DTO.Request.Srt;

namespace VideoSrtSearchSystem.Services.Srt
{
    public interface ISrtService
    {
        void ImportSrt(ImportSrtRequest request);
    }
}
