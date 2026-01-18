using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingAPI.Models
{
    public class Room
    {
        public int Id { get; set; }
        [ForeignKey(nameof(RoomType))]
        public int RoomTypeId { get; set; }
        public RoomType RoomType { get; set; } = null!;
        [ForeignKey(nameof(Hotel))]
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}