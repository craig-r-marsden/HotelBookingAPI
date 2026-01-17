# Copilot Instructions

## General Guidelines
- First general instruction
- Second general instruction

## Code Style
- Use specific formatting rules
- Follow naming conventions

## Project-Specific Rules
- Preference to move business logic from controllers into service classes; use service layer for Booking operations.
- Preference to move DTO types into a shared Dtos folder and ensure services/controllers reference HotelBookingAPI.Dtos; when asked to move DTOs, update all relevant files (BookingService, RoomService, HotelsRoomsController, BookingsController, etc.) to import and use the HotelBookingAPI.Dtos namespace rather than keeping DTOs declared locally, so code compiles across all relevant files, not only the active document.
- Use the following services: BookingService, RoomService, HotelService. 
- The RoomsController has been replaced by HotelsRoomsController; controllers should be thinned to utilize services. 
- Register these services in Program.cs.