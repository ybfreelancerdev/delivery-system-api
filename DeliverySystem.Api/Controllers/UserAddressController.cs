using DeliverySystem.Data.Common;
using DeliverySystem.Data.Models.Entities;
using DeliverySystem.Data.Models.Views;
using DeliverySystem.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace DeliverySystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAddressController : ControllerBase
    {
        #region Property Initialization
        private Authentication.Authorization _authorization;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IUserService _userService;
        private readonly IUserAddressService _userAddressService;
        private int LoggedInUserId
        {
            get
            {
                ClaimsPrincipal userClaims = this.User as ClaimsPrincipal;
                return _authorization.GetUserId(userClaims);
            }
        }
        #endregion

        #region 'Constructor'
        public UserAddressController(IUserService userService, IUserAddressService userAddressService)
        {
            _authorization = new Authentication.Authorization();
            _userService = userService;
            _userAddressService = userAddressService;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }
        #endregion

        [HttpPost("add-address")]
        public async Task<OperationResult<string>> Add(AddressDto dto)
        {
            if (dto.Pincode.Length != 6)
                return new OperationResult<string>(false, "Invalid pincode.");

            if (dto.IsDefault)
            {
                var existingDefaults = await _userAddressService
                    .FindAllAsync(x => x.UserId == LoggedInUserId && x.IsDefault && !x.IsDeleted);

                foreach (var addr in existingDefaults)
                {
                    addr.IsDefault = false;
                    _userAddressService.UpdateAsync(addr, addr.Id);
                }
            }

            var address = new UserAddress
            {
                UserId = LoggedInUserId,
                Address = dto.Address,
                City = dto.City,
                Pincode = dto.Pincode,
                IsDefault = dto.IsDefault
            };

            _userAddressService.Add(address);
            await _userAddressService.SaveAsync();

            return new OperationResult<string>(true, "Address added successfully.");
        }

        [HttpPut("update-address/{id}")]
        public async Task<OperationResult<string>> Update(int id, AddressDto dto)
        {
            if (dto.Pincode.Length != 6)
                return new OperationResult<string>(false, "Invalid pincode.");

            var address = await _userAddressService
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == LoggedInUserId && !x.IsDeleted);

            if (address == null)
                return new OperationResult<string>(false, "Address not found.");

            if (dto.IsDefault)
            {
                var existingDefaults = await _userAddressService
                    .FindAllAsync(x => x.UserId == LoggedInUserId && x.IsDefault && !x.IsDeleted);

                foreach (var addr in existingDefaults)
                {
                    addr.IsDefault = false;
                    _userAddressService.UpdateAsync(addr, addr.Id);
                }
            }

            address.Address = dto.Address;
            address.City = dto.City;
            address.Pincode = dto.Pincode;
            address.IsDefault = dto.IsDefault;

            _userAddressService.UpdateAsync(address, address.Id);
            await _userAddressService.SaveAsync();

            return new OperationResult<string>(true, "Address updated successfully.");
        }

        [HttpDelete("delete-address/{id}")]
        public async Task<OperationResult<string>> Delete(int id)
        {
            var address = await _userAddressService
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == LoggedInUserId && !x.IsDeleted);

            if (address == null)
                return new OperationResult<string>(false, "Address not found.");

            address.IsDeleted = true;

            _userAddressService.UpdateAsync(address, address.Id);
            await _userAddressService.SaveAsync();

            return new OperationResult<string>(true, "Address deleted successfully.");
        }

        [HttpGet("get-address/{id}")]
        public async Task<OperationResult<GetAddressDto>> GetById(int id)
        {
            var address = await _userAddressService
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == LoggedInUserId && !x.IsDeleted);

            if (address == null)
                return new OperationResult<GetAddressDto>(false, "Address not found.");

            var result = new GetAddressDto
            {
                Id = address.Id,
                Address = address.Address,
                City = address.City,
                Pincode = address.Pincode
            };

            return new OperationResult<GetAddressDto>(true, "", result);
        }

        [HttpGet("my-addresses")]
        public async Task<OperationResult<List<GetAddressDto>>> GetAll()
        {
            var list = await _userAddressService
                .FindAllAsync(x => x.UserId == LoggedInUserId && !x.IsDeleted);

            var result = list.Select(x => new GetAddressDto
            {
                Id = x.Id,
                Address = x.Address,
                City = x.City,
                Pincode = x.Pincode
            }).ToList();

            return new OperationResult<List<GetAddressDto>>(true, "", result);
        }
    }
}
