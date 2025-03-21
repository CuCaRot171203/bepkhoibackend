﻿using BepKhoiBackend.BusinessObject.dtos.UserDto.ShipperDto;
using BepKhoiBackend.DataAccess.Repository.UserRepository.ShipperRepository;
using System.Collections.Generic;

namespace BepKhoiBackend.BusinessObject.Services.UserService.ShipperService
{
    public class ShipperService : IShipperService
    {
        private readonly IShipperRepository _shipperRepository;

        public ShipperService(IShipperRepository shipperRepository)
        {
            _shipperRepository = shipperRepository;
        }

        public List<ShipperDTO> GetAllShippers()
        {
            var shippers = _shipperRepository.GetAllShippers();
            return shippers
                .Where(s => s.UserInformation != null) // Lọc ra các User có thông tin hợp lệ
                .Select(s => new ShipperDTO
                {
                    UserId = s.UserId,
                    UserName = s.UserInformation?.UserName ?? "Unknown", // Kiểm tra null
                    Phone = s.Phone,
                    Status = s.Status
                }).ToList();
        }

        public GetShipperDTO GetShipperById(int id)
        {
            var shipper = _shipperRepository.GetShipperById(id);
            if (shipper == null || shipper.UserInformation == null) return null; // Kiểm tra null

            return new GetShipperDTO
            {
                UserId = shipper.UserId,
                UserName = shipper.UserInformation?.UserName ?? "Unknown",
                RoleName = shipper.Role?.RoleName ?? "Unknown",
                Phone = shipper.Phone ?? "N/A",
                Email = shipper.Email ?? "N/A",
                Address = shipper.UserInformation?.Address ?? "N/A",
                Province_City = shipper.UserInformation?.ProvinceCity ?? "N/A",
                District = shipper.UserInformation?.District ?? "N/A",
                Ward_Commune = shipper.UserInformation?.WardCommune ?? "N/A",
                Date_of_Birth = shipper.UserInformation?.DateOfBirth ?? null,
            };
        }


        public void CreateShipper(string email, string password, string phone, string userName)
        {
            _shipperRepository.CreateShipper(email, password, phone, userName);
        }

        public bool UpdateShipper(int userId, string email, string phone, string userName,
                          string address, string provinceCity, string district, string wardCommune,
                          DateTime? dateOfBirth)
        {
            return _shipperRepository.UpdateShipper(userId, email, phone, userName,
                                                    address, provinceCity, district, wardCommune, dateOfBirth);
        }



        public void DeleteShipper(int userId)
        {
            _shipperRepository.DeleteShipper(userId);
        }
        public List<ShipperInvoiceDTO> GetShipperInvoices(int shipperId)
        {
            var invoices = _shipperRepository.GetShipperInvoices(shipperId);

            return invoices.Select(i => new ShipperInvoiceDTO
            {
                InvoiceId = i.InvoiceId,
                CustomerId = i.CustomerId ?? 0,
                CustomerName = i.Customer != null ? i.Customer.CustomerName : "Unknown",
                CheckInTime = i.CheckInTime,
                AmountDue = i.AmountDue,
                Status = i.Status,
                PaymentMethodId = i.PaymentMethodId,
                PaymentMethodName = i.PaymentMethod?.PaymentMethodTitle ?? "Unknown", // Lấy tên phương thức thanh toán
                InvoiceDetails = i.InvoiceDetails.Select(d => new ShipperInvoiceDetailDTO
                {
                    InvoiceDetailId = d.InvoiceDetailId,
                    ProductId = d.ProductId,
                    ProductName = d.Product?.ProductName ?? "Unknown",
                    Quantity = d.Quantity,
                    ProductPrice = d.Price,
                    ProductVAT = d.ProductVat ?? 0m,
                    ProductNote = d.ProductNote
                }).ToList()
            }).ToList();
        }
        public List<ShipperDTO> GetShippersByNameOrPhone(string searchTerm)
        {
            var shippers = _shipperRepository.GetShippersByNameOrPhone(searchTerm);
            return shippers
                .Where(s => s.UserInformation != null)
                .Select(s => new ShipperDTO
                {
                    UserId = s.UserId,
                    UserName = s.UserInformation?.UserName ?? "Unknown",
                    Phone = s.Phone,
                    Status = s.Status
                }).ToList();
        }
        public List<ShipperDTO> GetShipperByStatus(bool status)
        {
            return _shipperRepository.GetShippersSortedByStatus(status)
                .Where(s => s.UserInformation != null)
                .Select(s => new ShipperDTO
                {
                    UserId = s.UserId,
                    UserName = s.UserInformation?.UserName ?? "Unknown",
                    Status = s.Status,
                    Phone = s.Phone
                }).ToList();
        }

    }
}
