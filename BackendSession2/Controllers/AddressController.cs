using BackendSession2.Core.Models;
using BackendSession2.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BackendSession2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ILogger _logger;
        private readonly int _success = 0;
        private readonly int _fail = 1;
        private readonly string _msgError = "Some thing went wrong";
        private readonly string _msgSuccess = "SUCCESS";

        public AddressController(IAddressRepository addressRepository, ILogger<AddressController> logger)
        {
            _addressRepository = addressRepository;
            _logger = logger;
        }

        [Route("test")]
        [HttpGet]

        public ActionResult TestRun()
        {
            string value = _addressRepository.testAPI();
            return Ok(value);
        }

        [Route("createProvince")]
        [HttpPost]
        public async Task<IActionResult> createProvince(ProvinceModel addressItem)
        {
            try
            {
                if (String.IsNullOrEmpty(addressItem.ProvinceName))
                {
                    var obj = new ResponseModel
                    {
                        msg = this._msgError,
                        err = this._fail
                    };
                    return Ok(obj);
                }
                else
                {
                    ProvinceModel model = await _addressRepository.createProvince(addressItem);
                    var response = new ResponseModel
                    {
                        msg = this._msgSuccess,
                        err = this._success,
                        data = model
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message} {ex.StackTrace} {Request.GetDisplayUrl()}");
            }
        }

        [Route("getProvinces")]
        [HttpGet]
        public async Task<IActionResult> getProvince()
        {
            try
            {
                var provinces = await _addressRepository.getProvinces();
                return Ok(provinces);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message} {ex.StackTrace} {Request.GetDisplayUrl()}");
            }
        }

        [Route("updateProvince")]
        [HttpPut("id")]
        public async Task<IActionResult> updateProvince([FromRoute] string id, [FromBody] ProvinceModel provinceModel)
        {
            try
            {
                if (String.IsNullOrEmpty(provinceModel.ProvinceName))
                {
                    var obj = new ResponseModel
                    {
                        msg = this._msgError,
                        err = this._fail
                    };
                    return Ok(obj);
                }
                else
                {
                    ProvinceModel model = await _addressRepository.updateProvince(provinceModel);
                    var response = new ResponseModel
                    {
                        msg = this._msgSuccess,
                        err = this._success,
                        data = model
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message} {ex.StackTrace} {Request.GetDisplayUrl()}");
            }
        }

        [Route("deleteProvince")]
        [HttpGet]
        public async Task<IActionResult> deleteProvince([FromBody] string id)
        {
            try
            {
                int result = await _addressRepository.deleteProvince(id);
                var response = new ResponseModel();
                if (result == 0)
                {
                    response = new ResponseModel
                    {
                        msg = this._msgSuccess,
                        err = this._success
                    };
                }
                else
                {
                    response = new ResponseModel
                    {
                        msg = this._msgError,
                        err = this._fail
                    };
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message} {ex.StackTrace} {Request.GetDisplayUrl()}");
            }
        }
    }


}
