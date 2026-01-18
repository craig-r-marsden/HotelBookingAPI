using System;

namespace HotelBookingAPI.Dtos
{
    public record HotelDto(int Id, string Name);
    public record RoomDto(int Id, int Type, int Capacity, int HotelId);
    public record CreateBookingDto(int RoomId, string GuestName, DateOnly CheckIn, DateOnly CheckOut, int NumberOfGuests);
    public record BookingCreatedDto(int Id, string Reference);
    public record BookingDetailsDto(int Id, string Reference, string GuestName, DateOnly CheckIn, DateOnly CheckOut, int NumberOfGuests, RoomDto? Room, HotelDto? Hotel);
}