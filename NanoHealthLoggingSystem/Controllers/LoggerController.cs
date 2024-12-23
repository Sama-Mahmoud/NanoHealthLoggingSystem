using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanoHealthLoggingSystem.Dtos;
using NanoHealthLoggingSystem.Enums;
using NanoHealthLoggingSystem.IRepositories;

namespace NanoHealthLoggingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User , Admin")]
    public class LoggerController : ControllerBase
    {
        private readonly ILoggingRepository _repository;

        public LoggerController(ILoggingRepository repository)
        {
            _repository = repository;
        }
        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file, [FromForm]LoggingDto Dto  ) {
            try
            {
                var Id = await _repository.Add(Dto);
                if (Dto.StorageId.Equals(StorageType.Filesystem.ToString()))
                {
                    await _repository.SaveInfileStreem(file, Id);
                }
                else if (Dto.StorageId.Equals(StorageType.S3.ToString()))
                {
                    await _repository.SaveInS3(file, Id);

                }
                return Ok(Id);
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("GetLogs")]
        public async Task<IActionResult> Get(Dictionary<string,string> filters)
        {
            try
            {
                var Logges = _repository.GetAll(filters); 
                return Ok(Logges);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetLogFile")]
        public async Task<IActionResult> Get(Guid Id)
        {
            try
            {
                var Logges = await _repository.GetJsonfile(Id);
                return Ok(Logges);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
