﻿using DragonAcc.Service.Interfaces;
using DragonAcc.Service.Models.LuckyWheel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragonAcc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LuckyWheelController : BaseController
    {
        private readonly ILuckyWheelService _luckyWheelService;
        public LuckyWheelController(ILuckyWheelService luckyWheelService)
        {
            _luckyWheelService = luckyWheelService;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _luckyWheelService.GetAll();
            return Response(result);
        }


        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromForm] AddLuckyWheelModel model)
        {
            try
            {
                var result = await _luckyWheelService.Add(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }


        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdateLuckyWheelModel model)
        {
            try
            {
                var result = await _luckyWheelService.Update(model);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
        [Authorize]
        [HttpPost("spin")]
        public async Task<IActionResult> Spin([FromQuery] int prizeId)
        {
            try
            {
                var result = await _luckyWheelService.SpinWheel(prizeId);
                return Response(result);
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

    }
}
