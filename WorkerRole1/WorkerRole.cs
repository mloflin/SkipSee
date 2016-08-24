using Microsoft.WindowsAzure.ServiceRuntime;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace MovieMiner
{
    public class WorkerRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;
            return base.OnStart();
        }

        public override void Run()
        {
            //Executes the updates on a given cadence as defined by the wait command and logic

            Trace.TraceInformation("MovieMiner entry point called");

            string lastMovieRun = string.Empty;
            string lastInstagramRun = string.Empty;
            int DebugCount = 0; //store this as 0 to slow down long debugging
            //On a restart / initial run - grab lastRun from storage
            SqlCommand sql = new SqlCommand();
            try
            {
                SqlConnection conn = new SqlConnection(RoleEnvironment.GetConfigurationSettingValue("SkipSee"));
                conn.Open();

                string cmd = string.Empty;
                string Operation = string.Empty;

                    //LastMovieRun
                    cmd = "SELECT Top 1 value FROM Parameter WHERE Name = @name";
                    Operation = "lastMovieRun";
                    object objlastMovieRun = new SqlCommand(cmd, conn)
                    {
                        Parameters = {new SqlParameter("@name", Operation)}
                    }.ExecuteScalar();

                    if (objlastMovieRun != null)
                        lastMovieRun = objlastMovieRun.ToString();

                    //LastRun
                    cmd = "SELECT Top 1 value FROM Parameter WHERE Name = @name";
                    Operation = "lastInstagramRun";
                    object objlastInstagramRun = new SqlCommand(cmd, conn)
                    {
                        Parameters = { new SqlParameter("@name", Operation) }
                    }.ExecuteScalar();

                    if (objlastInstagramRun != null)
                        lastInstagramRun = objlastInstagramRun.ToString();

                conn.Close();

            }
            catch (Exception ex)
            {
                Trace.TraceError("Error retrieving parameters - " + ex.Message);
                Tracing("Miner", "Parameters", "Error", "Miner", 0, "Error retrieving Parameters", ex.Message);
            }

            while (true)
            {

                string Today = string.Empty;
                string Now = string.Empty;
                int MaxDiff = 0;
                Now = DateTime.Now.ToString();

                //if debug - sleep only 1 minute - else set Today to Now and thread sleep for 5 minutes
#if DEBUG
                {
                    
                    if (DebugCount != 0)
                    {
                        Thread.Sleep(1000 * 60 * 5);
                    }
                    else
                    {
                        DebugCount += 1;
                    }

                    MaxDiff = -2000;
                    //Today = DateTime.Now.ToShortDateString();
                }
#else
                {
                    Thread.Sleep(1000 * 60 * 60);
                    Today = DateTime.Now.ToShortDateString();
                    MaxDiff = 30;
                }
#endif

                #region In_Theaters
                //Once a day get the latest In_Theaters Movies
                //if lastMovieRun != today's date, run the loop
                if (lastMovieRun != Today)
                {
                    try //updating the stored value
                    {
                        SqlConnection conn = new SqlConnection(RoleEnvironment.GetConfigurationSettingValue("SkipSee"));
                        string cmd = string.Empty;

                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        if (lastMovieRun != string.Empty) //if it exists - update it
                        {
                            cmd = "UPDATE [dbo].[Parameter] SET [Value] = @value WHERE [Name] = @name";
                        }
                        else //else create it
                        {
                            cmd = "INSERT INTO [dbo].[Parameter] ([Name],[Value]) VALUES (@name, @value)";
                        }
                        string name = "lastMovieRun";
                        string value = DateTime.Now.ToShortDateString();

                        //execute command
                        sql = new SqlCommand(cmd, conn);
                        sql.Parameters.AddWithValue("@name", name);
                        sql.Parameters.AddWithValue("@value", value);
                        sql.ExecuteNonQuery();

                        if (conn.State == ConnectionState.Open)
                            conn.Close();

                        Trace.TraceInformation("In_Theaters = Starting - " + lastMovieRun);
                        In_Theaters();
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("In_Theaters = Initialization Failed - " + lastMovieRun + " - " + ex.Message);
                        Tracing("Miner", "In_Theaters", "Error", "MovieMiner", 0, "Initialization Failed", ex.Message);
                    }
                    Trace.TraceInformation("In_Theaters = Complete - " + lastMovieRun);
                }
            #endregion 

                #region Instagram
                //Once a day get the latest Instagram Tags
                //if lastInstagramRun >= 5 minutes, run the loop
                DateTime dateValue;
                DateTime dtNow = DateTime.MinValue;
                DateTime dtlastInstagramRun = DateTime.MaxValue;

                if(DateTime.TryParse(Now, out dateValue))
                {
                    dtNow = dateValue;
                }

                if (DateTime.TryParse(lastInstagramRun, out dateValue))
                {
                    dtlastInstagramRun = dateValue;
                }
                else
                {
                    dtlastInstagramRun = DateTime.MinValue;
                }

                TimeSpan span = dtNow - dtlastInstagramRun;
                int diff = (int)span.TotalMinutes;

                //TODO add a check for "in progress" for this and the theaters
                if (diff >= MaxDiff)
                {
                    try //updating the stored value
                    {
                        SqlConnection conn = new SqlConnection(RoleEnvironment.GetConfigurationSettingValue("SkipSee"));
                        string cmd = string.Empty;

                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        if (lastInstagramRun != string.Empty) //if it exists - update it
                        {
                            cmd = "UPDATE [dbo].[Parameter] SET [Value] = @value WHERE [Name] = @name";
                        }
                        else //else create it
                        {
                            cmd = "INSERT INTO [dbo].[Parameter] ([Name],[Value]) VALUES (@name, @value)";
                        }
                        string name = "lastInstagramRun";
                        string value = DateTime.Now.ToString();

                        //execute command
                        sql = new SqlCommand(cmd, conn);
                        sql.Parameters.AddWithValue("@name", name);
                        sql.Parameters.AddWithValue("@value", value);
                        sql.ExecuteNonQuery();

                        if (conn.State == ConnectionState.Open)
                            conn.Close();

                        Trace.TraceInformation("Instagram = Starting - " + lastInstagramRun);
                        Instagram();
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Instagram = Initialization Failed - " + lastInstagramRun + " - " + ex.Message);
                        Tracing("Miner", "Instagram", "Error", "TagMiner", 0, "Initialization Failed", ex.Message);
                    }
                    Trace.TraceInformation("Instagram = Complete - " + lastInstagramRun);
                }
                #endregion
            }
        }

        private async void In_Theaters()
        {
            Trace.TraceInformation("In_Theaters = Starting REST Call");

            Tracing("Miner", "Start_Update_Movies", "Start", "MovieMiner", 0, "", "");

            //Store the startTime for duration
            DateTime startTime = DateTime.Now;
            int MovieCount = 0;
            //Setup Rest call
            RestClient restClient = new RestClient();
            RestRequest restRequest = new RestRequest(Method.GET)
            {
                Resource = "http://api.rottentomatoes.com/api/public/v1.0/lists/movies/in_theaters.json?apikey=m4fzj9pgc9m4xgcp23dbvj2a&page_limit=16&page=1"
            };
            IRestResponse restResponse = await restClient.ExecuteTaskAsync((IRestRequest)restRequest);

            Trace.TraceInformation("In_Theaters = Completed REST Call");

            try //getting the In-Theaters movies
            {
                Trace.TraceInformation("In_Theaters = Storing Data");

                JValue jTotal = (JValue)JObject.Parse(restResponse.Content)["total"];
                int totalMovies = (int)jTotal;
                decimal totalPages = 0;
                if (totalMovies != 0)
                {
                    totalPages = totalMovies / 16; //16 movies per page
                    totalPages = Math.Ceiling(totalPages);
                }

                for(int r = 1; r <= totalPages; r++)
                {
                    if (r != 1)
                    {
                        //If r = 1 then we already have the data to process, else update it with the next page of data and keep looping until r = totalPages
                        Trace.TraceInformation("In_Theaters = Starting REST Call");

                        //Setup Rest call
                        restClient = new RestClient();
                        restRequest = new RestRequest(Method.GET)
                        {
                            Resource = "http://api.rottentomatoes.com/api/public/v1.0/lists/movies/in_theaters.json?apikey=m4fzj9pgc9m4xgcp23dbvj2a&page_limit=16&page=" + r
                        };
                        restResponse = await restClient.ExecuteTaskAsync((IRestRequest)restRequest);

                        Trace.TraceInformation("In_Theaters = Completed REST Call");
                    }

                    JArray jarray = (JArray)JObject.Parse(restResponse.Content)["movies"];
                    string Count = jarray.Count.ToString();
                    if (Count != "0")
                    {
                        using (SqlConnection conn = new SqlConnection(RoleEnvironment.GetConfigurationSettingValue("SkipSee")))
                        {
                            for (int i = 0; i < jarray.Count; ++i) //iterate through each movie
                            {
                                JObject jobject = (JObject)jarray[i];


                                //Parse the variables for each movie
                                string MovieID = (string)jobject["id"] ?? string.Empty;
                                string title = (string)jobject["title"] ?? string.Empty;
                                if (!(MovieID == string.Empty) && !(title == string.Empty))
                                {
                                    int year = (int)jobject["year"];
                                    string MPAA = (string)jobject["mpaa_rating"];

                                    int runtime = 0;
                                    if (jobject["runtime"] != null && jobject["runtime"].ToString() != string.Empty)
                                        runtime = (int)jobject["runtime"];

                                    Ratings ratings = jobject["ratings"].ToObject<Ratings>();
                                    string critics_score = ratings.critics_score.ToString();
                                    string critics_rating = string.Empty;
                                    if (ratings.critics_rating != null)
                                        critics_rating = ratings.critics_rating;

                                    string audience_score = ratings.audience_score.ToString();
                                    string audience_rating = string.Empty;
                                    if (ratings.audience_rating != null)
                                        audience_rating = ratings.audience_rating;

                                    ReleaseDates releaseDates = jobject["release_dates"].ToObject<ReleaseDates>();
                                    DateTime theater = releaseDates.theater;
                                    DateTime dvd = releaseDates.DVD;
                                    if (dvd < DateTime.Now.AddYears(-100))
                                        dvd = DateTime.MaxValue;

                                    AbridgedCast[] abridgedCastArray = jobject["abridged_cast"].ToObject<AbridgedCast[]>();

                                    string synopsis = (string)jobject["synopsis"] ?? string.Empty;
                                    string critics_consensus = (string)jobject["critics_consensus"] ?? string.Empty;

                                    Posters posters = jobject["posters"].ToObject<Posters>();
                                    string thumbnail = (posters.thumbnail).ToString() ?? string.Empty;
                                    string profile = (posters.profile).ToString() ?? string.Empty;
                                    string detailed = (posters.detailed).ToString() ?? string.Empty;
                                    string original = (posters.original).ToString() ?? string.Empty;

                                    //store the tag as is with no spaces
                                    //assuming it isn't case sensitive
                                    //using regex to remove all non
                                    Regex rgx = new Regex("[^a-zA-Z0-9_]");
                                    string Tag = rgx.Replace(title, "");

                                    object objMovie = null;
                                    object objBlurb = null;
                                    object objPoster = null;
                                    object objRating = null;
                                    object objReleaseDates = null;
                                    object objTag = null;

                                    SqlCommand sql = new SqlCommand(string.Empty, conn);
                                    string cmd = string.Empty;

                                    try
                                    {
                                        if (conn.State != ConnectionState.Open)
                                            conn.Open();
                                        //go check each table to see if the rows already exist or not
                                        //if they do then perform updates, else inserts
                                        //also should catch when there was an error midway - it won't keep inserting duplicates
                                        objMovie = new SqlCommand("SELECT TOP 1 MovieID FROM tblMovies WHERE MovieID = @movieid", conn)
                                        {
                                            Parameters = {new SqlParameter("@movieid", MovieID)}
                                        }.ExecuteScalar();

                                        objBlurb = new SqlCommand("SELECT TOP 1 MovieID FROM tblBlurbs WHERE MovieID = @movieid", conn)
                                        {
                                            Parameters = {new SqlParameter("@movieid", MovieID)}
                                        }.ExecuteScalar();

                                        objPoster = new SqlCommand("SELECT TOP 1 MovieID FROM tblPosters WHERE MovieID = @movieid", conn)
                                        {
                                            Parameters = {new SqlParameter("@movieid", MovieID)}
                                        }.ExecuteScalar();

                                        objRating = new SqlCommand("SELECT TOP 1 MovieID FROM tblRatings WHERE MovieID = @movieid", conn)
                                        {
                                            Parameters = {new SqlParameter("@movieid", MovieID)}
                                        }.ExecuteScalar();

                                        objReleaseDates = new SqlCommand("SELECT TOP 1 MovieID FROM tblReleaseDates WHERE MovieID = @movieid", conn)
                                        {
                                            Parameters = {new SqlParameter("@movieid", MovieID)}
                                        }.ExecuteScalar();

                                        objTag = new SqlCommand("SELECT TOP 1 MovieID FROM tblTags WHERE MovieID = @movieid and Tag = @tag", conn)
                                        {
                                            Parameters = { new SqlParameter("@movieid", MovieID), new SqlParameter("@tag",Tag)}
                                        }.ExecuteScalar();

                                    }
                                    catch (Exception ex)
                                    {
                                        Trace.TraceError("In_Theaters = Select Movie Failed: " + ex.Message);
                                        Tracing("Miner", "In_Theaters", "Error", "MovieMiner", 0, "Select Movie Failed: ", ex.Message);
                                    }

                                    try
                                    {
                                        //Movies
                                        if (objMovie != null)
                                        {
                                            cmd = "UPDATE [dbo].[tblMovies] SET [Title] = @title, [Year] = @year, [MPAA_Rating] = @mpaa_rating, [RunTime] = @runtime, [Last_Updated] = @last_updated, [Times_Updated] = [Times_Updated] + 1 WHERE [MovieID] = @movieid";
                                        }
                                        else
                                        {
                                            cmd = "INSERT INTO [dbo].[tblMovies] ([MovieID],[Title],[Year],[MPAA_Rating],[RunTime],[Date_Added],[Last_Updated],[Times_Updated]) VALUES (@movieid, @title, @year, @mpaa_rating, @runtime,@date_added,@last_updated,@times_updated)";
                                            MovieCount += 1;
                                        }

                                        sql = new SqlCommand(cmd, conn);
                                        sql.Parameters.AddWithValue("@movieid", MovieID);
                                        sql.Parameters.AddWithValue("@title", title);
                                        sql.Parameters.AddWithValue("@year", year);
                                        sql.Parameters.AddWithValue("@mpaa_rating", MPAA);
                                        sql.Parameters.AddWithValue("@runtime", runtime);
                                        sql.Parameters.AddWithValue("@date_added", DateTime.Now.ToString());
                                        sql.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString());
                                        sql.Parameters.AddWithValue("@times_updated", 1);
                                    
                                        sql.ExecuteNonQuery();

                                        //Blurbs
                                        if (objBlurb != null)
                                        {
                                            cmd = "UPDATE [dbo].[tblBlurbs] SET [Synopsis] = @synopsis, [Critics_Consensus] = @critics_consensus WHERE [MovieID] = @movieid";
                                        }
                                        else
                                        {
                                            cmd = "INSERT INTO [dbo].[tblBlurbs] ([MovieID],[Synopsis],[Critics_Consensus]) VALUES (@movieid, @synopsis, @critics_consensus)";
                                        }

                                        sql = new SqlCommand(cmd, conn);
                                        sql.Parameters.AddWithValue("@movieid", MovieID);
                                        sql.Parameters.AddWithValue("@synopsis", synopsis);
                                        sql.Parameters.AddWithValue("@critics_consensus", critics_consensus);

                                        sql.ExecuteNonQuery();


                                        //Posters
                                        if (objPoster != null)
                                        {
                                            cmd = "UPDATE [dbo].[tblPosters] SET [Thumbnail] = @thumbnail, [Profile] = @profile, [Detailed] = @detailed, [Original] = @Original WHERE [MovieID] = @movieid";
                                        }
                                        else
                                        {
                                            cmd = "INSERT INTO [dbo].[tblPosters] ([MovieID],[Thumbnail],[Profile],[Detailed],[Original]) VALUES (@movieid, @thumbnail, @profile, @detailed, @original)";
                                        }

                                        sql = new SqlCommand(cmd, conn);
                                        sql.Parameters.AddWithValue("@movieid", MovieID);
                                        sql.Parameters.AddWithValue("@thumbnail", thumbnail);
                                        sql.Parameters.AddWithValue("@profile", profile);
                                        sql.Parameters.AddWithValue("@detailed", detailed);
                                        sql.Parameters.AddWithValue("@original", original);

                                        sql.ExecuteNonQuery();


                                        //Ratings
                                        if (objRating != null)
                                        {
                                            cmd = "UPDATE [dbo].[tblRatings] SET [Critics_Score] = @critics_score, [Critics_Rating] = @Critics_Rating, [Audience_Score] = @Audience_Score, [Audience_Rating] = @audience_rating WHERE [MovieID] = @movieid";
                                        }
                                        else
                                        {
                                            cmd = "INSERT INTO [dbo].[tblRatings] ([MovieID],[Critics_Score],[Critics_Rating],[Audience_Score],[Audience_Rating]) VALUES (@movieid, @critics_score, @critics_rating, @audience_score, @audience_rating)";
                                        }

                                        sql = new SqlCommand(cmd, conn);
                                        sql.Parameters.AddWithValue("@movieid", MovieID);
                                        sql.Parameters.AddWithValue("@critics_score", critics_score);
                                        sql.Parameters.AddWithValue("@critics_rating", critics_rating);
                                        sql.Parameters.AddWithValue("@audience_score", audience_score);
                                        sql.Parameters.AddWithValue("@audience_rating", audience_rating);

                                        sql.ExecuteNonQuery();


                                        //Release Dates
                                        if (objReleaseDates != null)
                                        {
                                            cmd = "UPDATE [dbo].[tblReleaseDates] SET [InTheaters_Date] = @intheaters_date, [OnDVD_Date] = @ondvd_date WHERE [MovieID] = @movieid";
                                        }
                                        else
                                        {
                                            cmd = "INSERT INTO [dbo].[tblReleaseDates] ([MovieID],[InTheaters_Date],[OnDVD_Date]) VALUES (@movieid, @intheaters_date, @ondvd_date)";
                                        }

                                        sql = new SqlCommand(cmd, conn);
                                        sql.Parameters.AddWithValue("@movieid", MovieID);
                                        sql.Parameters.AddWithValue("@intheaters_date", theater);
                                        sql.Parameters.AddWithValue("@ondvd_date", dvd);

                                        sql.ExecuteNonQuery();

                                        //Tag
                                        if (objTag != null)
                                        {
                                            //if it already exists, no need to update it
                                        }
                                        else
                                        {
                                            cmd = "INSERT INTO [dbo].[tblTags] ([MovieID],[Tag],[Image_Count],[Video_Count],[Date_Added],[Last_Updated]) VALUES (@movieid, @tag, 0,0, @date_added, @last_updated)";

                                            sql = new SqlCommand(cmd, conn);
                                            sql.Parameters.AddWithValue("@movieid", MovieID);
                                            sql.Parameters.AddWithValue("@tag", Tag);
                                            sql.Parameters.AddWithValue("@date_added", DateTime.Now.ToString());
                                            sql.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString());

                                            sql.ExecuteNonQuery();
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        Trace.TraceError("In_Theaters = Update Failed: " + ex.Message);
                                        Tracing("Miner", "In_Theaters", "Error", "MovieMiner", 0, "Update Failed: ", ex.Message);
                                    }


                                    object objCast = null;
                                    object objCharacters = null;
                                    try
                                    {
                                        if (conn.State != ConnectionState.Open)
                                            conn.Open();

                                        //Cast members could have 1 to many Characters - iterate through each
                                        foreach (AbridgedCast abridgedCast in abridgedCastArray)
                                        {
                                            //Cast
                                            string name = abridgedCast.name;
                                            if (name != null)
                                            {
                                                sql = new SqlCommand("SELECT TOP 1 ID FROM tblCast WHERE MovieID = @movieid and Name = @name", conn);
                                                sql.Parameters.AddWithValue("@movieid", MovieID);
                                                sql.Parameters.AddWithValue("@name", name);
                                                objCast = sql.ExecuteScalar();

                                                if (objCast == null)
                                                    new SqlCommand("INSERT INTO [dbo].[tblCast] ([MovieID],[Name]) VALUES (@movieid, @name)", conn)
                                                    {
                                                        Parameters = {new SqlParameter("@movieid", (object) MovieID),new SqlParameter("@name", (object) name)}
                                                    }.ExecuteNonQuery();
                                                else
                                                {
                                                    //TODO
                                                }
                                            }

                                            //Characters
                                            string[] characters = abridgedCast.characters;
                                            if (characters != null && characters.Length >= 1)
                                            {
                                                foreach (string c in characters)
                                                {
                                                    cmd = "SELECT TOP 1 ID FROM tblCharacters WHERE MovieID = @movieid and Character = @character and CastID = (select TOP 1 ID from tblCast where MovieID = @movieid and Name = @name)";
                                                    string character = c.ToString();

                                                    sql = new SqlCommand(cmd, conn);
                                                    sql.Parameters.AddWithValue("@movieid", MovieID);
                                                    sql.Parameters.AddWithValue("@character", character);
                                                    sql.Parameters.AddWithValue("@name", name);
                                                    objCharacters = sql.ExecuteScalar();

                                                    if (objCharacters == null)
                                                        new SqlCommand("INSERT INTO [dbo].[tblCharacters] ([MovieID],[CastID],[Character]) VALUES (@movieid,(Select TOP 1 [ID] from [tblCast] WHERE [MovieID] = @movieid AND [Name] = @name), @Character)", conn)
                                                        {
                                                            Parameters = {new SqlParameter("@movieid", (object) MovieID),new SqlParameter("@name", (object) name),new SqlParameter("@character", (object) c)}
                                                        }.ExecuteNonQuery();
                                                    else
                                                    {
                                                        //TODO
                                                    }
                                                }

                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Trace.TraceError("In_Theaters = Cast Update Failed: " + ex.Message);
                                        Tracing("Miner", "In_Theaters", "Error", "MovieMiner", 0, "Cast Update Failed: ", ex.Message);
                                    }
                                }
                            }
                            if (conn.State == ConnectionState.Open)
                                conn.Close();
                        }
                    }
                }
            
                Tracing("Miner", "Finish_Update_Movies", "Complete", "MovieMiner", (int)(DateTime.Now - startTime).TotalMilliseconds, "Field1 = Added Movies", MovieCount.ToString());
                Trace.TraceInformation("In_Theaters = Storing Data Complete");
            }
            catch (Exception ex)
            {
                Trace.TraceError("In_Theaters = REST Call Failed: " + ex.Message);
                Tracing("Miner", "In_Theaters", "Error", "MovieMiner", 0, "REST Call Failed", ex.Message);
            }
        }  

        private async void Instagram()
        {
            Trace.TraceInformation("Instagram = Starting REST Call");

            Tracing("Miner", "Start_Instagram_Pull", "Start", "TagMiner", 0, "", "");

            //Store the startTime for duration
            DateTime startTime = DateTime.Now;
            int Count = 0;
            
            using (SqlConnection conn = new SqlConnection(RoleEnvironment.GetConfigurationSettingValue("SkipSee")))
            {
                try
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    //Grab Movies in the Theater from DB
                    //Then grab the tags for each movie - iterating through them and calling Instagram for the results
                    //Store the results
                    SqlDataReader objMovies = new SqlCommand("SELECT MovieID FROM tblMovies WHERE Last_Updated >= DATEADD(dd, -1, GETDATE())", conn)
                    {
                        Parameters = { }
                    }.ExecuteReader();

                    string[] MovieID = new String[1000];  //not foreseeing more than 100 movies at one time
                    int cnt = 0;
                    while (objMovies.Read())
                    {
                        MovieID[cnt] = objMovies.GetInt32(0).ToString();
                        cnt += 1;
                    }
                    objMovies.Close();

                    foreach (var movieID in MovieID)
                    {
                        if (movieID != null)
                        {

                            if (conn.State != ConnectionState.Open)
                                conn.Open();

                            SqlDataReader objTags = new SqlCommand("SELECT ID, Tag FROM tblTags WHERE MovieID = @movieid", conn)
                            {
                                Parameters = { new SqlParameter("@movieid", movieID) }
                            }.ExecuteReader();

                            string[] Tags = new String[1000];  //not foreseeing more than 100 tags at one time
                            string[] TagIDs = new String[1000];  //not foreseeing more than 100 tags at one time
                            cnt = 0;
                            while (objTags.Read())
                            {
                                TagIDs[cnt] = objTags.GetInt32(0).ToString();
                                Tags[cnt] = objTags.GetString(1);
                                cnt += 1;
                            }
                            objTags.Close();

                            if (Tags != null && TagIDs != null)
                            {
                                //iterate over each Tag
                                for (int x = 0; x < cnt; x++)
                                {
                                    string tag = Tags[x].ToString();
                                    string tagID = TagIDs[x].ToString();

                                    //Setup REST Call
                                    string client_id = "0b6242f92b5f48f79ea64c37d7de52ae";
                                    RestClient restClient = new RestClient();
                                    RestRequest restRequest = new RestRequest(Method.GET)
                                    {
                                        Resource = "https://api.instagram.com/v1/tags/" + tag + "/media/recent?client_id=" + client_id
                                    };
                                    IRestResponse restResponse = await restClient.ExecuteTaskAsync((IRestRequest)restRequest);

                                    Trace.TraceInformation("Instagram = Completed REST Call");

                                    if (!restResponse.Content.ToString().Contains("502 Bad Gateway"))
                                    {
                                        try
                                        {
                                            JArray jarray = (JArray)JObject.Parse(restResponse.Content)["data"];

                                            string PostCount = jarray.Count.ToString();
                                            if (PostCount != "0")
                                            {
                                                for (int i = 0; i < jarray.Count; ++i) //iterate through each movie //
                                                {
                                                    JObject jobject = (JObject)jarray[i];

                                                    //Parse the variables for each item
                                                    string type = (string)jobject["type"] ?? string.Empty;
                                                    string postid = (string)jobject["id"] ?? string.Empty;

                                                    Comments comments = jobject["comments"].ToObject<Comments>();
                                                    int intComments = comments.count;

                                                    Likes likes = jobject["likes"].ToObject<Likes>();
                                                    int intLikes = likes.count;

                                                    string strCaption = string.Empty;
                                                    if (jobject["caption"] != null && jobject["caption"].HasValues)
                                                    {
                                                        Caption caption = jobject["caption"].ToObject<Caption>();
                                                        strCaption = caption.text.ToString() ?? string.Empty;
                                                    }

                                                    Images images = null;
                                                    Videos videos = null;
                                                    string thumbnail = string.Empty;
                                                    string low_resolution = string.Empty;
                                                    string standard_resolution = string.Empty;
                                                    if (type == "image")
                                                    {
                                                        images = jobject["images"].ToObject<Images>();
                                                        thumbnail = images.thumbnail.url.ToString() ?? string.Empty;
                                                        low_resolution = images.low_resolution.url.ToString() ?? string.Empty;
                                                        standard_resolution = images.standard_resolution.url.ToString() ?? string.Empty;
                                                    }
                                                    else if (type == "video")
                                                    {
                                                        videos = jobject["videos"].ToObject<Videos>();
                                                        low_resolution = videos.low_resolution.url.ToString() ?? string.Empty;
                                                        standard_resolution = videos.standard_resolution.url.ToString() ?? string.Empty;
                                                    }

                                                    var TagArray = new List<object>();
                                                    foreach (string h in jobject["tags"])
                                                    {
                                                        TagArray.Add(h);
                                                    }


                                                    //Check if this item already exists
                                                    object objPost = null;
                                                    object objImages = null;
                                                    object objVideos = null;
                                                    object objTag = null;

                                                    SqlCommand sql = new SqlCommand(string.Empty, conn);
                                                    string cmd = string.Empty;

                                                    try
                                                    {
                                                        if (conn.State != ConnectionState.Open)
                                                            conn.Open();
                                                        //go check each table to see if the rows already exist or not
                                                        //if they do then perform updates, else inserts
                                                        //also should catch when there was an error midway - it won't keep inserting duplicates
                                                        objPost = new SqlCommand("SELECT TOP 1 ID FROM tblInst_Post WHERE PostID = @postid", conn)
                                                        {
                                                            Parameters = { new SqlParameter("@postid", postid) }
                                                        }.ExecuteScalar();

                                                        objImages = new SqlCommand("SELECT TOP 1 ID FROM tblInst_Images WHERE PostID = @postid", conn)
                                                        {
                                                            Parameters = { new SqlParameter("@postid", postid) }
                                                        }.ExecuteScalar();

                                                        objVideos = new SqlCommand("SELECT TOP 1 MovieID FROM tblInst_Videos WHERE PostID = @postid", conn)
                                                        {
                                                            Parameters = { new SqlParameter("@postid", postid) }
                                                        }.ExecuteScalar();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Trace.TraceError("Instagram_Pull = Select Post Failed: " + ex.Message);
                                                        Tracing("Miner", "Instagram_Pull", "Error", "TagMiner", 0, "Select Post Failed: ", ex.Message);
                                                    }

                                                    try
                                                    {
                                                        //Post
                                                        if (objPost != null)
                                                        {
                                                            cmd = "UPDATE [dbo].[tblInst_Post] SET [Comments] = @comments, [Likes] = @likes, [Last_Updated] = @last_updated WHERE [PostID] = @postid";
                                                        }
                                                        else
                                                        {
                                                            cmd = "INSERT INTO [dbo].[tblInst_Post] ([MovieID],[TagID],[PostID],[Type],[Caption],[Comments],[Likes],[Date_Added],[Last_Updated]) VALUES (@movieid, @tagid, @postid, @type, @caption, @comments, @likes, @date_added, @last_updated)";
                                                            Count += 1;
                                                        }

                                                        sql = new SqlCommand(cmd, conn);
                                                        sql.Parameters.AddWithValue("@movieid", movieID);
                                                        sql.Parameters.AddWithValue("@tagid", tagID);
                                                        sql.Parameters.AddWithValue("@postid", postid);
                                                        sql.Parameters.AddWithValue("@type", type);
                                                        sql.Parameters.AddWithValue("@caption", strCaption);
                                                        sql.Parameters.AddWithValue("@comments", intComments);
                                                        sql.Parameters.AddWithValue("@likes", intLikes);
                                                        sql.Parameters.AddWithValue("@date_added", DateTime.Now.ToString());
                                                        sql.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString());

                                                        sql.ExecuteNonQuery();

                                                        if (type == "image")
                                                        {
                                                            //Images
                                                            if (objImages != null)
                                                            {
                                                                cmd = "UPDATE [dbo].[tblInst_Images] SET [Thumbnail] = @thumbnail, [Low_Resolution] = @low_resolution, [Standard_Resolution] = @standard_resolution where PostID = @postid";
                                                            }
                                                            else
                                                            {
                                                                cmd = "INSERT INTO [dbo].[tblInst_Images] ([MovieID],[TagID],[PostID],[Thumbnail],[Low_Resolution],[Standard_Resolution]) VALUES (@movieid, @tagid, @postid, @thumbnail, @low_resolution, @standard_resolution)";
                                                            }

                                                            sql = new SqlCommand(cmd, conn);
                                                            sql.Parameters.AddWithValue("@movieid", movieID);
                                                            sql.Parameters.AddWithValue("@tagid", tagID);
                                                            sql.Parameters.AddWithValue("@postid", postid);
                                                            sql.Parameters.AddWithValue("@thumbnail", thumbnail);
                                                            sql.Parameters.AddWithValue("@low_resolution", low_resolution);
                                                            sql.Parameters.AddWithValue("@standard_resolution", standard_resolution);

                                                            sql.ExecuteNonQuery();
                                                        }
                                                        else
                                                        {
                                                            //Video
                                                            if (objVideos != null)
                                                            {
                                                                cmd = "UPDATE [dbo].[tblInst_Videos] SET [Low_Resolution] = @low_resolution, [Standard_Resolution] = @standard_resolution where PostID = @postid";
                                                            }
                                                            else
                                                            {
                                                                cmd = "INSERT INTO [dbo].[tblInst_Videos] ([MovieID],[TagID],[PostID],[Low_Resolution],[Standard_Resolution]) VALUES (@movieid, @tagid, @postid, @low_resolution, @standard_resolution)";
                                                            }

                                                            sql = new SqlCommand(cmd, conn);
                                                            sql.Parameters.AddWithValue("@movieid", movieID);
                                                            sql.Parameters.AddWithValue("@tagid", tagID);
                                                            sql.Parameters.AddWithValue("@postid", postid);
                                                            sql.Parameters.AddWithValue("@low_resolution", low_resolution);
                                                            sql.Parameters.AddWithValue("@standard_resolution", standard_resolution);

                                                            sql.ExecuteNonQuery();
                                                        }

                                                        //Loop through Tags storing the new ones
                                                        if (conn.State != ConnectionState.Open)
                                                            conn.Open();

                                                        foreach (string htag in TagArray)
                                                        {
                                                            //Tags

                                                            if (htag != null)
                                                            {
                                                                sql = new SqlCommand("SELECT TOP 1 ID FROM tblInst_Tags WHERE MovieID = @MovieID and TagID = @tagid and PostID = @postid and Tag = @tag", conn);
                                                                sql.Parameters.AddWithValue("@movieid", movieID);
                                                                sql.Parameters.AddWithValue("@postid", postid);
                                                                sql.Parameters.AddWithValue("@tagid", tagID);
                                                                sql.Parameters.AddWithValue("@tag", htag);
                                                                objTag = sql.ExecuteScalar();

                                                                if (objTag == null)
                                                                {
                                                                    cmd = "INSERT INTO [dbo].[tblInst_Tags] ([MovieID],[TagID],[PostID],[Tag],[Date_Added]) VALUES (@movieid, @tagID, @postid, @tag, @date_added)";

                                                                    sql = new SqlCommand(cmd, conn);
                                                                    sql.Parameters.AddWithValue("@movieid", movieID);
                                                                    sql.Parameters.AddWithValue("@postid", postid);
                                                                    sql.Parameters.AddWithValue("@tagid", tagID);
                                                                    sql.Parameters.AddWithValue("@tag", htag);
                                                                    sql.Parameters.AddWithValue("@date_added", DateTime.Now.ToString());

                                                                    sql.ExecuteNonQuery();
                                                                }
                                                                else
                                                                {
                                                                    //TODO
                                                                }
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Trace.TraceError("Instagram_Pull = Update Failed: " + ex.Message);
                                                        Tracing("Miner", "Instagram_Pull", "Error", "TagMiner", 0, "Update Failed: ", ex.Message);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Trace.TraceError("Instagram_Pull = Tag Caching Failed: " + ex.Message);
                                            Tracing("Miner", "Instagram_Pull", "Error", "TagMiner", 0, "Tag Caching: ", ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        Trace.TraceError("Instagram_Pull = 502 Bad Gateway" + restResponse.Content.ToString());
                                        Tracing("Miner", "Instagram_Pull", "Error", "TagMiner", 0, "502 Bad Gateway: ", restResponse.Content.ToString());
                                    }

                                    //Update Counts for each Tag once completed
                                    if (conn.State != ConnectionState.Open)
                                        conn.Open();
                                    string strcmd = "UPDATE [dbo].[tblTags] SET [Image_Count] = (Select Count(*) FROM tblInst_Images where [TagID] = @tagID), [Video_Count] = (Select Count(*) from tblInst_Videos where [TagID] = @tagID), [Last_Updated] = @last_updated WHERE [ID] = @tagID";
                                    SqlCommand sqlc = new SqlCommand(strcmd, conn);
                                    sqlc.Parameters.AddWithValue("@tagid", tagID);
                                    sqlc.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString());

                                    sqlc.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    Tracing("Miner", "Finish_Instagram_Pull", "Complete", "TagMiner", (int)(DateTime.Now - startTime).TotalMilliseconds, "Field1 = Added Posts", Count.ToString());
                    Trace.TraceInformation("Instagram_Pull = Storing Data Complete");
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Instagram_Pull = Tag Caching Failed: " + ex.Message);
                    Tracing("Miner", "Instagram_Pull", "Error", "TagMiner", 0, "High-Level Failure: ", ex.Message);
                }
            }
        }

        #region Tracing
        private void Tracing(string Role, string Operation, string Status, string UserIdentity, int CallDurationMS, string Notes, string Field1)
        {
            using (SqlConnection conn = new SqlConnection(RoleEnvironment.GetConfigurationSettingValue("SkipSee")))
            {
                try
                {
                    SqlCommand sql = new SqlCommand("INSERT INTO [dbo].[Tracing] ([Timestamp],[Role],[Operation],[Status],[UserIdentity],[CallDurationMS],[Notes],[Field1]) VALUES (@timestamp, @role, @operation, @status, @useridentity, @callduration, @notes, @field1)", conn);
                    DateTime now = DateTime.Now;
                    sql.Parameters.AddWithValue("@timestamp", now);
                    sql.Parameters.AddWithValue("@role", Role);
                    sql.Parameters.AddWithValue("@operation", Operation);
                    sql.Parameters.AddWithValue("@status", Status);
                    sql.Parameters.AddWithValue("@useridentity", UserIdentity);
                    sql.Parameters.AddWithValue("@callduration", CallDurationMS);
                    sql.Parameters.AddWithValue("@notes", Notes);
                    sql.Parameters.AddWithValue("@field1", Field1);
                    conn.Open();
                    sql.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Tracing = Error Logging Trace: " + ex.Message);
                    Tracing("Miner", "Tracing", "Error", "MovieMiner", 0, "Error Logging Trace", ex.Message);
                }
            }
        }
        #endregion

        #region Movie_Classes
        public class ReleaseDates
        {
            public DateTime theater { get; set; }

            public DateTime DVD { get; set; }
        }

        public class Ratings
        {
            public string critics_rating { get; set; }

            public int critics_score { get; set; }

            public string audience_rating { get; set; }

            public int audience_score { get; set; }
        }

        public class Posters
        {
            public string thumbnail { get; set; }

            public string profile { get; set; }

            public string detailed { get; set; }

            public string original { get; set; }
        }

        public class AbridgedCast
        {
            public string name { get; set; }

            public string[] characters { get; set; }
        }

        public class MovieLinks
        {
            public string self { get; set; }

            public string alternate { get; set; }

            public string cast { get; set; }

            public string clips { get; set; }

            public string reviews { get; set; }

            public string similar { get; set; }
        }
        #endregion

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
