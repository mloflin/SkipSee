# SkipSee

This project was based on expanding the concepts of RottenTomatoes and other Movie Review websites by taking into consideration social media (e.g. Instagram) reviews. Ultimately providing a "Should I Skip it or See it" mantra.

We built a worker role that would regularly check RottenTomatoes API for new movies and then once new movies were found it automatically created hashtags to search Instagram for (e.g. The Godfather became #TheGodfather). For Instagram we pulled in any applicable videos/images with the appropriate tags and then via Admin we could easily expand the the given Tags for a given Movie.

The UX never got past the Proof of Concept phases but the aggregation of data worked and the future goal would be to favor people who use #SkipSee with video summaries and even taking into consideration who your friends/friends of friends were that have similar tastes in movies. Even weighing the sentiment of a given post could then give a social score for a given movie that could trend over time.

I worked on this project in 2014 and had to pull together the files, needless to say it currently doesn't build completely.

Note: WorkRole.cs is under the WorkerRole1 folder and contains some of the backend worker logic.
Note2: All relevant UX work was under the ~/Admin/ folder including only Instagram, Logs, and Movies pages.
