using VideoSrtSearchSystem.DTO.Request.Srt;
using VideoSrtSearchSystem.DTO.Response.Srt;

namespace VideoSrtSearchSystem.Services.Srt
{
    public interface ISrtService
    {
        void ImportSrt(ImportSrtRequest request);
        List<SearchSrtResponse> SearchSrt(string keyword, int page);
    }
}
