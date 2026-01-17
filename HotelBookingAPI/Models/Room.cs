using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingAPI.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int Type { get; set; } // 1: Single, 2: Double, 3: Deluxe
        public int Capacity { get; set; }
        [ForeignKey(nameof(Hotel))]
        public int HotelId { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}