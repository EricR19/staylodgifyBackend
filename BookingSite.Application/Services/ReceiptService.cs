using BookingSite.Application.DTOs;
using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Application.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _receiptRepository;
        private readonly IReservationRepository _reservationRepository;

        public ReceiptService(IReceiptRepository receiptRepository, IReservationRepository reservationRepository)
        {
            _receiptRepository = receiptRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<ReceiptDto>> GetAllByTenantAsync(int tenantId)
        {
            var receipts = await _receiptRepository.GetAllAsync();
            // Only receipts whose reservation belongs to this tenant
            return receipts
                .Where(r => r.Reservation != null && r.Reservation.Tenant_Id == tenantId)
                .Select(r => new ReceiptDto
                {
                    Id = r.Id,
                    ReservationId = r.Reservation_Id,
                    FileUrl = r.File_Url,
                    UploadedAt = r.Uploaded_At
                });
        }

        public async Task<ReceiptDto?> GetByIdAsync(int tenantId, int id)
        {
            var receipt = await _receiptRepository.GetByIdAsync(id);
            if (receipt == null || receipt.Reservation == null || receipt.Reservation.Tenant_Id != tenantId)
                return null;

            return new ReceiptDto
            {
                Id = receipt.Id,
                ReservationId = receipt.Reservation_Id,
                FileUrl = receipt.File_Url,
                UploadedAt = receipt.Uploaded_At
            };
        }

        public async Task<IEnumerable<ReceiptDto>> GetByReservationIdAsync(int tenantId, int reservationId)
        {
            var receipts = await _receiptRepository.GetByReservationIdAsync(reservationId);
            // Only if reservation belongs to tenant
            return receipts
                .Where(r => r.Reservation != null && r.Reservation.Tenant_Id == tenantId)
                .Select(r => new ReceiptDto
                {
                    Id = r.Id,
                    ReservationId = r.Reservation_Id,
                    FileUrl = r.File_Url,
                    UploadedAt = r.Uploaded_At
                });
        }

        public async Task<ReceiptDto> CreateAsync(ReceiptDto dto, int tenantId)
        {
            // Validate reservation belongs to tenant
            var reservation = await _reservationRepository.GetByIdAndTenantIdAsync(dto.ReservationId, tenantId);
            if (reservation == null)
                throw new KeyNotFoundException("Reservation not found for this tenant.");

            var receipt = new Receipts
            {
                Reservation_Id = dto.ReservationId,
                File_Url = dto.FileUrl,
                Uploaded_At = dto.UploadedAt
            };

            await _receiptRepository.AddAsync(receipt);

            dto.Id = receipt.Id;
            return dto;
        }

        public async Task<bool> UpdateAsync(int tenantId, int id, ReceiptDto dto)
        {
            var receipt = await _receiptRepository.GetByIdAsync(id);
            if (receipt == null || receipt.Reservation == null || receipt.Reservation.Tenant_Id != tenantId)
                return false;

            receipt.File_Url = dto.FileUrl;
            receipt.Uploaded_At = dto.UploadedAt;

            await _receiptRepository.UpdateAsync(receipt);
            return true;
        }

        public async Task<bool> DeleteAsync(int tenantId, int id)
        {
            var receipt = await _receiptRepository.GetByIdAsync(id);
            if (receipt == null || receipt.Reservation == null || receipt.Reservation.Tenant_Id != tenantId)
                return false;

            await _receiptRepository.DeleteAsync(id);
            return true;
        }
    }
}