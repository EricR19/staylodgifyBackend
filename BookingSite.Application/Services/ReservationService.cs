using BookingSite.Application.DTOs;
using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IGuestRepository _guestRepository;
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IReceiptRepository _receiptRepository;

        public ReservationService(
            IReservationRepository reservationRepository,
            IGuestRepository guestRepository,
            IAvailabilityRepository availabilityRepository,
            IReceiptRepository receiptRepository)
        {
            _reservationRepository = reservationRepository;
            _guestRepository = guestRepository;
            _availabilityRepository = availabilityRepository;
            _receiptRepository = receiptRepository;
        }

        public async Task<IEnumerable<ReservationCreateDto>> GetAllAsync(int tenantId)
        {
            var reservations = await _reservationRepository.GetAllByTenantIdAsync(tenantId);
            var result = new List<ReservationCreateDto>();

            foreach (var r in reservations)
            {
                var guest = r.Guest != null
                    ? new GuestDto
                    {
                        Id = r.Guest.Id,
                        TenantId = r.Guest.TenantId,
                        Name = r.Guest.Name,
                        Email = r.Guest.Email,
                        Phone = r.Guest.Phone
                    }
                    : null;

                result.Add(new ReservationCreateDto
                {
                    Id = r.Id,
                    RoomId = r.Room_Id,
                    TenantId = r.Tenant_Id,
                    GuestId = r.Guest_Id,
                    StartDate = r.Start_Date,
                    EndDate = r.End_Date,
                    Status = r.Status,
                    Guest = guest
                });
            }

            return result;
        }

        public async Task<ReservationCreateDto?> GetByIdAsync(int tenantId, int id)
        {
            var r = await _reservationRepository.GetByIdAndTenantIdAsync(id, tenantId);
            if (r == null) return null;

            var guest = r.Guest != null
                ? new GuestDto
                {
                    Id = r.Guest.Id,
                    TenantId = r.Guest.TenantId,
                    Name = r.Guest.Name,
                    Email = r.Guest.Email,
                    Phone = r.Guest.Phone
                }
                : null;

            return new ReservationCreateDto
            {
                Id = r.Id,
                RoomId = r.Room_Id,
                TenantId = r.Tenant_Id,
                GuestId = r.Guest_Id,
                StartDate = r.Start_Date,
                EndDate = r.End_Date,
                Status = r.Status,
                Guest = guest
            };
        }

        public async Task<IEnumerable<ReservationCreateDto>> GetByRoomIdAsync(int roomId)
        {
            var reservations = await _reservationRepository.GetByRoomIdAsync(roomId);
            var result = new List<ReservationCreateDto>();

            foreach (var r in reservations)
            {
                var guest = r.Guest != null
                    ? new GuestDto
                    {
                        Id = r.Guest.Id,
                        TenantId = r.Guest.TenantId,
                        Name = r.Guest.Name,
                        Email = r.Guest.Email,
                        Phone = r.Guest.Phone
                    }
                    : null;

                result.Add(new ReservationCreateDto
                {
                    Id = r.Id,
                    RoomId = r.Room_Id,
                    TenantId = r.Tenant_Id,
                    GuestId = r.Guest_Id,
                    StartDate = r.Start_Date,
                    EndDate = r.End_Date,
                    Status = r.Status,
                    Guest = guest
                });
            }

            return result;
        }

        public async Task<ReservationCreateDto> CreateAsync(ReservationCreateRequestDto dto, int tenantId)
        {
            // Check if all dates are available before creating the reservation
            var currentDate = dto.StartDate.Date;
            while (currentDate <= dto.EndDate.Date)
            {
                var availability = await _availabilityRepository.GetByRoomIdAndDateAsync(dto.RoomId, currentDate);
                if (availability != null && (availability.is_available == false))
                {
                    throw new InvalidOperationException($"The room is not available on {currentDate:yyyy-MM-dd}.");
                }
                currentDate = currentDate.AddDays(1);
            }

            Guest guest = null;

            // If GuestId is provided, find the existing guest
            if (dto.GuestId.HasValue && dto.GuestId.Value > 0)
            {
                guest = await _guestRepository.GetByIdAsync(dto.GuestId.Value);
                if (guest == null || guest.TenantId != tenantId)
                    throw new KeyNotFoundException("Guest not found for this tenant.");
            }
            // If GuestCreateReservationDto is provided, find by email and tenant, or create a new one
            else if (dto.Guest != null)
            {
                guest = await _guestRepository.GetByEmailAndTenantIdAsync(dto.Guest.Email, tenantId);
                if (guest == null)
                {
                    guest = new Guest
                    {
                        TenantId = tenantId,
                        Name = dto.Guest.Name,
                        Email = dto.Guest.Email,
                        Phone = dto.Guest.Phone
                    };
                    await _guestRepository.AddAsync(guest);
                }
            }
            else
            {
                throw new KeyNotFoundException("Guest information is required.");
            }

            var reservation = new Reservation
            {
                Tenant_Id = tenantId,
                Room_Id = dto.RoomId,
                Guest_Id = guest.Id,
                Start_Date = dto.StartDate,
                End_Date = dto.EndDate,
                Status = dto.Status
            };

            await _reservationRepository.AddAsync(reservation);

            // Block room availability for the reserved dates
            currentDate = reservation.Start_Date.Date;
            while (currentDate <= reservation.End_Date.Date)
            {
                var availability = await _availabilityRepository.GetByRoomIdAndDateAsync(reservation.Room_Id, currentDate);
                if (availability != null)
                {
                    availability.is_available = false;
                    await _availabilityRepository.UpdateAsync(availability);
                }
                else
                {
                    await _availabilityRepository.AddAsync(new Availability
                    {
                        room_id = reservation.Room_Id,
                        Date = currentDate,
                        is_available = false,
                        Price = 0 // Set the price if needed
                    });
                }
                currentDate = currentDate.AddDays(1);
            }

            return new ReservationCreateDto
            {
                Id = reservation.Id,
                RoomId = reservation.Room_Id,
                TenantId = reservation.Tenant_Id,
                GuestId = reservation.Guest_Id,
                StartDate = reservation.Start_Date,
                EndDate = reservation.End_Date,
                Status = reservation.Status,
                Guest = new GuestDto
                {
                    Id = guest.Id,
                    TenantId = guest.TenantId,
                    Name = guest.Name,
                    Email = guest.Email,
                    Phone = guest.Phone
                }
            };
        }

        public async Task<ReservationCreateDto> CreateWithProofAsync(ReservationWithProofRequestDto dto, int tenantId)
        {
            // 1. Create or find guest
            Guest guest = null;
            if (dto.GuestId.HasValue && dto.GuestId.Value > 0)
            {
                guest = await _guestRepository.GetByIdAsync(dto.GuestId.Value);
                if (guest == null || guest.TenantId != tenantId)
                    throw new KeyNotFoundException("Guest not found for this tenant.");
            }
            else
            {
                guest = await _guestRepository.GetByEmailAndTenantIdAsync(dto.GuestEmail, tenantId);
                if (guest == null)
                {
                    guest = new Guest
                    {
                        TenantId = tenantId,
                        Name = dto.GuestName,
                        Email = dto.GuestEmail,
                        Phone = dto.GuestPhone
                    };
                    await _guestRepository.AddAsync(guest);
                }
            }

            // 2. Create reservation
            var reservation = new Reservation
            {
                Tenant_Id = tenantId,
                Room_Id = dto.RoomId,
                Guest_Id = guest.Id,
                Start_Date = dto.StartDate,
                End_Date = dto.EndDate,
                Status = dto.Status
            };
            await _reservationRepository.AddAsync(reservation);

            // 3. Block room availability
            var currentDate = reservation.Start_Date.Date;
            while (currentDate <= reservation.End_Date.Date)
            {
                var availability = await _availabilityRepository.GetByRoomIdAndDateAsync(reservation.Room_Id, currentDate);
                if (availability != null)
                {
                    availability.is_available = false;
                    await _availabilityRepository.UpdateAsync(availability);
                }
                else
                {
                    await _availabilityRepository.AddAsync(new Availability
                    {
                        room_id = reservation.Room_Id,
                        Date = currentDate,
                        is_available = false,
                        Price = 0
                    });
                }
                currentDate = currentDate.AddDays(1);
            }

            // 4. Save payment proof file if present
            if (dto.PaymentProof != null && dto.PaymentProof.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{dto.PaymentProof.FileName}";
                var filePath = Path.Combine("wwwroot/receipts", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.PaymentProof.CopyToAsync(stream);
                }
                var fileUrl = $"/receipts/{fileName}";

                // Save to receipts table
                var receipt = new Receipts
                {
                    Reservation_Id = reservation.Id,
                    File_Url = fileUrl,
                    Uploaded_At = DateTime.UtcNow
                };
                await _receiptRepository.AddAsync(receipt);
            }

            // 5. Return DTO
            return new ReservationCreateDto
            {
                Id = reservation.Id,
                RoomId = reservation.Room_Id,
                TenantId = reservation.Tenant_Id,
                GuestId = reservation.Guest_Id,
                StartDate = reservation.Start_Date,
                EndDate = reservation.End_Date,
                Status = reservation.Status,
                Guest = new GuestDto
                {
                    Id = guest.Id,
                    TenantId = guest.TenantId,
                    Name = guest.Name,
                    Email = guest.Email,
                    Phone = guest.Phone
                }
            };
        }

        public async Task<bool> UpdateAsync(int tenantId, int id, ReservationCreateDto dto)
        {
            var reservation = await _reservationRepository.GetByIdAndTenantIdAsync(id, tenantId);
            if (reservation == null)
                return false;

            reservation.Tenant_Id = tenantId;
            reservation.Room_Id = dto.RoomId;
            reservation.Start_Date = dto.StartDate;
            reservation.End_Date = dto.EndDate;
            reservation.Status = dto.Status;

            await _reservationRepository.UpdateAsync(reservation);
            // Optionally, update availability if dates or room change

            return true;
        }

        public async Task<bool> DeleteAsync(int tenantId, int id)
        {
            var reservation = await _reservationRepository.GetByIdAndTenantIdAsync(id, tenantId);
            if (reservation == null)
                return false;

            // Release availability for all dates in the reservation
            var currentDate = reservation.Start_Date.Date;
            while (currentDate <= reservation.End_Date.Date)
            {
                var availability = await _availabilityRepository.GetByRoomIdAndDateAsync(reservation.Room_Id, currentDate);
                if (availability != null)
                {
                    // Mark as available again or delete the record
                    availability.is_available = true;
                    await _availabilityRepository.UpdateAsync(availability);
                }
                currentDate = currentDate.AddDays(1);
            }

            await _reservationRepository.DeleteAsync(id);

            return true;
        }

        public async Task<bool> ConfirmAsync(int tenantId, int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAndTenantIdAsync(reservationId, tenantId);
            if (reservation == null)
                return false;

            reservation.Status = "confirmed";
            await _reservationRepository.UpdateAsync(reservation);
            return true;
        }

        public async Task<bool> CancelAsync(int tenantId, int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAndTenantIdAsync(reservationId, tenantId);
            if (reservation == null)
                return false;

            reservation.Status = "cancelled";
            await _reservationRepository.UpdateAsync(reservation);

            // Release availability for the cancelled dates
            var currentDate = reservation.Start_Date.Date;
            while (currentDate <= reservation.End_Date.Date)
            {
                var availability = await _availabilityRepository.GetByRoomIdAndDateAsync(reservation.Room_Id, currentDate);
                if (availability != null)
                {
                    availability.is_available = true;
                    await _availabilityRepository.UpdateAsync(availability);
                }
                currentDate = currentDate.AddDays(1);
            }

            return true;
        }
    }
}