using RadioTaxi.Services;

namespace RadioTaxi.Services
{
    public interface ICommon
    {
        Task<string> UploadImgAvatarAsync(IFormFile file);
        Task<string> UploadedFile(IFormFile ProfilePicture);

        Task<string> UploadImgBackgroudAsync(IFormFile file);
        Task<string> UploadPdfAsync(IFormFile file);
        string RandomString(int length);
        string[] GenerateAlphabetArray(int length);
        List<String> RotateTeams(List<string> teams);
        IPayPal PaypalServices { get; }
        IPayPal PaypalCapture { get; }
    }
}
