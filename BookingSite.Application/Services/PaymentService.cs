using BookingSite.Application.DTOs;
using BookingSite.Domain.Entities;
using BookingSite.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSite.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync(int tenantId)
        {
            var payments = await _paymentRepository.GetByTenantIdAsync(tenantId);
            return payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                Reservation_Id = p.Reservation_Id,
                Payment_Type = p.Payment_Type,
                Provider = p.Provider,
                Provider_Payment_Id = p.Provider_Payment_Id,
                Card_Last4 = p.Card_Last4,
                Card_Brand = p.Card_Brand,
                Receipt_Url = p.Receipt_Url,
                Amount = p.Amount,
                Currency = p.Currency,
                Status = p.Status,
                Paid_At = p.Paid_At,
                Created_At = p.Created_At
            });
        }

        public async Task<PaymentDto?> GetByIdAsync(int tenantId, int id)
        {
            var payment = await _paymentRepository.GetByIdAndTenantIdAsync(id, tenantId);
            if (payment == null)
                return null;

            return new PaymentDto
            {
                Id = payment.Id,
                Reservation_Id = payment.Reservation_Id,
                Payment_Type = payment.Payment_Type,
                Provider = payment.Provider,
                Provider_Payment_Id = payment.Provider_Payment_Id,
                Card_Last4 = payment.Card_Last4,
                Card_Brand = payment.Card_Brand,
                Receipt_Url = payment.Receipt_Url,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Status = payment.Status,
                Paid_At = payment.Paid_At,
                Created_At = payment.Created_At
            };
        }

        public async Task<PaymentDto> CreateAsync(PaymentCreateDto dto, int tenantId)
        {
            var payment = new Payments
            {
                Tenant_Id = tenantId,
                Reservation_Id = dto.Reservation_Id,
                Payment_Type = dto.Payment_Type,
                Provider = dto.Provider,
                Provider_Payment_Id = dto.Provider_Payment_Id,
                Card_Last4 = dto.Card_Last4,
                Card_Brand = dto.Card_Brand,
                Receipt_Url = dto.Receipt_Url,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Status = dto.Status,
                Paid_At = dto.Paid_At,
                Created_At = System.DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);

            return new PaymentDto
            {
                Id = payment.Id,
                Reservation_Id = payment.Reservation_Id,
                Payment_Type = payment.Payment_Type,
                Provider = payment.Provider,
                Provider_Payment_Id = payment.Provider_Payment_Id,
                Card_Last4 = payment.Card_Last4,
                Card_Brand = payment.Card_Brand,
                Receipt_Url = payment.Receipt_Url,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Status = payment.Status,
                Paid_At = payment.Paid_At,
                Created_At = payment.Created_At
            };
        }

        public async Task<bool> UpdateAsync(int tenantId, int id, PaymentCreateDto dto)
        {
            var payment = await _paymentRepository.GetByIdAndTenantIdAsync(id, tenantId);
            if (payment == null)
                return false;

            payment.Reservation_Id = dto.Reservation_Id;
            payment.Payment_Type = dto.Payment_Type;
            payment.Provider = dto.Provider;
            payment.Provider_Payment_Id = dto.Provider_Payment_Id;
            payment.Card_Last4 = dto.Card_Last4;
            payment.Card_Brand = dto.Card_Brand;
            payment.Receipt_Url = dto.Receipt_Url;
            payment.Amount = dto.Amount;
            payment.Currency = dto.Currency;
            payment.Status = dto.Status;
            payment.Paid_At = dto.Paid_At;

            await _paymentRepository.UpdateAsync(payment);
            return true;
        }

        public async Task<bool> DeleteAsync(int tenantId, int id)
        {
            var payment = await _paymentRepository.GetByIdAndTenantIdAsync(id, tenantId);
            if (payment == null)
                return false;

            await _paymentRepository.DeleteAsync(id);
            return true;
        }
    }
}