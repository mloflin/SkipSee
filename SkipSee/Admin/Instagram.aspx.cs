using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SkipSee.Admin
{
    public partial class Instagram : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected async void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Regex rgx = new Regex("[^a-zA-Z0-9_]");
                string tag = rgx.Replace(txtTag.Text.ToString(), "");

                string response = await Call_Instagram(tag);

                if (!response.ToString().Contains("502 Bad Gateway"))
                {
                    try
                    {
                        JArray jarray = (JArray)JObject.Parse(response)["data"];

                        string Count = jarray.Count.ToString();
                        if (Count != "0")
                        {
                            for (int i = 0; i < jarray.Count; ++i) //iterate through each movie //
                            {
                                JObject jobject = (JObject)jarray[i];

                                //Parse the variables for each item
                                string type = (string)jobject["type"] ?? string.Empty;
                                Comments comments = jobject["comments"].ToObject<Comments>();
                                Likes likes = jobject["likes"].ToObject<Likes>();
                                Caption caption = jobject["caption"].ToObject<Caption>();

                                Images images = null;
                                Videos videos = null;
                                if (type == "image")
                                {
                                    images = jobject["images"].ToObject<Images>();
                                    Image image = new Image();
                                    image.ImageUrl = images.standard_resolution.url.ToString();
                                    image.Width = 225;
                                    image.Height = 225;
                                    ImagePlaceholder.Controls.Add(image);
                                }
                                else if (type == "video")
                                {
                                    videos = jobject["videos"].ToObject<Videos>();
                                    LiteralControl video = new LiteralControl();
                                    video.Text = @"<video controls='controls' width='225' height='225'><source src='" + videos.standard_resolution.url.ToString() + "' type='video/mp4' /></video>";
                                    VideoPlaceholder.Controls.Add(video);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private static async Task<string> Call_Instagram(string tag)
        {
          string client_id = "0b6242f92b5f48f79ea64c37d7de52ae";
          RestClient restClient = new RestClient();
          RestRequest restRequest = new RestRequest(Method.GET)
          {
            Resource = "https://api.instagram.com/v1/tags/" + tag + "/media/recent?client_id=" + client_id
          };
          IRestResponse restResponse = await restClient.ExecuteTaskAsync((IRestRequest) restRequest);
          return ((object) restResponse.Content).ToString();
        }

        #region Instagram_Classes
        public class Comments
        {
            public int count { get; set; }
        }

        public class Likes
        {
            public int count { get; set; }
        }

        public class Caption
        {
            public string text { get; set; }
        }

        public class LowResolution
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class Thumbnail
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class StandardResolution
        {
            public string url { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class Images
        {
            public LowResolution low_resolution { get; set; }
            public Thumbnail thumbnail { get; set; }
            public StandardResolution standard_resolution { get; set; }
        }

        public class Videos
        {
            public LowResolution low_resolution { get; set; }
            public StandardResolution standard_resolution { get; set; }
        }
        #endregion
    }
}