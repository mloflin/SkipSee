select * from tblInst_Post;
select * from tblInst_Images order by PostID;
select * from tblInst_Videos order by PostID;

select top 10 Standard_Resolution 
from tblInst_Post inner join tblInst_Images 
ON tblInst_Post.PostID = tblInst_Images.PostID 
WHERE tblInst_Post.MovieID = '771312513' 
order by Likes desc

select top 10 Standard_Resolution 
from tblInst_Post inner join tblInst_Videos 
ON tblInst_Post.PostID = tblInst_Videos.PostID 
WHERE tblInst_Post.MovieID = '771312513' 
order by Likes desc
