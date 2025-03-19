namespace BepKhoiBackend.BusinessObject.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; } // Đảm bảo có thuộc tính này

        public bool? IsVerify { get; set; }
        public string Password { get; set; } = null!;

    }
}
