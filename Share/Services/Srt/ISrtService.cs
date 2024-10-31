using Share.DTO.Request.Srt;
using Share.DTO.Response.Srt;

namespace Share.Services.Srt
{
    public interface ISrtService
    {
        string ImportSrt(ImportSrtRequest request);
        SearchSrtResponse SearchSrt(string keyword, int page);
        DownloadSrtResponse DownloadSrt(string videoGuid);
    }
}
