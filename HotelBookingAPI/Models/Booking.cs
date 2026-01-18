using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingAPI.Models
{
    [Index(nameof(Reference), IsUnique = true)]
    public class Booking
    {
        public int Id { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        [ForeignKey(nameof(Room))]
        public int RoomID { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int NumberOfGuests { get; set; }
    }
}