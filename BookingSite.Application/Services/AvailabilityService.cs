using BookingSite.Application.DTOs;
using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Application.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IRoomRepository _roomRepository;

        public AvailabilityService(IAvailabilityRepository availabilityRepository, IRoomRepository roomRepository)
        {
            _availabilityRepository = availabilityRepository;
            _roomRepository = roomRepository;
        }

        public async Task<IEnumerable<AvailabilityDto>> GetAllAsync(int tenantId)
        {
            var availabilities = await _availabilityRepository.GetAllAsync();
            return availabilities
                .Where(a => a.Room != null && a.Room.Property != null && a.Room.Property.Tenant_Id == tenantId)
                .Select(a => new AvailabilityDto
                {
                    Id = a.Id,
                    RoomId = a.room_id,
                    Date = a.Date,
                    IsAvailable = a.is_available,
                    Price = a.Price
                });
        }

        public async Task<AvailabilityDto?> GetByIdAsync(int tenantId, int id)
        {
            var availability = await _availabilityRepository.GetByIdAsync(id);
            if (availability == null || availability.Room == null || availability.Room.Property == null || availability.Room.Property.Tenant_Id != tenantId)
                return null;

            return new AvailabilityDto
            {
                Id = availability.Id,
                RoomId = availability.room_id,
                Date = availability.Date,
                IsAvailable = availability.is_available,
                Price = availability.Price
            };
        }

        public async Task<AvailabilityDto> CreateAsync(AvailabilityDto dto, int tenantId)
        {
            var room = await _roomRepository.GetByIdAsync(dto.RoomId);
            if (room == null || room.Property == null || room.Property.Tenant_Id != tenantId)
                throw new UnauthorizedAccessException("Room does not belong to tenant.");

            var availability = new Availability
            {
                room_id = dto.RoomId,
                Date = dto.Date,
                is_available = dto.IsAvailable,
                Price = dto.Price
            };

            await _availabilityRepository.AddAsync(availability);

            dto.Id = availability.Id;
            return dto;
        }

        public async Task<bool> UpdateAsync(int tenantId, int id, AvailabilityDto dto)
        {
            var availability = await _availabilityRepository.GetByIdAsync(id);
            if (availability == null || availability.Room == null || availability.Room.Property == null || availability.Room.Property.Tenant_Id != tenantId)
                return false;

            var room = await _roomRepository.GetByIdAsync(dto.RoomId);
            if (room == null || room.Property == null || room.Property.Tenant_Id != tenantId)
                return false;

            availability.room_id = dto.RoomId;
            availability.Date = dto.Date;
            availability.is_available = dto.IsAvailable;
            availability.Price = dto.Price;

            await _availabilityRepository.UpdateAsync(availability);
            return true;
        }

        public async Task<bool> DeleteAsync(int tenantId, int id)
        {
            var availability = await _availabilityRepository.GetByIdAsync(id);
            if (availability == null || availability.Room == null || availability.Room.Property == null || availability.Room.Property.Tenant_Id != tenantId)
                return false;

            await _availabilityRepository.DeleteAsync(id);
            return true;
        }
    }
}