namespace LMS.Models
{
    public class TestResultModel
    {
        public string stdOut { get; set; }
        public string stdErr { get; set; }
        public int ProcessExtCode { get; set; }
        public bool UmlFounded { get; set; }
    }
}