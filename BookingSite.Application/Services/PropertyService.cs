using BookingSite.Application.DTOs;
using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace BookingSite.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        // Helper para convertir Property entity a DTO
        private PropertyDto MapToDto(Property p)
        {
            return new PropertyDto
            {
                Id = p.Id,
                TenantId = p.Tenant_Id,
                Name = p.Name,
                Description = p.Description,
                Address = p.Address,
                Phone = p.Phone,
                MainImage = p.Main_Image,
                OtherImages = !string.IsNullOrEmpty(p.Other_Images) 
                    ? JsonSerializer.Deserialize<List<string>>(p.Other_Images) : null,
                Amenities = !string.IsNullOrEmpty(p.Amenities) 
                    ? JsonSerializer.Deserialize<List<string>>(p.Amenities) : null,
                CheckInTime = p.Check_In_Time?.ToString(@"hh\:mm"),
                CheckOutTime = p.Check_Out_Time?.ToString(@"hh\:mm"),
                HouseRules = !string.IsNullOrEmpty(p.House_Rules) 
                    ? JsonSerializer.Deserialize<PropertyHouseRulesDto>(p.House_Rules) : null
            };
        }

        public async Task<IEnumerable<PropertyDto>> GetAllByTenantAsync(int tenantId)
        {
            var properties = await _propertyRepository.GetByTenantIdAsync(tenantId);
            return properties.Select(MapToDto);
        }

        public async Task<PropertyDto?> GetByIdAndTenantAsync(int id, int tenantId)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null || property.Tenant_Id != tenantId)
                return null;
            return MapToDto(property);
        }

        public async Task<PropertyDto> CreatePropertyAsync(PropertyCreateDto dto, int tenantId)
        {
            var property = new Property
            {
                Tenant_Id = tenantId,
                Name = dto.Name,
                Description = dto.Description,
                Address = dto.Address,
                Phone = dto.Phone,
                Main_Image = dto.MainImage,
                Other_Images = dto.OtherImages != null ? JsonSerializer.Serialize(dto.OtherImages) : null,
                Amenities = dto.Amenities != null ? JsonSerializer.Serialize(dto.Amenities) : null,
                Check_In_Time = !string.IsNullOrEmpty(dto.CheckInTime) 
                    ? TimeSpan.Parse(dto.CheckInTime) : TimeSpan.Parse("15:00"),
                Check_Out_Time = !string.IsNullOrEmpty(dto.CheckOutTime) 
                    ? TimeSpan.Parse(dto.CheckOutTime) : TimeSpan.Parse("10:00"),
                House_Rules = dto.HouseRules != null ? JsonSerializer.Serialize(dto.HouseRules) : null
            };

            await _propertyRepository.AddAsync(property);

            return MapToDto(property);
        }

        public async Task<bool> UpdateAsync(int id, PropertyCreateDto dto, int tenantId)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null || property.Tenant_Id != tenantId)
                return false;

            property.Name = dto.Name;
            property.Description = dto.Description;
            property.Address = dto.Address;
            property.Phone = dto.Phone;
            property.Main_Image = dto.MainImage;
            property.Other_Images = dto.OtherImages != null ? JsonSerializer.Serialize(dto.OtherImages) : null;
            property.Amenities = dto.Amenities != null ? JsonSerializer.Serialize(dto.Amenities) : null;
            property.Check_In_Time = !string.IsNullOrEmpty(dto.CheckInTime) 
                ? TimeSpan.Parse(dto.CheckInTime) : property.Check_In_Time;
            property.Check_Out_Time = !string.IsNullOrEmpty(dto.CheckOutTime) 
                ? TimeSpan.Parse(dto.CheckOutTime) : property.Check_Out_Time;
            property.House_Rules = dto.HouseRules != null ? JsonSerializer.Serialize(dto.HouseRules) : property.House_Rules;

            await _propertyRepository.UpdateAsync(property);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null || property.Tenant_Id != tenantId)
                return false;

            await _propertyRepository.DeleteAsync(id);
            return true;
        }
    }
}