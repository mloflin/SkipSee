using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SkipSee.Admin
{
    public partial class Movies : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnAddTag_Click(object sender, EventArgs e)
        {
            //store the tag as is with no spaces
            //assuming it isn't case sensitive
            if (txtAddTag.Text.ToString() != null && txtAddTag.Text.ToString() != string.Empty)
            {
                Regex rgx = new Regex("[^a-zA-Z0-9_]");
                string Tag = rgx.Replace(txtAddTag.Text.ToString(), "");
                string MovieID = ddlMovie.SelectedValue.ToString();

                //Tag
                if (Tag != null && Tag != string.Empty)
                {
                    using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SkipSee"].ConnectionString))
                    {
                        if (conn.State != System.Data.ConnectionState.Open)
                            conn.Open();

                        SqlCommand sql = new SqlCommand(string.Empty, conn);
                        string cmd = string.Empty;

                        cmd = "INSERT INTO [dbo].[tblTags] ([MovieID],[Tag],[Image_Count],[Video_Count],[Date_Added],[Last_Updated]) VALUES (@movieid, @tag, 0,0, @date_added, @last_updated)";

                        sql = new SqlCommand(cmd, conn);
                        sql.Parameters.AddWithValue("@movieid", MovieID);
                        sql.Parameters.AddWithValue("@tag", Tag);
                        sql.Parameters.AddWithValue("@date_added", DateTime.Now.ToString());
                        sql.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString());

                        sql.ExecuteNonQuery();

                        conn.Close();

                        //clear text box
                        txtAddTag.Text = "";
                    }
                    //Update the tags list
                    gvTags.DataBind();
                }
            }
        }

        protected void ddlMovie_SelectedIndexChanged(object sender, EventArgs e)
        {
            string movieID = ddlMovie.SelectedValue.ToString();
            Populate_Instagram(movieID);

        }
        private void Populate_Instagram(string movieID)
        {
            //Images
            string[] ImageURL = new String[10];  //not foreseeing more than 100 movies at one time
            int cnt = 0;
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SkipSee"].ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlDataReader objImages = new SqlCommand("select top 10 Standard_Resolution from tblInst_Post INNER JOIN tblInst_Images ON tblInst_Post.PostID = tblInst_Images.PostID WHERE tblInst_Post.MovieID = @movieid order by Likes desc", conn)
                {
                    Parameters = { new SqlParameter("@movieid", movieID) }
                }.ExecuteReader();


                while (objImages.Read())
                {
                    ImageURL[cnt] = objImages.GetString(0);
                    cnt += 1;
                }
                objImages.Close();

                for (int i = 0; i < cnt; i++)
                {
                    Image image = new Image();
                    image.ImageUrl = ImageURL[i].ToString();
                    image.Width = 225;
                    image.Height = 225;
                    ImagePlaceholder.Controls.Add(image);
                }

                string[] VideoURL = new String[10];  //not foreseeing more than 100 movies at one time
                cnt = 0;
                SqlDataReader objVideos = new SqlCommand("select top 10 Standard_Resolution from tblInst_Post INNER JOIN tblInst_Videos ON tblInst_Post.PostID = tblInst_Videos.PostID WHERE tblInst_Post.MovieID = @movieid order by Likes desc", conn)
                {
                    Parameters = { new SqlParameter("@movieid", movieID) }
                }.ExecuteReader();


                while (objVideos.Read())
                {
                    VideoURL[cnt] = objVideos.GetString(0);
                    cnt += 1;
                }
                objVideos.Close();

                for (int i = 0; i < cnt; i++)
                {
                    LiteralControl video = new LiteralControl();
                    video.Text = @"<video controls='controls' width='225' height='225'><source src='" + VideoURL[i].ToString() + "' type='video/mp4' /></video>";
                    VideoPlaceholder.Controls.Add(video);
                }
                conn.Close();
            }
        }

        protected void ddlMovie_DataBound(object sender, EventArgs e)
        {
            string movieID = ddlMovie.SelectedValue;
            Populate_Instagram(movieID);
        }
    }
}