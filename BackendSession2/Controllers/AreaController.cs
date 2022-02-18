using BackendSession2.Core.Models;
using BackendSession2.Core.Repositories;
using BackendSession2.Service;
using BackendSession2.Signalr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BackendSession2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AreaController : ControllerBase
    {
        private IHubContext<SignalrHub, IHubClient> _signalrHub;
        private readonly IAreaRepository _areaRepository;
        private readonly ILogger _logger;
        private readonly int _success = 0;
        private readonly int _fail = 1;
        private readonly string _msgError = "Some thing went wrong";
        private readonly string _msgSuccess = "SUCCESS";

        public AreaController(IAreaRepository areaRepository, ILogger<AreaController> logger, IHubContext<SignalrHub, IHubClient> signalrHub)
        {
            _areaRepository = areaRepository;
            _logger = logger;
            _signalrHub = signalrHub;
        }
        //true: insert false: update
        public bool isValidData(AreaModel area, bool isInsertMethod)
        {
            bool isValid = true;
            if (String.IsNullOrEmpty(area.Code) || String.IsNullOrEmpty(area.Name))
            {
                isValid = false;
            }
            if (String.IsNullOrEmpty(area.ParentId))
            {
                if (area.Type != "province")
                {
                    isValid = false;
                }
            }
            if (!isInsertMethod)
            {
                if (String.IsNullOrEmpty(area.Id.ToString()))
                {
                    isValid = false;
                }
            }
            return isValid;
        }

        [Route("test")]
        [HttpPost]
        public async Task<string> PostMessage([FromBody] MessageInstance msg)
        {
            var retMessage = string.Empty;
            try
            {
                msg.Timestamp = DateTime.UtcNow.ToString();
                await _signalrHub.Clients.All.BroadcastMessage("test");
                retMessage = "Success";
            }
            catch (Exception e)
            {
                retMessage = e.ToString();
            }
            return retMessage;
        }

        [Route("createArea")]
        [HttpPost]
        public async Task<IActionResult> insertArea(AreaModel area)
        {
            try
            {
                if (!(this.isValidData(area, true)))
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
                    AreaModel model = await _areaRepository.insertArea(area);
                    var response = new ResponseModel
                    {
                        msg = this._msgSuccess,
                        err = this._success,
                        data = model
                    };
                    //await _signalrHub.Clients.All.BroadcastMessage("SignalR: Server Add: Them thanh cong");
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message} {ex.StackTrace} {Request.GetDisplayUrl()}");
            }
        }

        [Route("getAres")]
        [HttpGet]
        public async Task<IActionResult> getAres(string type, string parentId)
        {
            if (String.IsNullOrEmpty(type))
            {
                var obj = new ResponseModel
                {
                    msg = "Type not found",
                    err = this._fail
                };
                return Ok(obj);
            }
            else
            {
                if (type != "province")
                {
                    if (String.IsNullOrEmpty(parentId))
                    {
                        var obj = new ResponseModel
                        {
                            msg = "ParentId not found",
                            err = this._fail
                        };
                        return Ok(obj);
                    }
                }
                else
                {
                    parentId = "";
                }
            }
            try
            {
                var areas = await _areaRepository.getAreas(type, parentId);
                return Ok(areas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message} {ex.StackTrace} {Request.GetDisplayUrl()}");
            }
        }

        [Route("updateArea")]
        [HttpPut("id")]
        public async Task<IActionResult> updateProvince([FromRoute] Guid id, [FromBody] AreaModel area)
        {
            try
            {
                if (!this.isValidData(area, false))
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
                    AreaModel model = await _areaRepository.updateArea(area);
                    var response = new ResponseModel
                    {
                        msg = this._msgSuccess,
                        err = this._success,
                        data = model
                    };
                    //await _signalrHub.Clients.All.BroadcastMessage("SignalR: Server Update: Sua thanh cong");
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message} {ex.StackTrace} {Request.GetDisplayUrl()}");
            }
        }

        [Route("deleteArea")]
        [HttpDelete]
        public async Task<IActionResult> deleteProvince(Guid id, string type, string code)
        {
            if(String.IsNullOrEmpty(id.ToString()) || String.IsNullOrEmpty(type) || String.IsNullOrEmpty(code))
            {
                var obj = new ResponseModel
                {
                    msg = "Missing Param",
                    err = this._fail
                };
                return Ok(obj);
            }
            try
            {
                int result = await _areaRepository.deleteArea(id, type, code);
                var response = new ResponseModel();
                if (result == 0)
                {
                    response = new ResponseModel
                    {
                        msg = this._msgError,
                        err = this._fail
                    };
                }
                else
                {
                    response = new ResponseModel
                    {
                        msg = this._msgSuccess,
                        err = this._success
                    };
                }
                //await _signalrHub.Clients.All.BroadcastMessage("SignalR: Server delete: Xoa thanh cong");
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
