using LMS.EntityСontext;

namespace LMS.EntityContext
{
    public class ChatMessages
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int AssignedVariantId { get; set; }

        public DateTime SendDate { get; set; }

        public string Message { get; set; }

        public AssignedVariant Variant { get; set; }

        public User User { get; set; }
    }
}