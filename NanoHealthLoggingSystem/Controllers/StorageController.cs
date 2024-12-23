using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanoHealthLoggingSystem.Dtos;
using NanoHealthLoggingSystem.IRepositories;

namespace NanoHealthLoggingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StorageController : ControllerBase
    {
        private readonly IStorageRepository _repository;

        public StorageController(IStorageRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var Storage = await _repository.GetAll();
                return Ok(Storage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> Get(string Id)
        {
            try
            {
                var Storage = await _repository.Get(Id);
                return Ok(Storage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(StorageDto dto)
        {
            try
            {
                var id = await _repository.Add(dto);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _repository.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Put(StorageDto dto)
        {
            try
            {
                var Id =await _repository.Update(dto);
                return Ok(Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
