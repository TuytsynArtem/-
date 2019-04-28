using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HelloXamarinForms.Views
{

    public class Prediction
    {
        public string TagId { get; set; }
        public string TagName { get; set; }
        public double Probability { get; set; }
    }

    public class RootObject
    {
        public string Id { get; set; }
        public string Project { get; set; }
        public string Iteration { get; set; }
        public DateTime Created { get; set; }
        public List<Prediction> Predictions { get; set; }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MushroomPage : ContentPage
    {

        public MushroomPage()
        {
            
            //InitializeComponent();
            Button getPhoto = new Button { Text = "Img" };
            Image img = new Image{ };
           Label predictionrespose = new Label { };
           

            getPhoto.Clicked += async (o, e) =>
            {
                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    MediaFile photo = await CrossMedia.Current.PickPhotoAsync();
                    if (photo == null) return;
                    img.Source = ImageSource.FromFile(photo.Path);
                    predictionrespose.Text = await MakePredictionRequest(photo.Path);
                }
            };

            Content = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    new StackLayout
                    {

                        Children={getPhoto , predictionrespose},
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        
                    },
                    img

                }
            };

        }
        public static async Task<string> MakePredictionRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid Prediction-Key.
            client.DefaultRequestHeaders.Add("Prediction-Key", "4419aff217074723ba0464ec51776865");

            // Prediction URL - replace this example URL with your valid Prediction URL.
            string url = "https://westeurope.api.cognitive.microsoft.com/customvision/v3.0/Prediction/66a82350-68b5-470f-9018-b46db58cfd43/classify/iterations/Iteration21/image";

            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);
            string result = "";
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                result = await response.Content.ReadAsStringAsync();
            }
            var rateInfo = JsonConvert.DeserializeObject<RootObject>(result.ToString());
            var score = rateInfo.Predictions.Max(x => x.Probability);
            var name = rateInfo.Predictions.Where(x => x.Probability == score).ToArray();
            if (name[0] == null) return " ";

            return name[0].TagName + " на  " + Math.Round(name[0].Probability*100) + "%";
        }
        //fdafdafdsafafs
        private static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
         
    }

}
