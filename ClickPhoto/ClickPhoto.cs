using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickPhoto
{
    public interface ICameraService
    {
        Task<string> CapturePhotoAsync();
    }

    internal class CameraService : ICameraService
    {
        public async Task<string> CapturePhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                string photostring = await LoadPhotoAsync(photo);
                return photostring;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                return fnsEx.Message;
            }
            catch (PermissionException pEx)
            {
                return pEx.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        async Task<string> LoadPhotoAsync(FileResult photo)
        {
            if (photo == null)
            {
                return null;
            }

            try
            {
                using (var stream = await photo.OpenReadAsync())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        byte[] fileBytes = memoryStream.ToArray();
                        string base64String = $"data:{photo.ContentType};base64,{Convert.ToBase64String(fileBytes)}";
                        return base64String;
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error converting photo to base64: {ex.Message}";
            }
        }
    }
}
