using BookingSite.Application.DTOs;
using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<IEnumerable<PropertyDto>> GetAllByTenantAsync(int tenantId)
        {
            var properties = await _propertyRepository.GetByTenantIdAsync(tenantId);
            return properties.Select(p => new PropertyDto
            {
                Id = p.Id,
                TenantId = p.Tenant_Id,
                Name = p.Name,
                Description = p.Description,
                Address = p.Address,
                Phone = p.Phone,
                MainImage = p.Main_Image,
                OtherImages = p.Other_Images != null ?
                    System.Text.Json.JsonSerializer.Deserialize<List<string>>(p.Other_Images) : null
            });
        }

        public async Task<PropertyDto?> GetByIdAndTenantAsync(int id, int tenantId)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null || property.Tenant_Id != tenantId)
                return null;
            return new PropertyDto
            {
                Id = property.Id,
                TenantId = property.Tenant_Id,
                Name = property.Name,
                Description = property.Description,
                Address = property.Address,
                Phone = property.Phone,
                MainImage = property.Main_Image,
                OtherImages = property.Other_Images != null ?
                    System.Text.Json.JsonSerializer.Deserialize<List<string>>(property.Other_Images) : null
            };
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
                Other_Images = dto.OtherImages != null ? System.Text.Json.JsonSerializer.Serialize(dto.OtherImages) : null
            };

            await _propertyRepository.AddAsync(property);

            return new PropertyDto
            {
                Id = property.Id,
                TenantId = property.Tenant_Id,
                Name = property.Name,
                Description = property.Description,
                Address = property.Address,
                Phone = property.Phone,
                MainImage = property.Main_Image,
                OtherImages = property.Other_Images != null ?
                    System.Text.Json.JsonSerializer.Deserialize<List<string>>(property.Other_Images) : null
            };
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
            property.Other_Images = dto.OtherImages != null ? System.Text.Json.JsonSerializer.Serialize(dto.OtherImages) : null;

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