using BookingSite.Application.DTOs;
using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IPropertyRepository _propertyRepository;

        public RoomService(IRoomRepository roomRepository, IPropertyRepository propertyRepository)
        {
            _roomRepository = roomRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<IEnumerable<RoomDto>> GetAllAsync(int tenantId)
        {
            var rooms = await _roomRepository.GetAllAsync();
            return rooms
                .Where(r => r.Property != null && r.Property.Tenant_Id == tenantId)
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    PropertyId = r.property_id,
                    RoomNumber = r.room_number,
                    Description = r.description,
                    Capacity = r.capacity,
                    PricePerNight = r.price_per_night,
                    Images = r.images != null ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(r.images) : null
                });
        }

        public async Task<IEnumerable<RoomDto>> GetByPropertyIdAsync(int tenantId, int propertyId)
        {
            var rooms = await _roomRepository.GetByPropertyIdAsync(propertyId);
            return rooms
                .Where(r => r.Property != null && r.Property.Tenant_Id == tenantId)
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    PropertyId = r.property_id,
                    RoomNumber = r.room_number,
                    Description = r.description,
                    Capacity = r.capacity,
                    PricePerNight = r.price_per_night,
                    Images = r.images != null ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(r.images) : null
                });
        }

        public async Task<RoomDto?> GetByIdAsync(int tenantId, int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null || room.Property == null || room.Property.Tenant_Id != tenantId)
                return null;

            return new RoomDto
            {
                Id = room.Id,
                PropertyId = room.property_id,
                RoomNumber = room.room_number,
                Description = room.description,
                Capacity = room.capacity,
                PricePerNight = room.price_per_night,
                Images = room.images != null ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(room.images) : null
            };
        }

        public async Task<RoomDto> CreateAsync(RoomDto dto, int tenantId)
        {
            var property = await _propertyRepository.GetByIdAsync(dto.PropertyId);
            if (property == null || property.Tenant_Id != tenantId)
                throw new UnauthorizedAccessException("Property does not belong to tenant.");

            var room = new Room
            {
                property_id = dto.PropertyId,
                room_number = dto.RoomNumber,
                description = dto.Description,
                capacity = dto.Capacity,
                price_per_night = dto.PricePerNight,
                images = dto.Images != null ? System.Text.Json.JsonSerializer.Serialize(dto.Images) : null,
                created_at = System.DateTime.UtcNow
            };

            await _roomRepository.AddAsync(room);

            dto.Id = room.Id;
            return dto;
        }

        public async Task<bool> UpdateAsync(int tenantId, int id, RoomDto dto)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null || room.Property == null || room.Property.Tenant_Id != tenantId)
                return false;

            var property = await _propertyRepository.GetByIdAsync(dto.PropertyId);
            if (property == null || property.Tenant_Id != tenantId)
                return false;

            room.property_id = dto.PropertyId;
            room.room_number = dto.RoomNumber;
            room.description = dto.Description;
            room.capacity = dto.Capacity;
            room.price_per_night = dto.PricePerNight;
            room.images = dto.Images != null ? System.Text.Json.JsonSerializer.Serialize(dto.Images) : null;

            await _roomRepository.UpdateAsync(room);
            return true;
        }

        public async Task<bool> DeleteAsync(int tenantId, int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null || room.Property == null || room.Property.Tenant_Id != tenantId)
                return false;

            await _roomRepository.DeleteAsync(id);
            return true;
        }
    }
}