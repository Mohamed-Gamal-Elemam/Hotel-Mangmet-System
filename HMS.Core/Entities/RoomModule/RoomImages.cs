namespace HMS.Core.Entities.RoomModule
{
    public class RoomImages : BaseEntity<int>
    {
        public string ImageUrl { get; set; } = null!;
        public int RoomId { get; set; }
      
    }
}