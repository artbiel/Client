namespace Client.Models
{
    public class CourseMainInfoVM : BaseModel
    {
        public string Title { get; set; }
        public string ImgSrc { get; set; }
    }

    public enum CourseSearchType
    {
        All, 
        My
    }
}
